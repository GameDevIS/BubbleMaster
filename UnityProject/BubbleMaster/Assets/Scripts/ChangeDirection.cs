﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDirection : MonoBehaviour {

	public bool isVisible;							// is change direction sprite visible in play mode

	public Vector2 direction;

	void Awake()
	{
		GetComponent<SpriteRenderer> ().enabled = isVisible;
	}


}
