using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {

	// Public Vars
	public float speed;								// Bubble Speed
	public float shootSpeed;						// Bubble Shoot Speed
	public float enemyCapturedSpeed;				// Bubble speed while enemy is captured

	public Vector2 direction;						// Bubble moving direction
	public float timeBetweenScale;					// The time between switching scale sizes
	public int floatingBubblesLayerID;				// Floating bubbles layer id to switch to after spawn
	public GameObject collectablePrefab;			// Collectable prefab

	public Sprite grayBubble;						// Bubble gray sprite
	public Sprite transparentBubble;				// Bubble transparent sprite
	public Sprite enemyTrapped;						// Bubble sprite while enemy is captured

	// Sounds
	public AudioClip enemyPopSound;					// Sound when bubble pop while enemy captured
	public AudioClip bubblePopSound;				// Sound while destroying empty bubble

	// Private Vars 
	float screenTopY;								// Screen upper most y position
	float screenBottomY;							// Screen lowest y position
	float bubbleYoffset;							// Bubble extents in the height so we can add that to the offset check
	bool isSpawnFinish;								// Is bubble spawn finished
	bool isEnemyCaptured;							// Is there enemy captured in bubble
	float originalSpeed;							// Orignal speed so we can switch back after bubble spawn
	Rigidbody2D rb2d;								// Ref to Rigidbody2D component
	Vector2 originalScale;							// Original scale used in the spawn bubble func
	AudioSource audioSource;						// Ref to Audio source componenet

	// Coroutines references
	Coroutine spawnBubbleRoutine;					// Ref to spawn bubble coroutine
	Coroutine destroyBubbleOverTimeRoutine;			// Ref to destroy bubble over time coroutine

	// Use this for initialization
	void Start () {

		isEnemyCaptured = false;
		originalSpeed = speed;
		originalScale = transform.localScale;
		audioSource = GetComponent<AudioSource> ();

		bubbleYoffset = GetComponent<SpriteRenderer> ().bounds.extents.y;

		screenTopY = Camera.main.ViewportToWorldPoint (new Vector2 (0, 1)).y;
		screenBottomY = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0)).y;

		rb2d = GetComponent<Rigidbody2D> ();

		spawnBubbleRoutine =  StartCoroutine (SpawnBubble ());

		StartCoroutine (AnimateBubble ());

		destroyBubbleOverTimeRoutine = StartCoroutine (DestroyBubbleOverTime ());

	}

	void Update()
	{
		if (transform.position.y > screenTopY + bubbleYoffset) {
			transform.position = new Vector2 (transform.position.x, screenBottomY - bubbleYoffset);
		}
	}

	IEnumerator AnimateBubble()
	{
		Vector2 smallScale = originalScale / 1.05f;

		while (true) {

			if (isSpawnFinish == true) {
				transform.localScale = smallScale;

				yield return new WaitForSeconds (0.2f);

				transform.localScale = originalScale;

				yield return new WaitForSeconds (0.2f);
			} else {
				yield return null;
			}

		}

	}
		
	IEnumerator SpawnBubble()
	{
		isSpawnFinish = false;

		speed = shootSpeed;

		for (int i = 3; i > 0; i--) {

			transform.localScale = originalScale / i;

			yield return new WaitForSeconds (timeBetweenScale);

			speed = speed / 2;
		}


		isSpawnFinish = true;
		speed = originalSpeed;
		direction = Vector2.up;
		gameObject.layer = floatingBubblesLayerID;

	}

	IEnumerator DestroyBubbleOverTime()
	{
		yield return new WaitForSeconds (5f);

		GetComponent<SpriteRenderer> ().sprite = grayBubble;

		yield return new WaitForSeconds (5f);

		GetComponent<SpriteRenderer> ().sprite = transparentBubble;

		yield return new WaitForSeconds (5f);

		DestroyBubble ();


	}

	
	void FixedUpdate () {

		rb2d.velocity = direction * speed;
	}
		
	void OnTriggerEnter2D(Collider2D other)
	{

		if(other.CompareTag("ChangeDirection"))
		{
			direction = other.GetComponent<ChangeDirection> ().direction;
		}

		if (other.CompareTag("Player")) {
			DestroyBubble ();
		}

		if (other.CompareTag("Wall")) {
			StopBubbleSpawn ();
		}

		if (other.CompareTag("Enemy")) {

			StopBubbleSpawn ();
			StopCoroutine (destroyBubbleOverTimeRoutine);

			isEnemyCaptured = true;
			speed = enemyCapturedSpeed;
			GetComponent<SpriteRenderer> ().sprite = enemyTrapped;

		}
	}

	void StopBubbleSpawn()
	{
		StopCoroutine (spawnBubbleRoutine);
		isSpawnFinish = true;

		speed = originalSpeed;
		direction = Vector2.up;
		transform.localScale = originalScale;
		gameObject.layer = floatingBubblesLayerID;

	}

	void DestroyBubble()
	{
		AudioClip destroySound;

		if (isEnemyCaptured == true) {
			Instantiate (collectablePrefab, transform.position, Quaternion.identity);
			destroySound = enemyPopSound;
		} else {
			destroySound = bubblePopSound;
		}

		GetComponent<SpriteRenderer> ().enabled = false;

		audioSource.PlayOneShot (destroySound);

		Destroy (this.gameObject,destroySound.length);
	}

}
