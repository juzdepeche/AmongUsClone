using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
	public int id;
	public GameObject[] possibleVents;

	private void OnDrawGizmos()
	{
		foreach (GameObject _vent in possibleVents)
		{
			Gizmos.DrawLine(transform.position, _vent.transform.position);
		}
	}
}
