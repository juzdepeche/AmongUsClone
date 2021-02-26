using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManifoldsTask : MonoBehaviour
{
	public static event Action OnTaskDone;
	[SerializeField] private List<UnlockManifoldsButton> _buttonList = new List<UnlockManifoldsButton>();

	private int currentValue;
	private void OnEnable()
	{
		List<int> numbers = new List<int>();

		for (int i = 0; i < _buttonList.Count; i++)
		{
			numbers.Add(i + 1);
		}

		for (int i = 0; i < _buttonList.Count; i++)
		{
			int pickedNumber = numbers[UnityEngine.Random.Range(0, numbers.Count)];
			_buttonList[i].Initialize(pickedNumber, this);
			numbers.Remove(pickedNumber);
		}
		ResetButtons();

		currentValue = 1;
	}

	private void ResetButtons()
	{
		foreach (UnlockManifoldsButton button in _buttonList)
		{
			button.ToggleButton(true);
		}
	}

	public void OnButtonPressed(int buttonID, UnlockManifoldsButton currentButton)
	{
		if (currentValue >= _buttonList.Count)
		{
			// task completed
			Debug.Log("[Unlock Manifolds Task Succeeded!]");
			OnTaskDone?.Invoke();
		}

		// Check if the correct button
		if (currentValue == buttonID)
		{
			currentValue++;
			currentButton.ToggleButton(false);
		}
		else
		{
			// wrong button.
			currentValue = 1;
			ResetButtons();
			Debug.Log("[Unlock Manifolds Task FAILED!]");
		}
	}

}