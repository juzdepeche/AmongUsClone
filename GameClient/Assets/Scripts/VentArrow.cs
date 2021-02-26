using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentArrow : MonoBehaviour
{
	public int toVentId;
	public int fromVentId;

	public void Initialize(int _toVentId)
	{
		toVentId = _toVentId;
	}

	private void OnMouseDown()
	{
		ClientSend.GoToVent(toVentId);
	}
}
