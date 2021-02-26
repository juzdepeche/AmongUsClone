using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoteRegister : MonoBehaviour
{
	private static Dictionary<int, bool> playerWhoVoted;
	private static Dictionary<int, int> registeredVotes;

	public static void ResetVotes()
	{
		playerWhoVoted = new Dictionary<int, bool>();
		registeredVotes = new Dictionary<int, int>();

		foreach (Player _player in PlayerHelper.GetAllAlivePlayers())
		{
			playerWhoVoted.Add(_player.id, false);
			registeredVotes.Add(_player.id, 0);
		}
	}

	public static void RegisterVote(int _fromId, int _toId)
	{
		Player _playerWhoVoted = PlayerHelper.GetPlayerById(_fromId);
		Player _playerGettingVoted = PlayerHelper.GetPlayerById(_toId);

		if (
			_playerWhoVoted == null ||
			_playerGettingVoted == null ||
			_playerWhoVoted.dead ||
			playerWhoVoted[_fromId] ||
			_playerGettingVoted.dead
		)
		{
			ServerSend.PrivateVoteStatus(false, _fromId, _toId);
		}
		else
		{
			playerWhoVoted[_fromId] = true;
			registeredVotes[_toId]++;

			ServerSend.PrivateVoteStatus(true, _fromId, _toId);
			ServerSend.PublicVoteStatus(true, _fromId);

			if (playerWhoVoted.All(p => p.Value))
			{
				CountVotes();
			}
		}
	}

	private static void CountVotes()
	{
		var first = registeredVotes.OrderByDescending(rv => rv.Value).ElementAt(0);
		var second = registeredVotes.OrderByDescending(rv => rv.Value).ElementAt(1);

		//tie
		if (first.Value == second.Value)
		{
			ServerSend.VoteResults(-1);
		}
		else
		{
			int _votedOutPlayerId = first.Key;
			ServerSend.VoteResults(_votedOutPlayerId);
			PlayerHelper.GetPlayerById(_votedOutPlayerId).Die(false);
		}

		GameLogic.instance.OnMeetingEnd();
	}
}
