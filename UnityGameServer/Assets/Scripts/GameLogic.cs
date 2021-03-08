using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public string roomId;
	public bool hasGameStarted = false;
	public static Dictionary<int, Vector3> deadBodyPosition = new Dictionary<int, Vector3>();
	public List<GameObject> taskObjects = new List<GameObject>();
	public static Dictionary<int, Task> tasks = new Dictionary<int, Task>();
	public List<Transform> spawnPoints = new List<Transform>();
	public List<Transform> preGameLobbySpawnPoints = new List<Transform>();
	public List<GameObject> vents = new List<GameObject>();
	public static float interactRadius = 3f;
	public bool inMeeting = false;

	private void CreateTasks(int taskNumber)
	{
		for (int i = 0; i < taskNumber || i <= taskObjects.Count; i++)
		{
			int randomIndex = UnityEngine.Random.Range(0, taskObjects.Count);
			GameObject taskObject = taskObjects[randomIndex];
			taskObjects.RemoveAt(randomIndex);
			Task _task = new Task(i, taskObject, false, (TaskType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(TaskType)).Length));
			tasks.Add(i, _task);
		}
	}

	private void SendTasks()
	{
		var _players = PlayerHelper.GetAllPlayers(roomId);
		foreach (Player _player in _players)
		{
			foreach (Task _task in GameLogic.tasks.Values)
			{
				ServerSend.NewTask(_player.id, _task);
			}
		}
	}

	public List<Task> GetTasks()
	{
		// todo optmize parent loop
		var notDoneTasks = tasks.Where(t => !t.Value.done).Select(t => t.Value).ToList();

		return notDoneTasks;
	}

	public void CallMeeting(int _reporterId)
	{
		RoomManager.GetRoomVoteRegister(roomId).ResetVotes();
		inMeeting = true;
		MoveAllPlayersToTheMeetingArea();
		ServerSend.StartMeeting(_reporterId);
		deadBodyPosition = new Dictionary<int, Vector3>();
	}

	public void OnMeetingEnd()
	{
		StartCoroutine(ShowVoteResults(4f));
	}

	public void StartGame()
	{
		hasGameStarted = true;
		AssignRoles();
		MoveAllPlayersToTheMeetingArea();
		tasks = new Dictionary<int, Task>();
		CreateTasks(5);
		SendTasks();
	}

	private void AssignRoles()
	{
		List<int> _playersIndex = new List<int>();
		var _players = PlayerHelper.GetAllPlayers(roomId);

		foreach (Player _player in _players)
		{
			_playersIndex.Add(_player.id);
		}

		//toto config imposter count
		int imposterCount = _players.Count > 5 ? 2 : 1;
		while (imposterCount > 0)
		{
			int _imposterId = _playersIndex[UnityEngine.Random.Range(0, _playersIndex.Count)];
			_playersIndex.Remove(_imposterId);

			PlayerHelper.GetPlayerById(_imposterId).role = Role.Imposter;
			ServerSend.PlayerRole(_imposterId, Role.Imposter);

			imposterCount--;
		}

		foreach (int _playerIndex in _playersIndex)
		{
			PlayerHelper.GetPlayerById(_playerIndex).role = Role.Crew;
			ServerSend.PlayerRole(_playerIndex, Role.Crew);
		}
	}

	private IEnumerator ShowVoteResults(float _time)
	{

		yield return new WaitForSeconds(_time);

		inMeeting = false;
		ServerSend.RemoveVoteResults(roomId);
	}

	private void MoveAllPlayersToTheMeetingArea()
	{
		var players = PlayerHelper.GetAllPlayers(roomId);
		var playersSeats = PlayerHelper.GetRandomSeatsForEveryPlayers(roomId);

		int seatIndex = 0;
		foreach (Player _player in players)
		{
			_player.transform.position = playersSeats[seatIndex];
			ServerSend.TeleportPlayer(_player.id, playersSeats[seatIndex]);
			seatIndex++;
		}
	}
}
