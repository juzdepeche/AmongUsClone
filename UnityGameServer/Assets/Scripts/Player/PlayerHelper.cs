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

	public static List<Player> GetAllPlayers(string _roomId)
	{
		List<Player> players = new List<Player>();

		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			if (_client.player != null)
			{
				players.Add(_client.player);
			}
		}

		return players;
	}

	public static List<Player> GetAllAlivePlayers(string _roomId)
	{
		List<Player> players = new List<Player>();

		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			if (_client.player != null)
			{
				if (!_client.player.dead) players.Add(_client.player);
			}
		}

		return players;
	}

	public static List<Player> GetAllDeadPlayers(string _roomId)
	{
		List<Player> players = new List<Player>();

		foreach (Client _client in Server.rooms[_roomId].clients.Values)
		{
			if (_client.player != null)
			{
				if (_client.player.dead) players.Add(_client.player);
			}
		}

		return players;
	}

	public static List<Player> GetAlivePlayersByRole(string _roomId, Role _role)
	{
		//todo optimize parent loop
		return Server.rooms[_roomId].clients.Values.Select(c => c.player).Where(p => !p.dead && p.role == _role).ToList();
	}

	public static Player GetPlayerById(int _id)
	{
		return GetAllPlayers().Where(p => p.id == _id).First();
	}

	public static string GetPlayerRoomId(int _playerId)
	{
		return GetAllPlayers().Where(p => p.id == _playerId).First().roomId;
	}

	public static List<Vector3> GetRandomSeatsForEveryPlayers(string _roomId)
	{
		//todo: optimize by returning a shuffled list
		List<Vector3> seatsPositions = new List<Vector3>();

		var players = PlayerHelper.GetAllPlayers(_roomId);
		int playersToPlayerCount = players.Count;

		GameLogic roomGameManager = RoomManager.GetRoomGameManager(_roomId);

		var seats = roomGameManager.spawnPoints;
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

	public static Vector3 GetRandomSpawnPosition(string _roomId)
	{
		GameLogic roomGameManager = RoomManager.GetRoomGameManager(_roomId);
		var spawnPoints = roomGameManager.spawnPoints;
		return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
	}

	public static Vector3 GetRandomPreGameLobbySpawnPosition(string _roomId)
	{
		GameLogic roomGameManager = RoomManager.GetRoomGameManager(_roomId);
		var spawnPoints = roomGameManager.preGameLobbySpawnPoints;
		return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
	}
}
