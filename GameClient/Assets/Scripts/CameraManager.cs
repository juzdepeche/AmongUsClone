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
	public List<CameraTarget> camerasPositions;
	public GameObject securityCameraPrefab;
	public GameObject cameraPrefab;
	private List<GameObject> cameras;
	//camera object
	private GameObject cameraCameraPosition;
	private int currentCameraIndex = 0;

	private void Awake()
	{
		instance = this;
		cameras = new List<GameObject>();
	}

	private void Start()
	{
		NextCameraButton.OnButtonClicked += NextCamera;
	}

	public void SetCameras(List<CameraTarget> _cameras)
	{
		camerasPositions = _cameras;
		SpawnSecurityCameras();
		InitializeFirstCamera();
	}

	private void SpawnSecurityCameras()
	{
		int _cameraId = 0;

		foreach (CameraTarget _camera in camerasPositions)
		{
			var _newCamera = Instantiate(securityCameraPrefab, _camera.camera, Quaternion.identity);
			string _cameraName = "camera" + _cameraId;
			_newCamera.name = _cameraName;

			cameras.Add(_newCamera);
			_cameraId++;
		}
	}

	private void InitializeFirstCamera()
	{
		cameraCameraPosition = Instantiate(cameraPrefab, camerasPositions[0].cameraTarget, Quaternion.identity);
	}

	public void NextCamera()
	{
		currentCameraIndex++;
		if (currentCameraIndex >= camerasPositions.Count)
		{
			currentCameraIndex = 0;
		}

		cameraCameraPosition.transform.position = camerasPositions[currentCameraIndex].cameraTarget;
	}
}
