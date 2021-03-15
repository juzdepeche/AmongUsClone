using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
	public static void WelcomeReceived(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		string _username = _packet.ReadString();
		string _roomId = _packet.ReadString();

		Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient} in room {_roomId}.");
		if (_fromClient != _clientIdCheck)
		{
			Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
		}

		bool _isPartyLeader = false;
		if (!Server.rooms.ContainsKey(_roomId))
		{
			Server.rooms.Add(_roomId, new Room(_roomId));
			Vector3 _mapPosition = NetworkManager.instance.InstantiateRoom(_roomId).transform.position;
			Server.rooms[_roomId].position = _mapPosition;

			_isPartyLeader = true;
		}

		Server.rooms[_roomId].clients.Add(_fromClient, Server.clients[_fromClient]);
		Server.clients[_fromClient].SendIntoGame(_username, _roomId);
		Server.clients[_fromClient].roomId = _roomId;
		Server.clients[_fromClient].player.isPartyLeader = _isPartyLeader;
		ServerSend.SetPartyLeader(_fromClient, _isPartyLeader);
		ServerSend.MapPosition(_fromClient, Server.rooms[_roomId].position);
	}

	public static void PlayerMovement(int _fromClient, Packet _packet)
	{
		//todo roomid rename gamelogic to gamemanager
		GameLogic roomManager = RoomManager.GetRoomGameManager(PlayerHelper.GetPlayerRoomId(_fromClient));
		if (roomManager.inMeeting) return;

		Player player = PlayerHelper.GetPlayerById(_fromClient);
		if (player.venting) return;

		bool[] _inputs = new bool[_packet.ReadInt()];
		for (int i = 0; i < _inputs.Length; i++)
		{
			_inputs[i] = _packet.ReadBool();
		}

		Server.clients[_fromClient].player.SetInput(_inputs);
	}

	public static void Kill(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = Server.clients[_fromClient].player;
			if (player.role == Role.Imposter && player.killTarget != null)
			{
				player.killTarget.Die(true);
			}
		}
	}

	public static void Report(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = Server.clients[_fromClient].player;
			if (player.canReportTarget)
			{
				string _roomId = PlayerHelper.GetPlayerRoomId(_fromClient);
				GameLogic roomManager = RoomManager.GetRoomGameManager(_roomId);
				roomManager.CallMeeting(_fromClient);
			}
		}
	}

	public static void Vote(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			int _playerVoted = _packet.ReadInt();

			string _roomId = PlayerHelper.GetPlayerRoomId(_fromClient);
			VoteRegister _voteRegister = RoomManager.GetRoomVoteRegister(_roomId);
			_voteRegister.RegisterVote(_fromClient, _playerVoted);
		}
	}

	public static void Interact(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = PlayerHelper.GetPlayerById(_fromClient);
			InteractTarget _interactTarget = player.GetInteractTarget();
			switch (_interactTarget)
			{
				case InteractTarget.Camera:
					player.OpenCamera();
					break;
				case InteractTarget.Task:
					if (player.taskTarget != null && !player.dead && !player.taskTarget.done && player.taskIsDoing == null)
					{
						player.taskIsDoing = player.taskTarget;
						ServerSend.DoTask(_fromClient, player.taskTarget.id, player.taskTarget.taskType);
					}
					break;
			}
		}
	}

	public static void TaskDone(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			int _taskType = _packet.ReadInt();
			Player player = PlayerHelper.GetPlayerById(_fromClient);

			//todo check id instead for cheaters
			if (player.taskIsDoing.taskType == (TaskType)_taskType)
			{
				TaskManager _roomTaskManager = RoomManager.GetRoomTaskManager(PlayerHelper.GetPlayerRoomId(_fromClient));
				_roomTaskManager.TaskDone(player.taskIsDoing.id, _fromClient);
				ServerSend.TaskDone(PlayerHelper.GetPlayerRoomId(_fromClient), player.taskIsDoing);
			}
		}
	}

	public static void Vent(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = PlayerHelper.GetPlayerById(_fromClient);
			if (player.role == Role.Imposter && player.currentVent != null)
			{
				if (player.venting)
				{
					player.venting = false;
					ServerSend.PlayerVentOut(_fromClient);
				}
				else
				{
					player.venting = true;
					player.transform.position = player.currentVent.transform.position;

					ServerSend.PlayerVentIn(_fromClient);
					ServerSend.TeleportPlayer(_fromClient, player.currentVent.transform.position);
					ServerSend.PossibleVentsDirections(player, player.currentVent.GetComponent<Vent>().possibleVents);
				}
			}
		}
	}

	public static void GoToVent(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = PlayerHelper.GetPlayerById(_fromClient);
			if (player.role == Role.Imposter && player.currentVent != null && player.venting)
			{
				int _toVentId = _packet.ReadInt();
				GameLogic roomGameManager = RoomManager.GetRoomGameManager(PlayerHelper.GetPlayerRoomId(_fromClient));
				foreach (GameObject _vent in roomGameManager.vents)
				{
					if (_vent.GetComponent<Vent>().id == _toVentId)
					{
						player.currentVent = _vent;
						player.transform.position = player.currentVent.transform.position;

						ServerSend.PlayerVentUpdated(_fromClient);
						ServerSend.TeleportPlayer(_fromClient, _vent.transform.position);
						ServerSend.PossibleVentsDirections(player, _vent.GetComponent<Vent>().possibleVents);
						return;
					}
				}

				//problem
				player.venting = false;
				ServerSend.PlayerVentOut(_fromClient);
			}
		}
	}

	public static void StartGame(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = PlayerHelper.GetPlayerById(_fromClient);
			GameLogic roomGameManager = RoomManager.GetRoomGameManager(PlayerHelper.GetPlayerRoomId(_fromClient));
			if (player.isPartyLeader && !roomGameManager.hasGameStarted)
			{
				// todo restart game
				roomGameManager.StartGame();
			}
		}
	}
}
