﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour {

	public int force;									// the force that's been applied to collectable in spawn collectable
	public Sprite[] sprites;							// sprite array of all the possible collectable sprites

	Rigidbody2D rb2d;
	BoxCollider2D boxCollider2d;

	float screenTopY;
	float screenBottomY;

	// Use this for initialization
	void Start () {

		screenTopY = Camera.main.ViewportToWorldPoint (new Vector2 (0, 1)).y;
		screenBottomY = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0)).y;

		rb2d = GetComponent<Rigidbody2D> ();
		boxCollider2d = GetComponent<BoxCollider2D> ();

		int randomIndex = Random.Range (0, sprites.Length);
		GetComponent<SpriteRenderer>().sprite = sprites[randomIndex];

		StartCoroutine (SpawnCollectable ());

	}

	void Update()
	{
		// Check if collectable is offscreen bring it back to the screen other side
		if (transform.position.y > screenTopY) {

			transform.position = new Vector2 (transform.position.x, screenBottomY);
		}
	}

	IEnumerator SpawnCollectable()
	{
		boxCollider2d.isTrigger = false;
		rb2d.AddForce (Vector2.up * force, ForceMode2D.Impulse);

		yield return new WaitForSeconds (0.1f);

		boxCollider2d.isTrigger = true;

		yield return new WaitForSeconds (0.5f);

		rb2d.gravityScale = 0;
		rb2d.velocity = new Vector2 (0, 0);


	}

}
