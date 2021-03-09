using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraTarget
{
	public Vector3 camera;
	public Vector3 cameraTarget;
	public CameraTarget(Vector3 _camera, Vector3 _cameraTarget)
	{
		camera = _camera;
		cameraTarget = _cameraTarget;
	}
}

public class CameraManager : MonoBehaviour
{
	public static CameraManager instance;
	public List<CameraTarget> cameras;
	public GameObject cameraPrefab;

	private void Awake()
	{
		instance = this;
	}

	public void SetCameras(List<CameraTarget> _cameras)
	{
		cameras = _cameras;
		SpawnCameras();
	}

	private void SpawnCameras()
	{
		int _cameraId = 0;

		foreach (CameraTarget _camera in cameras)
		{
			var _newCamera = Instantiate(cameraPrefab, _camera.camera, Quaternion.identity);
			string _cameraName = "camera" + _cameraId;
			_newCamera.name = _cameraName;
			GameObject.Find($"/{_cameraName}/Camera").transform.position = _camera.cameraTarget;

			_cameraId++;
		}
	}
}
