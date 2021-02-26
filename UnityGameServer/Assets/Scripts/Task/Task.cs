using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
	public int id;
	public GameObject taskObject;
	public Vector3 position;
	public bool done;
	public TaskType taskType;
	public int doneByPlayerId = -1;

	public Task(int _id, GameObject _taskObject, bool _done, TaskType _taskType)
	{
		id = _id;
		taskObject = _taskObject;
		position = _taskObject.transform.position;
		done = _done;
		taskType = _taskType;

		taskObject.SetActive(true);
	}
}
