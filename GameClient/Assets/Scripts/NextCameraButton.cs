using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextCameraButton : MonoBehaviour
{
	public static event Action OnButtonClicked;

	private void OnMouseDown()
	{
		OnButtonClicked?.Invoke();
	}
}
