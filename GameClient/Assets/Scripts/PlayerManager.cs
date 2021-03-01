using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
	public int id;
	public string username;
	public int colorIndex;
	public Role role = Role.Unknown;
	public bool isPartyLeader = false;
	public TextMeshProUGUI usernameText;
	public float interpolateMovementFactor;
	public SpriteRenderer colorSpriteRenderer;
	public SpriteRenderer outlineSpriteRenderer;
	private Vector3 fromPos = Vector3.zero;
	private Vector3 toPos = Vector3.zero;
	public GameObject deadBodyPrefab;
	public bool dead = false;
	public int taskIsDoingId = -1;
	public GameObject ventArrowPrefab;
	public List<GameObject> ventArrows = new List<GameObject>();

	private float lastTime;

	public void Initialize(int _id, string _username, int _colorIndex, Vector3 _startPosition)
	{
		id = _id;
		username = _username;
		colorIndex = _colorIndex;
		fromPos = _startPosition;
		toPos = _startPosition;

		usernameText.text = _username;
		colorSpriteRenderer.color = GameManager.instance.playerColors[_colorIndex];
	}

	public void SetPosition(Vector3 position)
	{
		fromPos = toPos;
		toPos = position;
		lastTime = Time.time;
	}

	public void SetRole(Role _role)
	{
		role = _role;
	}

	private void Update()
	{
		float sharpness = (Time.time - lastTime) / (1.0f / Constants.TICKS_PER_SEC);
		transform.position = Vector3.Lerp(fromPos, toPos, sharpness);
	}

	public void Die(bool _spawnDeadBody)
	{
		if (_spawnDeadBody)
		{
			GameManager.instance.SpawnDeadPlayer(transform.position, GameManager.players[id].colorIndex);
		}

		dead = true;
		if (Client.instance.myId == id)
		{
			MakeBodyTransparent();
			GameManager.instance.ShowOtherDeadPlayers();
		}
		else if (GameManager.players[Client.instance.myId].GetComponent<PlayerManager>().dead)
		{
			MakeBodyTransparent();
		}
		else
		{
			MakeBodyTransparent(false);
		}
	}

	public void MakeBodyTransparent(bool visible = true)
	{
		Color playerColor = GameManager.instance.playerColors[colorIndex];
		if (visible)
		{
			playerColor.a = 0.6f;
			usernameText.enabled = true;
		}
		else
		{
			playerColor.a = 0f;
			usernameText.enabled = false;
		}

		colorSpriteRenderer.color = playerColor;
		outlineSpriteRenderer.color = playerColor;
	}

	public void Respawn()
	{
		// change position
	}

	public void Hide()
	{
		usernameText.enabled = false;
		Color colorColor = colorSpriteRenderer.color;
		Color outlineColor = outlineSpriteRenderer.color;

		colorColor.a = 0;
		outlineColor.a = 0;

		colorSpriteRenderer.color = colorColor;
		outlineSpriteRenderer.color = outlineColor;
	}

	public void HideArrows()
	{
		foreach (GameObject _ventArrow in ventArrows)
		{
			Destroy(_ventArrow);
		}
		ventArrows = new List<GameObject>();
	}

	public void Show()
	{
		usernameText.enabled = true;
		Color colorColor = colorSpriteRenderer.color;
		Color outlineColor = outlineSpriteRenderer.color;

		colorColor.a = 1;
		outlineColor.a = 1;

		colorSpriteRenderer.color = colorColor;
		outlineSpriteRenderer.color = outlineColor;
	}

	public void ShowVentArrows(Vector3 _currentPosition, Dictionary<int, Vector3> _ventPositions)
	{
		HideArrows();
		foreach (var _ventPosition in _ventPositions)
		{
			var dir = _ventPosition.Value - _currentPosition;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			Quaternion direction = Quaternion.AngleAxis(angle, Vector3.forward);

			var _newVentArrow = Instantiate(ventArrowPrefab, _currentPosition + dir.normalized * 2, direction);
			_newVentArrow.GetComponent<VentArrow>().Initialize(_ventPosition.Key);
			ventArrows.Add(_newVentArrow);
		}
	}
}
