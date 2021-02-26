using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
	public static void Welcome(Packet _packet)
	{
		string _msg = _packet.ReadString();
		int _myId = _packet.ReadInt();

		Debug.Log($"Message from server: {_msg}");
		Client.instance.myId = _myId;
		ClientSend.WelcomeReceived();

		// Now that we have the client's id, connect UDP
		Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
	}

	public static void SpawnPlayer(Packet _packet)
	{
		int _id = _packet.ReadInt();
		string _username = _packet.ReadString();
		int _colorIndex = _packet.ReadInt();
		Vector3 _position = _packet.ReadVector3();
		Quaternion _rotation = _packet.ReadQuaternion();

		GameManager.instance.SpawnPlayer(_id, _username, _colorIndex, _position, _rotation);
	}

	public static void PlayerPosition(Packet _packet)
	{
		int _id = _packet.ReadInt();
		Vector3 _position = _packet.ReadVector3();

		PlayerManager player = GameManager.players[_id]?.GetComponent<PlayerManager>();
		if (player)
		{
			player.SetPosition(_position);
		}
	}

	public static void PlayerRole(Packet _packet)
	{
		int _id = _packet.ReadInt();
		int _role = _packet.ReadInt();

		GameManager.players[_id].GetComponent<PlayerManager>().SetRole((Role)_role);
	}

	public static void PlayerDisconnected(Packet _packet)
	{
		int _id = _packet.ReadInt();

		Destroy(GameManager.players[_id].gameObject);
		GameManager.players.Remove(_id);
	}

	public static void PlayerRespawned(Packet _packet)
	{
		int _id = _packet.ReadInt();

		GameManager.players[_id].Respawn();
	}

	public static void CanKillATarget(Packet _packet)
	{
		bool _canKillATarget = _packet.ReadBool();

		UIControl.instance.setKillButtonInteractable(_canKillATarget);
	}

	public static void KillTarget(Packet _packet)
	{
		int _id = _packet.ReadInt();
		bool _spawnDeadBody = _packet.ReadBool();

		GameManager.players[_id].Die(_spawnDeadBody);
	}

	public static void CanReportABody(Packet _packet)
	{
		bool _canReportABody = _packet.ReadBool();

		UIControl.instance.setReportButtonInteractable(_canReportABody);
	}

	public static void StartMeeting(Packet _packet)
	{
		int _reporterId = _packet.ReadInt();

		UIMeeting.instance.StartMeeting(_reporterId);
		GameManager.instance.RemoveDeadBodies();
	}

	public static void PrivateVoteStatus(Packet _packet)
	{
		bool _isSuccess = _packet.ReadBool();
		int _fromId = _packet.ReadInt();
		int _toId = _packet.ReadInt();

		UIMeeting.instance.PrivateVoteSuccess(_isSuccess, _fromId, _toId);
	}

	public static void PublicVoteStatus(Packet _packet)
	{
		bool _isSuccess = _packet.ReadBool();
		int _fromId = _packet.ReadInt();

		UIMeeting.instance.PublicVoteSuccess(_isSuccess, _fromId);
	}

	public static void VoteResults(Packet _packet)
	{
		int _votedOutId = _packet.ReadInt();
		string _username = PlayerHelper.GetPlayerById(_votedOutId).username;

		UIMeeting.instance.VoteResults();
		UIVoteResults.instance.ShowUI(_username);
	}

	public static void RemoveVoteResults(Packet _packet)
	{
		UIVoteResults.instance.HideUI();
	}

	public static void CanDoTask(Packet _packet)
	{
		bool _canDoTask = _packet.ReadBool();

		UIControl.instance.setUseButtonInteractable(_canDoTask);
	}

	public static void DoTask(Packet _packet)
	{
		int _taskId = _packet.ReadInt();
		TaskType _taskType = (TaskType)_packet.ReadInt();

		PlayerManager _player = PlayerHelper.GetPlayerThisPlayer();
		_player.taskIsDoingId = _taskId;

		TaskManager.instance.DoTask(_taskType);
	}

	public static void PlayerLeftTask(Packet _packet)
	{
		TaskType _taskType = (TaskType)_packet.ReadInt();

		TaskManager.instance.LeaveTask(_taskType);
	}

	public static void NewTask(Packet _packet)
	{
		int _id = _packet.ReadInt();
		bool _done = _packet.ReadBool();
		Vector3 _position = _packet.ReadVector3();
		TaskType _taskType = (TaskType)_packet.ReadInt();

		TaskManager.instance.NewTask(_id, _done, _position, _taskType);
	}

	public static void TaskDone(Packet _packet)
	{
		int _taskId = _packet.ReadInt();
		TaskType _taskType = (TaskType)_packet.ReadInt();

		TaskManager.instance.LeaveTask(_taskId, _taskType);
		TaskManager.instance.TaskIsReallyDone(_taskId);
		//todo change task ui in game (remove highlight)
	}

	public static void CanVent(Packet _packet)
	{
		bool _canVent = _packet.ReadBool();

		UIControl.instance.setVentButtonInteractable(_canVent);
	}

	public static void PlayerVentIn(Packet _packet)
	{
		int _playerId = _packet.ReadInt();
		PlayerManager player = PlayerHelper.GetPlayerById(_playerId);
		player.Hide();
	}

	public static void PlayerVentOut(Packet _packet)
	{
		int _playerId = _packet.ReadInt();
		PlayerManager player = PlayerHelper.GetPlayerById(_playerId);
		player.Show();
		player.HideArrows();
	}

	public static void PossibleVentsDirections(Packet _packet)
	{
		Vector3 currentPosition = _packet.ReadVector3();
		int length = _packet.ReadInt();

		Dictionary<int, Vector3> ventsPositions = new Dictionary<int, Vector3>();
		PlayerManager player = PlayerHelper.GetPlayerThisPlayer();
		for (int i = 0; i < length; i++)
		{
			int _ventId = _packet.ReadInt();
			Vector3 _possibleVent = _packet.ReadVector3();
			ventsPositions.Add(_ventId, _possibleVent);

			Debug.DrawLine(currentPosition, _possibleVent, Color.blue, 15f);
		}
		player.ShowVentArrows(currentPosition, ventsPositions);
	}

	public static void PlayerVentUpdated(Packet _packet)
	{
		// PlayerHelper.GetPlayerThisPlayer().HideArrows();
	}
}
