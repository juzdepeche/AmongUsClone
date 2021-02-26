using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartReactorTask : MonoBehaviour
{
	public static event Action OnTaskDone;
	[SerializeField] private Color _activeColor = Color.yellow;
	[SerializeField] private float _showColorTimeInSeconds = 0.5f;

	[SerializeField] private List<StartReactorButton> _buttonList = new List<StartReactorButton>();
	[SerializeField] private List<Image> _animatedImagesList = new List<Image>();
	[SerializeField] private List<Image> _taskProgressImageList = new List<Image>();

	private int _currentSequenceIndex = 1;
	private int _currentStep;

	private List<int> _generatedSequence;
	private bool _wasSequenceGenerated = false;

	private void OnEnable()
	{
		if (_wasSequenceGenerated == false)
		{
			_generatedSequence = new List<int>();
			List<int> uniqueImageIndexList = new List<int>();

			for (int i = 0; i < _animatedImagesList.Count; i++) { uniqueImageIndexList.Add(i); }

			// generate unique sequence
			for (int i = 0; i < _animatedImagesList.Count; i++)
			{
				int selectedIndex = uniqueImageIndexList[UnityEngine.Random.Range(0, uniqueImageIndexList.Count)];
				uniqueImageIndexList.Remove(selectedIndex);
				_generatedSequence.Add(selectedIndex);
			}

			for (int i = 0; i < _buttonList.Count; i++)
			{
				_buttonList[i].Initialize(i, this);
			}

			// reset all progress images
			foreach (Image img in _taskProgressImageList) { img.color = Color.white; }

			_currentSequenceIndex = 1;
		}

		// play the sequence
		StartCoroutine(PlaySequence());
	}

	private void ToggleButtons(bool isActive)
	{
		foreach (StartReactorButton button in _buttonList)
		{
			button.Toggle(isActive);
		}
	}

	private IEnumerator PlaySequence()
	{
		// disable all the buttons
		ToggleButtons(false);

		for (int i = 0; i < _currentSequenceIndex; i++)
		{
			_animatedImagesList[_generatedSequence[i]].color = _activeColor;
			yield return new WaitForSeconds(_showColorTimeInSeconds);
			_animatedImagesList[_generatedSequence[i]].color = Color.white;
		}

		// enable buttons
		ToggleButtons(true);
	}

	private IEnumerator ShowError()
	{
		ToggleButtons(false);

		for (int i = 0; i < 3; i++)
		{
			foreach (Image img in _animatedImagesList) { img.color = Color.red; }
			yield return new WaitForSeconds(0.5f);
			foreach (Image img in _animatedImagesList) { img.color = Color.white; }
		}

		StartCoroutine(PlaySequence());
	}

	public void OnButtonPressed(int index)
	{
		// check if the correct button was pressed
		if (_generatedSequence[_currentStep] == index)
		{
			_currentStep++;

			// check if we can move to the next in sequence
			if (_currentStep >= _currentSequenceIndex)
			{
				_taskProgressImageList[_currentSequenceIndex - 1].color = Color.green;
				_currentSequenceIndex++;
				_currentStep = 0;

				if (_currentSequenceIndex <= _taskProgressImageList.Count)
				{
					StartCoroutine(PlaySequence());
				}
			}
		}
		else
		{
			_currentStep = 0;
			StartCoroutine(ShowError());
		}

		// check finality
		if (_currentSequenceIndex > _taskProgressImageList.Count)
		{
			// task completed
			_wasSequenceGenerated = false;
			OnTaskDone?.Invoke();
		}
	}
}