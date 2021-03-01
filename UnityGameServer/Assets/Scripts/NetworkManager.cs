using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
	public static NetworkManager instance;

	public GameObject playerPrefab;

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

	private void Start()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;

		Server.Start(50, 26950);
	}

	private void OnApplicationQuit()
	{
		Server.Stop();
	}

	public Player InstantiatePlayer(string _roomId)
	{
		// todo roomid get map position
		return Instantiate(playerPrefab, PlayerHelper.GetRandomPreGameLobbySpawnPosition(_roomId), Quaternion.identity).GetComponent<Player>();
	}

	public GameObject InstantiateRoom(string _roomId)
	{
		return RoomManager.CreateRoom(_roomId);
	}
}
