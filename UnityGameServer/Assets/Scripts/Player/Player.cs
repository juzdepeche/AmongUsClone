using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public int id;
	public string roomId;
	public string username;
	public int colorIndex;
	public bool isPartyLeader;
	public float moveSpeed = 5f;
	private Rigidbody2D rb;
	public Role role;
	public bool dead = false;

	private bool[] inputs;
	public Player killTarget = null;
	public bool canReportTarget = false;
	public Task taskTarget = null;
	public Task taskIsDoing = null;
	public GameObject currentVent = null;
	public bool venting = false;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	public void Initialize(int _id, string _roomId, string _username, int _colorIndex)
	{
		id = _id;
		roomId = _roomId;
		username = _username;
		colorIndex = _colorIndex;

		inputs = new bool[5];
	}

	/// <summary>Processes player input and moves the player.</summary>
	public void FixedUpdate()
	{
		Vector2 _inputDirection = Vector2.zero;
		if (inputs[0])
		{
			_inputDirection.y += 1;
		}
		if (inputs[1])
		{
			_inputDirection.y -= 1;
		}
		if (inputs[2])
		{
			_inputDirection.x -= 1;
		}
		if (inputs[3])
		{
			_inputDirection.x += 1;
		}

		Move(_inputDirection);
	}

	private void Update()
	{
		// todo if (dead) return;
		LookForKillTarget();
		LookForDeadBodyToReport();
		LookForTask();
		CheckDoingTaskDistance();
		LookForVent();
	}

	private void LookForKillTarget()
	{
		killTarget = null;
		if (role == Role.Imposter)
		{
			foreach (Client _client in Server.clients.Values)
			{
				if (_client.id == id || _client.player == null || dead || _client.player.dead /*|| _client.player.role == Role.Imposter*/) continue;

				float distance = Vector3.Distance(transform.position, _client.player.transform.position);
				if (distance <= GameLogic.interactRadius)
				{
					if (killTarget != null)
					{
						float presentKillTargetDistance = Vector3.Distance(transform.position, killTarget.transform.position);
						killTarget = presentKillTargetDistance > distance ? _client.player : killTarget;
					}
					else
					{
						killTarget = _client.player;
					}
				}
			}

			ServerSend.CanKillATarget(id, killTarget != null);
		}
	}

	private void LookForDeadBodyToReport()
	{
		canReportTarget = false;
		foreach (Vector3 _deadBody in GameLogic.deadBodyPosition.Values)
		{
			float distance = Vector3.Distance(transform.position, _deadBody);
			if (distance <= GameLogic.interactRadius)
			{
				canReportTarget = true;
			}
		}

		ServerSend.CanReportABody(id, canReportTarget);
	}

	private void LookForTask()
	{
		if (taskIsDoing != null) return;
		taskTarget = null;
		GameLogic roomGameManager = RoomManager.GetRoomGameManager(PlayerHelper.GetPlayerRoomId(id));
		foreach (Task _task in roomGameManager.GetTasks())
		{
			float distance = Vector3.Distance(transform.position, _task.position);
			if (distance <= GameLogic.interactRadius)
			{
				taskTarget = _task;
			}
		}

		ServerSend.CanDoTask(id, taskTarget != null);
	}

	private void CheckDoingTaskDistance()
	{
		if (taskIsDoing == null) return;
		float distance = Vector3.Distance(transform.position, taskIsDoing.position);

		if (distance > GameLogic.interactRadius)
		{
			ServerSend.PlayerLeftTask(id, taskIsDoing.taskType);
			taskIsDoing = null;
		}
	}

	private void LookForVent()
	{
		currentVent = null;
		GameLogic roomGameManager = RoomManager.GetRoomGameManager(PlayerHelper.GetPlayerRoomId(id));
		foreach (GameObject _vent in roomGameManager.vents)
		{
			float distance = Vector3.Distance(transform.position, _vent.transform.position);
			if (distance <= GameLogic.interactRadius)
			{
				currentVent = _vent;
			}
		}

		ServerSend.CanVent(id, currentVent != null);
	}

	/// <summary>Calculates the player's desired movement direction and moves him.</summary>
	/// <param name="_inputDirection"></param>
	private void Move(Vector2 _inputDirection)
	{
		rb.MovePosition(rb.position + _inputDirection * moveSpeed * Time.fixedDeltaTime);

		ServerSend.PlayerPosition(this);
	}

	/// <summary>Updates the player input with newly received input.</summary>
	/// <param name="_inputs">The new key inputs.</param>
	public void SetInput(bool[] _inputs)
	{
		inputs = _inputs;
	}

	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(5f);

		ServerSend.PlayerRespawned(id);
	}

	public void Die(bool _addDeadBody)
	{
		dead = true;
		rb.isKinematic = true;
		GetComponent<CapsuleCollider2D>().isTrigger = true;

		if (_addDeadBody)
		{
			GameLogic.deadBodyPosition.Add(id, transform.position);
		}

		ServerSend.KillTarget(id, _addDeadBody);
	}
}
