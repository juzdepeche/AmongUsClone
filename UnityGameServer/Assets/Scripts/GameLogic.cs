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
	public List<Transform> spawnPoints = new List<Transform>();
	public List<Transform> preGameLobbySpawnPoints = new List<Transform>();
	public List<GameObject> vents = new List<GameObject>();
	public static float interactRadius = 3f;
	public bool inMeeting = false;
	public bool gameOver = false;
	private int imposterCount = 0;
	private TaskManager taskManager;
	private CameraManager cameraManager;

	private void Start()
	{
		taskManager = RoomManager.GetRoomTaskManager(roomId);
		cameraManager = RoomManager.GetRoomCameraManager(roomId);
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
		gameOver = false;
		hasGameStarted = true;
		AssignRoles();
		ShowRoles();
		StartCoroutine(HideRoles());
		MoveAllPlayersToTheMeetingArea();

		//todo config task number
		taskManager.CreateTasks(3);
		taskManager.SendTasks();

		cameraManager.SendCameras();
	}

	private void Update()
	{
		if (!hasGameStarted) return;

		CheckCrewmateWinCondition();
		// CheckImposterWinCondition();
	}

	private void CheckCrewmateWinCondition()
	{
		if (gameOver) return;

		float perc = taskManager.GetTasksCompletedPercentage();
		if (perc >= 1f)
		{
			gameOver = true;
			ServerSend.GameOver(roomId, Role.Crew);
			Debug.LogWarning("Game over, crewmates win");
		}
	}

	private void CheckImposterWinCondition()
	{
		if (gameOver) return;

		int _imposterCount = PlayerHelper.GetAlivePlayersByRole(roomId, Role.Imposter).Count();
		int _crewmateCount = PlayerHelper.GetAlivePlayersByRole(roomId, Role.Crew).Count();
		if (_imposterCount >= _crewmateCount)
		{
			gameOver = true;
			ServerSend.GameOver(roomId, Role.Imposter);
			Debug.LogWarning("Game over, imposters win");
		}
	}

	private void ShowRoles()
	{
		ServerSend.ShowRoles(roomId, imposterCount);
	}

	private IEnumerator HideRoles()
	{
		yield return new WaitForSeconds(3f);

		ServerSend.HideRoles(roomId);
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
		imposterCount = _players.Count > 5 ? 2 : 1;
		int imposterCounter = imposterCount;
		while (imposterCounter > 0)
		{
			int _imposterId = _playersIndex[UnityEngine.Random.Range(0, _playersIndex.Count)];
			_playersIndex.Remove(_imposterId);

			PlayerHelper.GetPlayerById(_imposterId).role = Role.Imposter;
			ServerSend.PlayerRole(_imposterId, Role.Imposter);

			imposterCounter--;
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
