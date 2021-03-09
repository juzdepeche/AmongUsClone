using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public string roomId;
	public Dictionary<int, Task> tasks = new Dictionary<int, Task>();
	public List<GameObject> taskObjects = new List<GameObject>();

	public void CreateTasks(int taskNumber)
	{
		tasks = new Dictionary<int, Task>();
		for (int i = 0; i < taskNumber && i < taskObjects.Count; i++)
		{
			int randomIndex = UnityEngine.Random.Range(0, taskObjects.Count);
			GameObject taskObject = taskObjects[randomIndex];
			taskObjects.RemoveAt(randomIndex);
			Task _task = new Task(i, taskObject, false, (TaskType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(TaskType)).Length));
			tasks.Add(i, _task);
		}
	}

	public void SendTasks()
	{
		var _players = PlayerHelper.GetAllPlayers(roomId);
		foreach (Player _player in _players)
		{
			foreach (Task _task in tasks.Values)
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

	public void TaskDone(int _playerId, int _fromClient)
	{
		tasks[_playerId].TaskDone(_fromClient);
	}

	public float GetTasksCompletedPercentage()
	{
		float completedTasksCount = tasks.Where(t => t.Value.done).Count();
		return completedTasksCount / (float)tasks.Count();
	}
}
