using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
	/// <summary>Sends a packet to the server via TCP.</summary>
	/// <param name="_packet">The packet to send to the sever.</param>
	private static void SendTCPData(Packet _packet)
	{
		_packet.WriteLength();
		Client.instance.tcp.SendData(_packet);
	}

	/// <summary>Sends a packet to the server via UDP.</summary>
	/// <param name="_packet">The packet to send to the sever.</param>
	private static void SendUDPData(Packet _packet)
	{
		_packet.WriteLength();
		Client.instance.udp.SendData(_packet);
	}

	#region Packets
	/// <summary>Lets the server know that the welcome message was received.</summary>
	public static void WelcomeReceived()
	{
		using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
		{
			_packet.Write(Client.instance.myId);
			_packet.Write(UIManager.instance.usernameField.text);

			SendTCPData(_packet);
		}
	}

	/// <summary>Sends player input to the server.</summary>
	/// <param name="_inputs"></param>
	public static void PlayerMovement(bool[] _inputs)
	{
		using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
		{
			_packet.Write(_inputs.Length);
			foreach (bool _input in _inputs)
			{
				_packet.Write(_input);
			}
			_packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

			SendUDPData(_packet);
		}
	}

	public static void Kill()
	{
		using (Packet _packet = new Packet((int)ClientPackets.kill))
		{
			_packet.Write(Client.instance.myId);

			SendTCPData(_packet);
		}
	}

	public static void Report()
	{
		using (Packet _packet = new Packet((int)ClientPackets.report))
		{
			_packet.Write(Client.instance.myId);

			SendTCPData(_packet);
		}
	}

	public static void Vote(int _playerVoted)
	{
		using (Packet _packet = new Packet((int)ClientPackets.vote))
		{
			_packet.Write(Client.instance.myId);
			_packet.Write(_playerVoted);

			SendTCPData(_packet);
		}
	}

	public static void Interact()
	{
		using (Packet _packet = new Packet((int)ClientPackets.interact))
		{
			_packet.Write(Client.instance.myId);

			SendTCPData(_packet);
		}
	}

	public static void TaskDone(TaskType _taskType)
	{
		using (Packet _packet = new Packet((int)ClientPackets.taskDone))
		{
			_packet.Write(Client.instance.myId);
			_packet.Write((int)_taskType);

			SendTCPData(_packet);
		}
	}

	public static void Vent()
	{
		using (Packet _packet = new Packet((int)ClientPackets.vent))
		{
			_packet.Write(Client.instance.myId);

			SendTCPData(_packet);
		}
	}

	public static void GoToVent(int _goToVentId)
	{
		using (Packet _packet = new Packet((int)ClientPackets.goToVent))
		{
			_packet.Write(Client.instance.myId);
			_packet.Write(_goToVentId);

			SendTCPData(_packet);
		}
	}
	#endregion
}
