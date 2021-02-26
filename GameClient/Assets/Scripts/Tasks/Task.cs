using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
	public int id;
	public Vector3 position;
	public bool done;
	public TaskType taskType;

	public Task(int _id, bool _done, Vector3 _position, TaskType _taskType)
	{
		id = _id;
		done = _done;
		position = _position;
		taskType = _taskType;
	}
}
