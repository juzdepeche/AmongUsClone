using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public static GameLogic instance;
	public static Dictionary<int, Vector3> deadBodyPosition = new Dictionary<int, Vector3>();
	public List<GameObject> taskObjects = new List<GameObject>();
	public static Dictionary<int, Task> tasks = new Dictionary<int, Task>();
	public List<Transform> spawnPoints = new List<Transform>();
	public List<GameObject> vents = new List<GameObject>();
	public static float interactRadius = 3f;
	public bool inMeeting = false;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		CreateTasks(5);
	}

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

	public List<Task> GetTasks()
	{
		// todo optmize parent loop
		var notDoneTasks = tasks.Where(t => !t.Value.done).Select(t => t.Value).ToList();

		return notDoneTasks;
	}

	public void CallMeeting(int _reporterId)
	{
		VoteRegister.ResetVotes();
		inMeeting = true;
		MoveAllPlayersToTheMeetingArea();
		ServerSend.StartMeeting(_reporterId);
		deadBodyPosition = new Dictionary<int, Vector3>();
	}

	public void OnMeetingEnd()
	{
		StartCoroutine(ShowVoteResults(4f));
	}

	private IEnumerator ShowVoteResults(float _time)
	{

		yield return new WaitForSeconds(_time);

		inMeeting = false;
		ServerSend.RemoveVoteResults();
	}

	private void MoveAllPlayersToTheMeetingArea()
	{
		var players = PlayerHelper.GetAllPlayers();
		var playersSeats = PlayerHelper.GetRandomSeatsForEveryPlayers();

		int seatIndex = 0;
		foreach (Player _player in players)
		{
			_player.transform.position = playersSeats[seatIndex];
			ServerSend.TeleportPlayer(_player.id, playersSeats[seatIndex]);
			seatIndex++;
		}
	}
}
