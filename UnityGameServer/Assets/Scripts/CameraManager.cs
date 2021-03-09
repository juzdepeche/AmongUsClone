using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	[Serializable]
	public struct CameraTarget
	{
		public GameObject camera;
		public GameObject cameraTarget;
		public CameraTarget(GameObject _camera, GameObject _cameraTarget)
		{
			camera = _camera;
			cameraTarget = _cameraTarget;
		}
	}

	public string roomId;
	public List<CameraTarget> cameras = new List<CameraTarget>();
	public GameObject cameraStation;

	public void SendCameras()
	{
		ServerSend.CameraPositions(roomId, cameras);
	}
}
