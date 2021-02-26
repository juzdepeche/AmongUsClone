using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIVoteResults : MonoBehaviour
{
	public static UIVoteResults instance;
	public GameObject menu;

	private void Awake()
	{
		instance = this;
		HideUI();
	}

	public void HideUI()
	{
		menu.SetActive(false);
	}

	public void ShowUI(string _username)
	{
		menu.SetActive(true);
		SetText(_username);
	}

	private void SetText(string _username)
	{
		GetComponentInChildren<TextMeshProUGUI>().text = $"{_username} was ejected.";
	}
}
