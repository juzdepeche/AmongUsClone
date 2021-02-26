using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public static CameraFollow instance;
	public Transform target;
	public float smoothTime = 0.2f;
	private Vector3 velocity = Vector3.zero;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (target == null) return;

		Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -10));
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
	}
}
