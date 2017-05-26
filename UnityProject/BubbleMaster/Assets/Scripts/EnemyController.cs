using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public float speed;
	public Vector2 direction;

	Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{

		if (other.CompareTag ("PlatformEnd")) {
			Flip ();
		}

		if (other.CompareTag("Bubble")) {
			Destroy (this.gameObject);
		}



	}

	void Flip()
	{
		direction = -direction;

		Vector2 newScale = transform.localScale;
		newScale.x = newScale.x * -1;

		transform.localScale = newScale;

	}


	void FixedUpdate () {

		rb2d.velocity = new Vector2 (direction.x * speed, rb2d.velocity.y); 
		
	}
}
