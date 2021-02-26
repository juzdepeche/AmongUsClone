using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHelper : MonoBehaviour
{
	// public static PlayerHelper instance;

	// private void Awake()
	// {
	// 	instance = this;
	// }

	public static List<Player> GetAllPlayers()
	{
		List<Player> players = new List<Player>();

		foreach (Client _client in Server.clients.Values)
		{
			if (_client.player != null)
			{
				players.Add(_client.player);
			}
		}

		return players;
	}

	public static List<Player> GetAllAlivePlayers()
	{
		List<Player> players = new List<Player>();

		foreach (Client _client in Server.clients.Values)
		{
			if (_client.player != null)
			{
				if (!_client.player.dead) players.Add(_client.player);
			}
		}

		return players;
	}

	public static List<Player> GetAllDeadPlayers()
	{
		List<Player> players = new List<Player>();

		foreach (Client _client in Server.clients.Values)
		{
			if (_client.player != null)
			{
				if (_client.player.dead) players.Add(_client.player);
			}
		}

		return players;
	}

	public static Player GetPlayerById(int _id)
	{
		return GetAllPlayers().Where(p => p.id == _id).First();
	}

	public static List<Vector3> GetRandomSeatsForEveryPlayers()
	{
		//todo: optimize by returning a shuffled list
		List<Vector3> seatsPositions = new List<Vector3>();

		var players = PlayerHelper.GetAllPlayers();
		int playersToPlayerCount = players.Count;

		var seats = GameLogic.instance.spawnPoints;
		bool[] takenSeats = new bool[seats.Count];

		while (playersToPlayerCount > 0)
		{
			int randomSeatIndex = Random.Range(0, seats.Count);

			if (!takenSeats[randomSeatIndex])
			{
				seatsPositions.Add(seats[randomSeatIndex].position);
				takenSeats[randomSeatIndex] = true;
				playersToPlayerCount--;
			}
		}

		return seatsPositions;
	}

	public static Vector3 GetRandomSpawnPosition()
	{
		var spawnPoints = GameLogic.instance.spawnPoints;
		return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
	}
}
