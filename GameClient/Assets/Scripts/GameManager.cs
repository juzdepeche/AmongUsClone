using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
	public static Dictionary<int, PlayerManager> deadPlayers = new Dictionary<int, PlayerManager>();
	private List<GameObject> deadBodies = new List<GameObject>();

	public GameObject localPlayerPrefab;
	public GameObject deadBodyPrefab;
	public GameObject playerPrefab;
	public List<Color> playerColors = new List<Color>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Debug.Log("Instance already exists, destroying object!");
			Destroy(this);
		}
	}

	public void SpawnPlayer(int _id, string _username, int _colorIndex, Vector3 _position, Quaternion _rotation)
	{
		GameObject _player;
		if (_id == Client.instance.myId)
		{
			_player = Instantiate(localPlayerPrefab, _position, _rotation);
			CameraFollow.instance.target = _player.transform;
		}
		else
		{
			_player = Instantiate(playerPrefab, _position, _rotation);
		}

		_player.GetComponent<PlayerManager>().Initialize(_id, _username, _colorIndex, _position);
		players.Add(_id, _player.GetComponent<PlayerManager>());
	}

	public void SpawnDeadPlayer(Vector3 _position, int _colorIndex)
	{
		var _newDeadBody = Instantiate(deadBodyPrefab, _position, Quaternion.identity);
		_newDeadBody.GetComponent<DeadBody>().SetColor(_colorIndex);
		deadBodies.Add(_newDeadBody);
	}

	public void RemoveDeadBodies()
	{
		foreach (var _deadBody in deadBodies)
		{
			Destroy(_deadBody);
		}

		deadBodies = new List<GameObject>();
	}

	public void ShowOtherDeadPlayers()
	{
		foreach (PlayerManager _player in players.Values)
		{
			if (_player.dead)
			{
				_player.MakeBodyTransparent();
			}
		}
	}
}
