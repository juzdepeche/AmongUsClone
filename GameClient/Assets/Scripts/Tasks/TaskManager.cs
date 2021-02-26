using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public static TaskManager instance;

	public TaskUI[] tasksUI;
	public static Dictionary<int, Task> tasks = new Dictionary<int, Task>();

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		KeypadTask.OnTaskDone += () => TaskDone(TaskType.Keypad);
		SwipeTask.OnTaskDone += () => TaskDone(TaskType.CardSwipe);
		WireTask.OnTaskDone += () => TaskDone(TaskType.Wire);
		UnlockManifoldsTask.OnTaskDone += () => TaskDone(TaskType.Numbers);
		StartReactorTask.OnTaskDone += () => TaskDone(TaskType.StartReactor);
	}

	public void DoTask(TaskType _taskType)
	{
		tasksUI.Where(t => t.taskType == _taskType).First().taskCanvas.SetActive(true);
	}

	public void LeaveTask(TaskType _taskType)
	{
		PlayerHelper.GetPlayerThisPlayer().taskIsDoingId = -1;
		tasksUI.Where(t => t.taskType == _taskType).First().taskCanvas.SetActive(false);
	}

	public void LeaveTask(int _taskId, TaskType _taskType)
	{
		if (PlayerHelper.GetPlayerThisPlayer().taskIsDoingId == _taskId)
		{
			PlayerHelper.GetPlayerThisPlayer().taskIsDoingId = -1;
			tasksUI.Where(t => t.taskType == _taskType).First().taskCanvas.SetActive(false);
		}
	}

	private void TaskDone(TaskType _taskType)
	{
		ClientSend.TaskDone(_taskType);
		// LeaveTask(_taskType);
	}

	public void NewTask(int _id, bool _done, Vector3 _position, TaskType _taskType)
	{
		tasks.Add(_id, new Task(_id, _done, _position, _taskType));
	}

	[Serializable]
	public struct TaskUI
	{
		public TaskType taskType;
		public GameObject taskCanvas;
	}

	internal void TaskIsReallyDone(int _id)
	{
		tasks[_id].done = true;
	}
}
