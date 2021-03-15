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
	public Button startButton;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		HideUI();
	}

	public void ShowUI()
	{
		SetVentButtonVisibility(true);
		SetKillButtonVisibility(true);
		SetUseButtonVisibility(true);
		SetReportButtonVisibility(true);
	}

	public void HideUI()
	{
		SetStartButtonVisibility(false);

		SetVentButtonVisibility(false);
		SetKillButtonVisibility(false);
		SetUseButtonVisibility(false);
		SetReportButtonVisibility(false);
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

	public void onStartButtonPressed()
	{
		ClientSend.StartGame();
		SetStartButtonVisibility(false);
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

	public void SetStartButtonVisibility(bool _isVisible)
	{
		startButton.gameObject.SetActive(_isVisible);
	}

	public void SetKillButtonVisibility(bool _isVisible)
	{
		_isVisible = _isVisible && PlayerHelper.GetThisPlayer().role == Role.Imposter;
		killButton.gameObject.SetActive(_isVisible);
	}

	public void SetVentButtonVisibility(bool _isVisible)
	{
		_isVisible = _isVisible && PlayerHelper.GetThisPlayer().role == Role.Imposter;
		ventButton.gameObject.SetActive(_isVisible);
	}

	public void SetUseButtonVisibility(bool _isVisible)
	{
		useButton.gameObject.SetActive(_isVisible);
	}

	public void SetReportButtonVisibility(bool _isVisible)
	{
		reportButton.gameObject.SetActive(_isVisible);
	}
}
