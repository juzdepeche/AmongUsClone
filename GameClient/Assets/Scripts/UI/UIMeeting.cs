using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMeeting : MonoBehaviour
{
	public static UIMeeting instance;
	public List<Button> voteButtons = new List<Button>();
	public GameObject voteMenu;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		HideUI();
	}

	public void StartMeeting(int _reporterId)
	{
		ShowUI();
		PrepareButtons(_reporterId);
	}

	public void EndMeeting()
	{
		HideUI();
	}

	private void PrepareButtons(int _reporterId)
	{
		EnableAllButtons();
		int buttonIndex = 0;

		foreach (PlayerManager _player in PlayerHelper.GetAllAlivePlayers())
		{
			if (_player == null) continue;

			Color textColor = _reporterId == _player.id ? Color.blue : Color.black;
			voteButtons[buttonIndex].GetComponent<VoteButton>().Initialize(_player.id, _player.username, true, textColor);

			buttonIndex++;
		}

		foreach (PlayerManager _player in PlayerHelper.GetAllDeadPlayers())
		{
			if (_player == null) continue;

			voteButtons[buttonIndex].GetComponent<VoteButton>().Initialize(_player.id, _player.username, false, Color.black);

			buttonIndex++;
		}

		for (int i = buttonIndex; i < voteButtons.Count; i++)
		{
			voteButtons[i].GetComponent<VoteButton>().Initialize(-1, string.Empty, false, Color.black);
		}
	}

	public void OnVoteButtonClicked(int _voteButtonIndex)
	{
		int playerVoted = voteButtons[_voteButtonIndex].GetComponent<VoteButton>().id;
		ClientSend.Vote(playerVoted);
	}

	public void PrivateVoteSuccess(bool _isSuccess, int _fromId, int _toId)
	{
		if (_isSuccess)
		{
			DisableAllButtons();

			var _myVoteButton = GetVoteButtonFromPlayerId(_fromId);
			_myVoteButton.GetComponent<VoteButton>().Disable();
			_myVoteButton.GetComponent<Image>().color = Color.green;

			var _votedForButton = GetVoteButtonFromPlayerId(_toId);
			_votedForButton.GetComponent<VoteButton>().SetColor(Color.red);
		}
		else
		{
			string _username = PlayerHelper.GetAllPlayers()[_toId].username;
			Debug.LogError($"There was an error while trying to vote out {_username}");
		}
	}

	public void PublicVoteSuccess(bool _isSuccess, int _fromId)
	{
		if (_isSuccess)
		{
			var _voteButtonToDisable = GetVoteButtonFromPlayerId(_fromId);
			_voteButtonToDisable.GetComponent<Image>().color = Color.green;
		}
		else
		{
			Debug.LogError($"There was an error while trying to vote");
		}
	}

	public void VoteResults()
	{
		HideUI();
	}

	private Button GetVoteButtonFromPlayerId(int _playerId)
	{
		return voteButtons.Where(vb => vb.GetComponent<VoteButton>().id == _playerId).First();
	}

	private void DisableAllButtons()
	{
		foreach (Button voteButton in voteButtons)
		{
			voteButton.GetComponent<VoteButton>().Disable();
		}
	}

	private void EnableAllButtons()
	{
		foreach (Button voteButton in voteButtons)
		{
			voteButton.GetComponent<Image>().color = Color.white;
			voteButton.GetComponent<VoteButton>().Enable();
		}
	}

	private void ShowUI()
	{
		voteMenu.SetActive(true);
	}

	private void HideUI()
	{
		voteMenu.SetActive(false);
	}
}
