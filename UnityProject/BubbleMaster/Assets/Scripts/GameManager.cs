using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public Text scoreText;							// UI text score
	public Text playerLives;						// UI player Lives
	public Text endScoreText;						// UI text score shown at game over panel screen
	public GameObject gameOverPanel;				// UI Panel shown after player lose all lives
	public PlayerController ourPlayer;				// Ref to the player

	void Start () {
		ourPlayer = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();
	}

	void Update () {

		scoreText.text = "Score : " + ourPlayer.score;
		playerLives.text = "x " + ourPlayer.lives;

		if (ourPlayer.lives <=0 && ourPlayer != null) {
			EndGame ();
		}
		
	}

	// Pause game and display game over panel
	void EndGame()
	{
		Time.timeScale = 0;
		endScoreText.text = "Your Score : " + ourPlayer.score;
		gameOverPanel.SetActive (true);
		Destroy (ourPlayer.gameObject);
	}

	// Reload current scene
	public void PlayAgain()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
