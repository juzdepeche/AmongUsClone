using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
	#region Send Data methods
	/// <summary>Sends a packet to a client via TCP.</summary>
	/// <param name="_toClient">The client to send the packet the packet to.</param>
	/// <param name="_packet">The packet to send to the client.</param>
	private static void SendTCPData(int _toClient, Packet _packet)
	{
		_packet.WriteLength();
		Server.clients[_toClient].tcp.SendData(_packet);
	}

	/// <summary>Sends a packet to a client via UDP.</summary>
	/// <param name="_toClient">The client to send the packet the packet to.</param>
	/// <param name="_packet">The packet to send to the client.</param>
	private static void SendUDPData(int _toClient, Packet _packet)
	{
		_packet.WriteLength();
		Server.clients[_toClient].udp.SendData(_packet);
	}

	/// <summary>Sends a packet to all clients via TCP.</summary>
	/// <param name="_packet">The packet to send.</param>
	private static void SendTCPDataToAll(Packet _packet)
	{
		_packet.WriteLength();
		for (int i = 1; i <= Server.MaxPlayers; i++)
		{
			Server.clients[i].tcp.SendData(_packet);
		}
	}

	/// <summary>Sends a packet to all clients except one via TCP.</summary>
	/// <param name="_exceptClient">The client to NOT send the data to.</param>
	/// <param name="_packet">The packet to send.</param>
	private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
	{
		_packet.WriteLength();
		for (int i = 1; i <= Server.MaxPlayers; i++)
		{
			if (i != _exceptClient)
			{
				Server.clients[i].tcp.SendData(_packet);
			}
		}
	}

	/// <summary>Sends a packet to all clients in a specific room via TCP.</summary>
	/// <param name="_roomId">The specific room id.</param>
	/// <param name="_packet">The packet to send.</param>
	private static void SendTCPDataToAll(string _roomId, Packet _packet)
	{
		_packet.WriteLength();
		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			_client.tcp.SendData(_packet);
		}
	}

	/// <summary>Sends a packet to all clients except one via TCP.</summary>
	/// <param name="_roomId">The specific room id.</param>
	/// <param name="_exceptClient">The client to NOT send the data to.</param>
	/// <param name="_packet">The packet to send.</param>
	private static void SendTCPDataToAll(string _roomId, int _exceptClient, Packet _packet)
	{
		_packet.WriteLength();
		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			if (_client.id != _exceptClient)
			{
				_client.tcp.SendData(_packet);
			}
		}
	}

	/// <summary>Sends a packet to all clients via UDP.</summary>
	/// <param name="_packet">The packet to send.</param>
	private static void SendUDPDataToAll(Packet _packet)
	{
		_packet.WriteLength();
		for (int i = 1; i <= Server.MaxPlayers; i++)
		{
			Server.clients[i].udp.SendData(_packet);
		}
	}
	/// <summary>Sends a packet to all clients except one via UDP.</summary>
	/// <param name="_exceptClient">The client to NOT send the data to.</param>
	/// <param name="_packet">The packet to send.</param>
	private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
	{
		_packet.WriteLength();
		for (int i = 1; i <= Server.MaxPlayers; i++)
		{
			if (i != _exceptClient)
			{
				Server.clients[i].udp.SendData(_packet);
			}
		}
	}

	/// <summary>Sends a packet to all clients via UDP.</summary>
	/// <param name="_roomId">The specific room id.</param>
	/// <param name="_packet">The packet to send.</param>
	private static void SendUDPDataToAll(string _roomId, Packet _packet)
	{
		_packet.WriteLength();
		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			_client.udp.SendData(_packet);
		}
	}

	/// <summary>Sends a packet to all clients except one via UDP.</summary>
	/// <param name="_roomId">The specific room id.</param>
	/// <param name="_exceptClient">The client to NOT send the data to.</param>
	/// <param name="_packet">The packet to send.</param>
	private static void SendUDPDataToAll(string _roomId, int _exceptClient, Packet _packet)
	{
		_packet.WriteLength();
		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			if (_client.id != _exceptClient)
			{
				_client.udp.SendData(_packet);
			}
		}
	}
	#endregion

	#region Packets
	/// <summary>Sends a welcome message to the given client.</summary>
	/// <param name="_toClient">The client to send the packet to.</param>
	/// <param name="_msg">The message to send.</param>
	public static void Welcome(int _toClient, string _msg)
	{
		using (Packet _packet = new Packet((int)ServerPackets.welcome))
		{
			_packet.Write(_msg);
			_packet.Write(_toClient);

			SendTCPData(_toClient, _packet);
		}
	}

	/// <summary>Tells a client to spawn a player.</summary>
	/// <param name="_toClient">The client that should spawn the player.</param>
	/// <param name="_player">The player to spawn.</param>
	public static void SpawnPlayer(int _toClient, Player _player)
	{
		using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
		{
			_packet.Write(_player.id);
			_packet.Write(_player.username);
			_packet.Write(_player.colorIndex);
			_packet.Write(_player.transform.position);
			_packet.Write(_player.transform.rotation);

			SendTCPData(_toClient, _packet);
		}
	}

	/// <summary>Sends a player's updated position to all clients.</summary>
	/// <param name="_player">The player whose position to update.</param>
	public static void PlayerPosition(Player _player)
	{
		GameLogic roomGameManager = RoomManager.GetRoomGameManager(PlayerHelper.GetPlayerRoomId(_player.id));
		if (roomGameManager.inMeeting || _player.venting) return;

		using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
		{
			_packet.Write(_player.id);
			_packet.Write(_player.transform.position);

			SendUDPDataToAll(_player.roomId, _packet);
		}
	}

	public static void TeleportPlayer(int _playerId, Vector3 _position)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
		{
			_packet.Write(_playerId);
			_packet.Write(_position);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_playerId), _packet);
		}
	}

	public static void PlayerRole(int _playerId, Role role)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerRole))
		{
			_packet.Write(_playerId);
			_packet.Write((int)role);

			SendTCPData(_playerId, _packet);
		}
	}

	public static void PlayerDisconnected(int _playerId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
		{
			_packet.Write(_playerId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_playerId), _packet);
		}
	}

	public static void PlayerRespawned(int _playerId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
		{
			_packet.Write(_playerId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_playerId), _packet);
		}
	}

	public static void CanKillATarget(int _playerId, bool _canKillATarget)
	{
		using (Packet _packet = new Packet((int)ServerPackets.canKillATarget))
		{
			_packet.Write(_canKillATarget);

			SendUDPData(_playerId, _packet);
		}
	}

	public static void KillTarget(int _playerId, bool _spawnDeadBody)
	{
		using (Packet _packet = new Packet((int)ServerPackets.killTarget))
		{
			_packet.Write(_playerId);
			_packet.Write(_spawnDeadBody);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_playerId), _packet);
		}
	}

	public static void CanReportABody(int _playerId, bool _canReportABody)
	{
		using (Packet _packet = new Packet((int)ServerPackets.canReportABody))
		{
			_packet.Write(_canReportABody);

			SendUDPData(_playerId, _packet);
		}
	}

	public static void StartMeeting(int _reporterId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.startMeeting))
		{
			_packet.Write(_reporterId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_reporterId), _packet);
		}
	}

	public static void PrivateVoteStatus(bool _isSuccess, int _fromId, int _toId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.privateVoteStatus))
		{
			_packet.Write(_isSuccess);
			_packet.Write(_fromId);
			_packet.Write(_toId);

			SendTCPData(_fromId, _packet);
		}
	}

	public static void PublicVoteStatus(bool _isSuccess, int _fromId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.publicVoteStatus))
		{
			_packet.Write(_isSuccess);
			_packet.Write(_fromId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_fromId), _fromId, _packet);
		}
	}

	public static void VoteResults(int _votedOutId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.voteResults))
		{
			_packet.Write(_votedOutId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_votedOutId), _packet);
		}
	}

	public static void RemoveVoteResults(string _roomId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.removeVoteResults))
		{
			SendTCPDataToAll(_roomId, _packet);
		}
	}

	public static void CanDoTask(int _id, bool _canDoTask)
	{
		using (Packet _packet = new Packet((int)ServerPackets.canDoTask))
		{
			_packet.Write(_canDoTask);

			SendTCPData(_id, _packet);
		}
	}

	public static void DoTask(int _playerId, int _taskId, TaskType _taskType)
	{
		using (Packet _packet = new Packet((int)ServerPackets.doTask))
		{
			_packet.Write(_taskId);
			_packet.Write((int)_taskType);

			SendTCPData(_playerId, _packet);
		}
	}

	public static void PlayerLeftTask(int _playerId, TaskType _taskType)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerLeftTask))
		{
			_packet.Write((int)_taskType);

			SendTCPData(_playerId, _packet);
		}
	}

	public static void NewTask(int _playerId, Task _task)
	{
		using (Packet _packet = new Packet((int)ServerPackets.newTask))
		{
			_packet.Write(_task.id);
			_packet.Write(_task.done);
			_packet.Write(_task.position);
			_packet.Write((int)_task.taskType);

			SendTCPData(_playerId, _packet);
		}
	}

	public static void TaskDone(string _roomId, Task _task)
	{
		using (Packet _packet = new Packet((int)ServerPackets.taskDone))
		{
			_packet.Write(_task.id);
			_packet.Write((int)_task.taskType);

			SendTCPDataToAll(_roomId, _packet);
		}
	}

	public static void CanVent(int _playerId, bool _canVent)
	{
		using (Packet _packet = new Packet((int)ServerPackets.canVent))
		{
			_packet.Write(_canVent);

			SendTCPData(_playerId, _packet);
		}
	}

	public static void PlayerVentIn(int _playerId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerVentIn))
		{
			_packet.Write(_playerId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_playerId), _packet);
		}
	}

	public static void PlayerVentOut(int _playerId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerVentOut))
		{
			_packet.Write(_playerId);

			SendTCPDataToAll(PlayerHelper.GetPlayerRoomId(_playerId), _packet);
		}
	}

	public static void PossibleVentsDirections(Player _player, GameObject[] _possibleVents)
	{
		using (Packet _packet = new Packet((int)ServerPackets.possibleVentsDirections))
		{
			_packet.Write(_player.currentVent.transform.position);
			_packet.Write(_possibleVents.Length);

			foreach (GameObject _vent in _possibleVents)
			{
				_packet.Write(_vent.GetComponent<Vent>().id);
				_packet.Write(_vent.transform.position);
			}

			SendTCPData(_player.id, _packet);
		}
	}

	public static void PlayerVentUpdated(int _playerId)
	{
		using (Packet _packet = new Packet((int)ServerPackets.playerVentUpdated))
		{
			SendTCPData(_playerId, _packet);
		}
	}

	public static void MapPosition(int _playerId, Vector3 _mapPosition)
	{
		using (Packet _packet = new Packet((int)ServerPackets.mapPosition))
		{
			_packet.Write(_mapPosition);

			SendTCPData(_playerId, _packet);
		}
	}

	public static void SetPartyLeader(int _playerId, bool _isPartyLeader)
	{
		using (Packet _packet = new Packet((int)ServerPackets.setPartyLeader))
		{
			_packet.Write(_isPartyLeader);

			SendTCPData(_playerId, _packet);
		}
	}
	#endregion
}
