using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
	public static List<PlayerManager> GetAllPlayers()
	{
		List<PlayerManager> players = new List<PlayerManager>();

		foreach (PlayerManager _player in GameManager.players.Values)
		{
			if (_player != null)
			{
				players.Add(_player);
			}
		}

		return players;
	}

	public static List<PlayerManager> GetAllAlivePlayers()
	{
		List<PlayerManager> players = new List<PlayerManager>();

		foreach (PlayerManager _player in GameManager.players.Values)
		{
			if (_player != null)
			{
				if (!_player.dead) players.Add(_player);
			}
		}

		return players;
	}

	public static List<PlayerManager> GetAllDeadPlayers()
	{
		List<PlayerManager> players = new List<PlayerManager>();

		foreach (PlayerManager _player in GameManager.players.Values)
		{
			if (_player != null)
			{
				if (_player.dead) players.Add(_player);
			}
		}

		return players;
	}

	public static PlayerManager GetPlayerById(int _id)
	{
		return GetAllPlayers().Where(p => p.id == _id).First();
	}

	public static PlayerManager GetThisPlayer()
	{
		return GetPlayerById(Client.instance.myId);
	}

}
