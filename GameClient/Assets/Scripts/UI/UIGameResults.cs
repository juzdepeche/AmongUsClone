using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameResults : MonoBehaviour
{
	public static UIGameResults instance;
	public GameObject menu;
	public Text winnersText;

	private void Awake()
	{
		instance = this;
		HideUI();
	}

	public void HideUI()
	{
		menu.SetActive(false);
	}

	public void ShowUI(Role _winners)
	{
		menu.SetActive(true);
		SetText(_winners);
	}

	private void SetText(Role _winners)
	{
		string _winnersText = _winners == Role.Imposter ? "Imposters" : "Crewmates";
		winnersText.text = $"{_winnersText} win.";
	}
}
