using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoteButton : MonoBehaviour
{
	public int id;
	public string username;
	public bool isInteractable;
	public Color color;

	public void Initialize(int _id, string _username, bool _isInteractable, Color _color)
	{
		id = _id;
		username = _username;
		GetComponent<Button>().interactable = _isInteractable;
		GetComponentInChildren<TextMeshProUGUI>().text = _username;
		GetComponentInChildren<TextMeshProUGUI>().color = _color;
	}

	public void Enable()
	{
		GetComponent<Button>().interactable = true;
	}

	public void Disable()
	{
		GetComponent<Button>().interactable = false;
	}

	public void SetColor(Color _color)
	{
		GetComponentInChildren<TextMeshProUGUI>().color = _color;
	}
}
