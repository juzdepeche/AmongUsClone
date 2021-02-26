using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public GameObject startMenu;
	public InputField usernameField;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Debug.Log("Instance already exists, destroying object!");
			Destroy(this);
		}
		startMenu.SetActive(true);
	}

	private void Start()
	{
		StartCoroutine(AutoConnectToServer());
	}

	private IEnumerator AutoConnectToServer()
	{
		yield return new WaitForSeconds(5f);
		string username = BuildHelper.GetArg(BuildParameter.Username);
		if (username != null)
		{
			usernameField.text = username;
			ConnectToServer();
		}
	}

	/// <summary>Attempts to connect to the server.</summary>
	public void ConnectToServer()
	{
		startMenu.SetActive(false);
		usernameField.interactable = false;
		Client.instance.ConnectToServer();
	}
}
