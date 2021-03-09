using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraStation : MonoBehaviour
{
	public static UICameraStation instance;
	public GameObject menu;

	private void Awake()
	{
		instance = this;
		HideUI();
	}

	public void ShowUI()
	{
		menu.SetActive(true);
	}

	public void HideUI()
	{
		menu.SetActive(false);
	}
}
