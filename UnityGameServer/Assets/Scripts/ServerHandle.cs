using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
	public static void WelcomeReceived(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		string _username = _packet.ReadString();

		Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
		if (_fromClient != _clientIdCheck)
		{
			Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
		}
		Server.clients[_fromClient].SendIntoGame(_username);
	}

	public static void PlayerMovement(int _fromClient, Packet _packet)
	{
		if (GameLogic.instance.inMeeting) return;

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
				GameLogic.instance.CallMeeting(_fromClient);
			}
		}
	}

	public static void Vote(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			int _playerVoted = _packet.ReadInt();

			VoteRegister.RegisterVote(_fromClient, _playerVoted);
		}
	}

	public static void Interact(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = PlayerHelper.GetPlayerById(_fromClient);
			if (player.taskTarget != null && !player.dead && !player.taskTarget.done && player.taskIsDoing == null)
			{
				player.taskIsDoing = player.taskTarget;
				ServerSend.DoTask(_fromClient, player.taskTarget.id, player.taskTarget.taskType);
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
				player.taskIsDoing.done = true;
				player.taskIsDoing.doneByPlayerId = _fromClient;
				player.taskIsDoing.taskObject.GetComponent<SpriteRenderer>().color = Color.green;

				ServerSend.TaskDone(player.taskIsDoing);
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

	public static void GoToSend(int _fromClient, Packet _packet)
	{
		int _clientIdCheck = _packet.ReadInt();
		if (_fromClient == _clientIdCheck)
		{
			Player player = PlayerHelper.GetPlayerById(_fromClient);
			if (player.role == Role.Imposter && player.currentVent != null && player.venting)
			{
				int _toVentId = _packet.ReadInt();
				foreach (GameObject _vent in GameLogic.instance.vents)
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
}
