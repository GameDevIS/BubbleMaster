using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// Public Vars
	public int lives;								// Player lives count
	public int score;								// Player score
	public int bubbleScore;							// Amount to add to score if we destroy a bubble
	public int collectableScore;					// Amount to add to score after picking up collectable

	public int jumpForce;							// Player jump force
	public int playerSpeed;							// Player speed
	public float shootDelay;						// delay between bubble shoots
	public float flyForce;							// force applied to player after got hit

	public int blinksCount;							// nubmer of blinks to do in player respawn func
	public float timeBetweenBlinks;					// time between blinks

	public bool isGrounded;							// Is player touching the ground
	public float groundRadius;						// Radius to check if player touching ground
	public LayerMask whatIsGround;					// Defines which layer is the ground layer to check isGrounded aginst
	public Transform[] groundPoints;				// Points we check if touching the ground layer
	public GameObject bubblePrefab;					// Bubble prefab
	public Transform bubbleSpawnPoint;				// Bubble spawn position
	public Transform playerSpawnPosition;			// Player spawn position

	// Sounds
	public AudioClip jumpSound;
	public AudioClip shootSound;
	public AudioClip pickUpSound;
	public AudioClip deathSound;

	// Private Vars 
	bool canShoot;									// define if the player can\can't shoot
	Vector2 direction;								// direction the player moving
	Rigidbody2D rb2d;		
	AudioSource audioSource;

	// Use this for initialization
	void Start () {	
	
		canShoot = true;

		direction = Vector2.right;
		rb2d = GetComponent<Rigidbody2D> ();
		audioSource = GetComponent<AudioSource> ();
	}

	void Update()
	{
		isGrounded = checkIsGrounded ();

		if( isGrounded && Input.GetKeyDown("up"))
		{
			rb2d.AddForce (new Vector2 (0, jumpForce),ForceMode2D.Impulse);
			audioSource.PlayOneShot (jumpSound);
		}

		// Checks if left control was pressed and if player can shoot then shoot bubble
		if(Input.GetKeyDown(KeyCode.LeftControl) && canShoot == true)
		{
			StartCoroutine(ShootBubble ());
		}
			

	}

	IEnumerator ShootBubble()
	{
		GameObject bubble =  Instantiate (bubblePrefab, bubbleSpawnPoint.position, Quaternion.identity);
		bubble.GetComponent<BubbleController> ().direction = direction;

		audioSource.PlayOneShot (shootSound);

		canShoot = false;

		yield return new WaitForSeconds (shootDelay);

		canShoot = true;

	}
		

	bool checkIsGrounded()
	{
		for (int i = 0; i < groundPoints.Length; i++) 
		{
			if (Physics2D.OverlapCircle(groundPoints[i].position,groundRadius,whatIsGround)) {
				return true;
			}
		}

		return false;
	}

	void FixedUpdate () {

		float inputX = Input.GetAxis ("Horizontal");

		if (inputX < 0) 
		{
			direction = Vector2.left;
		} 
		else if (inputX > 0) 
		{
			direction = Vector2.right;
		}

		transform.localScale = new Vector2 (direction.x, transform.localScale.y);

		rb2d.velocity = new Vector2 (inputX * playerSpeed, rb2d.velocity.y);
		
	}
		
	void OnCollisionEnter2D(Collision2D other)
	{

		if (other.gameObject.CompareTag("Enemy")) {
			KillPlayer ();
		}

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Bubble")) {
			score = score + bubbleScore;
		}

		if (other.CompareTag("Collectable")) {
			score = score + collectableScore;
			audioSource.PlayOneShot (pickUpSound);
			Destroy (other.gameObject);
		}

	}

	void KillPlayer()
	{
		lives--;
		audioSource.PlayOneShot (deathSound);

		if (lives > 0) {
			StartCoroutine (RespawnPlayer ());
		}

	}

	IEnumerator RespawnPlayer()
	{
		canShoot = false;

		GetComponent<BoxCollider2D> ().enabled = false;

		rb2d.AddForce (Vector2.up * flyForce, ForceMode2D.Impulse);

		yield return new WaitForSeconds (2f);

		rb2d.velocity = new Vector2 (0, 0);

		transform.position = playerSpawnPosition.position;

		GetComponent<BoxCollider2D> ().enabled = true;


		for (int i = 0; i < blinksCount; i++) {

			GetComponent<SpriteRenderer> ().enabled = false;

			yield return new WaitForSeconds (timeBetweenBlinks);

			GetComponent<SpriteRenderer> ().enabled = true;

			yield return new WaitForSeconds (timeBetweenBlinks);

		}

		canShoot = true;

	}

}
