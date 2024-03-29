using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	public const float RoomDistance = 150f;
	public static RoomManager instance;
	public List<GameObject> roomMaps;
	public GameObject roomMapPrefab;
	public int roomCount = 0;

	private void Awake()
	{
		instance = this;
	}

	public static GameLogic GetRoomGameManager(string _roomId)
	{
		return GameObject.Find($"/{_roomId}/GameManager").GetComponent<GameLogic>();
	}

	public static VoteRegister GetRoomVoteRegister(string _roomId)
	{
		return GameObject.Find($"/{_roomId}/GameManager").GetComponent<VoteRegister>();
	}

	public static TaskManager GetRoomTaskManager(string _roomId)
	{
		return GameObject.Find($"/{_roomId}/GameManager").GetComponent<TaskManager>();
	}

	public static CameraManager GetRoomCameraManager(string _roomId)
	{
		return GameObject.Find($"/{_roomId}/Cameras").GetComponent<CameraManager>();
	}

	public static Vector3 GetRoomPosition()
	{
		return new Vector3(RoomDistance * RoomManager.instance.roomCount, 0, 0);
	}

	public static GameObject CreateRoom(string _roomId)
	{
		var room = Instantiate(RoomManager.instance.roomMapPrefab, RoomManager.GetRoomPosition(), Quaternion.identity);
		room.GetComponentInChildren<GameLogic>().roomId = _roomId;
		room.GetComponentInChildren<VoteRegister>().roomId = _roomId;
		room.GetComponentInChildren<TaskManager>().roomId = _roomId;
		room.GetComponentInChildren<CameraManager>().roomId = _roomId;
		room.name = _roomId;
		RoomManager.instance.roomCount++;

		return room;
	}

	public static void RemoveRoom()
	{
		//todo roomid
	}
}
