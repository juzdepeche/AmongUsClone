using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
	public static UIControl instance;
	public Button reportButton;
	public Button killButton;
	public Button useButton;
	public Button ventButton;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		//todo: rendre invisible
		// killButton.enabled = GameManager.players[Client.instance.myId].role == Role.Imposter;
	}

	public void onReportButtonPressed()
	{
		ClientSend.Report();
	}

	public void onKillButtonPressed()
	{
		ClientSend.Kill();
	}

	public void onUseButtonPressed()
	{
		ClientSend.Interact();
	}

	public void onVentButtonPressed()
	{
		ClientSend.Vent();
	}

	public void setKillButtonInteractable(bool _isInteractable)
	{
		killButton.interactable = _isInteractable;
	}

	public void setUseButtonInteractable(bool _isInteractable)
	{
		useButton.interactable = _isInteractable;
	}

	public void setReportButtonInteractable(bool _isInteractable)
	{
		reportButton.interactable = _isInteractable;
	}

	public void setVentButtonInteractable(bool _isInteractable)
	{
		ventButton.interactable = _isInteractable;
	}
}
