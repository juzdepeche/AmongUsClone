using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
	public SpriteRenderer colorSprite;

	public void SetColor(int _colorIndex)
	{
		colorSprite.color = GameManager.instance.playerColors[_colorIndex];
	}
}
