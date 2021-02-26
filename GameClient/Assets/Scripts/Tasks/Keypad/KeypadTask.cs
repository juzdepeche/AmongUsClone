using System;
using UnityEngine;
using UnityEngine.UI;

public class KeypadTask : MonoBehaviour
{
	public Text _cardCode;
	public Text _inputCode;
	public int _codeLength = 5;
	public static event Action OnTaskDone;

	private void OnEnable()
	{
		string code = string.Empty;

		for (int i = 0; i < _codeLength; i++)
		{
			code += UnityEngine.Random.Range(1, 10);
		}

		_cardCode.text = code;
		_inputCode.text = string.Empty;
	}

	public void ButtonClick(int number)
	{
		_inputCode.text += number;

		if (_inputCode.text == _cardCode.text)
		{
			_inputCode.text = "Correct";
			OnTaskDone?.Invoke();
		}
		else if (_inputCode.text.Length >= _codeLength)
		{
			_inputCode.text = "Failed";
		}
	}
}