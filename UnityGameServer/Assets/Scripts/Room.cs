using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
	public string id;
	public Dictionary<int, Client> clients = new Dictionary<int, Client>();
	public Vector3 position;

	public Room(string _id)
	{
		id = _id;
	}
}
