using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoleShowcase : MonoBehaviour
{
	public static UIRoleShowcase instance;
	public GameObject menu;
	public Text imposterCountText;
	public Text roleText;

	private void Awake()
	{
		instance = this;
		HideUI();
	}

	public void HideUI()
	{
		menu.SetActive(false);
	}

	public void ShowUI(int _imposterCount)
	{
		menu.SetActive(true);
		SetImposterCountText(_imposterCount);
		SetPlayerRole(PlayerHelper.GetThisPlayer().role);
	}

	private void SetImposterCountText(int _imposterCount)
	{
		string _verb = _imposterCount > 1 ? "are" : "is";
		string _plural = _imposterCount > 1 ? "imposters" : "imposter";
		imposterCountText.text = $"There {_verb} {_imposterCount} {_plural} among us.";
	}

	private void SetPlayerRole(Role _role)
	{
		roleText.text = _role == Role.Imposter ? "Imposter" : "Crewmate";
	}
}
