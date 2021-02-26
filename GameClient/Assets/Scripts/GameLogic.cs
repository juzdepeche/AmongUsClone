using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public static GameLogic instance;
	public int tick;
	public float tickTimer;

	private void Awake()
	{
		instance = this;
		tick = 0;
		tickTimer = 0;
	}
}
