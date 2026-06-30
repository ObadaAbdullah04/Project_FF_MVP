using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Templates.ColorCube
{


public class MenuSelect : MonoBehaviour {

	public GameObject[] obstacles;
	public GameObject mainMenuUI;
	public GameObject gameplayUI;
	public GameObject pauseButton;
	public GameObject scoreText;
	public GameObject pauseMenuUI;
	public GameObject gameOverMenuUI;
	public GameObject player;
	public GameObject statisticsUI;
	public GameObject settingsUI;

	public GameObject gameOverScore;
	public GameObject gameOverBestScore;

	public GameObject lastScoreText;
	public GameObject bestScoreText;
	public GameObject gamesPlayedText;
	public GameObject colorChangeText;

	public GameObject soundOff;
	public GameObject musicOff;

	public bool _useAdapterMode;

	public static MenuSelect Instance { get; private set; }

	
	private AudioSource buttonClick;

	void Awake() {
		Instance = this;
		Time.timeScale = 1;
		Application.targetFrameRate = 300;
		if (!_useAdapterMode && PlayerPrefs.GetInt ("restartTheGame") == 1) {
			PlayerPrefs.SetInt ("restartTheGame", 0);
			GameStart();
		}
	}

	void Start() {
		if (_useAdapterMode)
		{
			if (gameplayUI != null) gameplayUI.SetActive(false);
			if (ColorCubeAdapter.Instance != null)
				ColorCubeAdapter.Instance.BeginGame();
			return;
		}

		buttonClick = GameObject.Find("buttonClickSound").GetComponent<AudioSource>();

		if(PlayerPrefs.GetInt("soundOff") == 1) {
			GameObject.Find("explosionSound").GetComponent<AudioSource> ().mute = true;
			GameObject.Find("colorChangeSound").GetComponent<AudioSource> ().mute = true;
			GameObject.Find("successSound").GetComponent<AudioSource> ().mute = true;
			GameObject.Find("buttonClickSound").GetComponent<AudioSource> ().mute = true;
			soundOff.SetActive(true);
		
		}else {
			GameObject.Find("explosionSound").GetComponent<AudioSource> ().mute = false;
			GameObject.Find("colorChangeSound").GetComponent<AudioSource> ().mute = false;
			GameObject.Find("successSound").GetComponent<AudioSource> ().mute = false;
			GameObject.Find("buttonClickSound").GetComponent<AudioSource> ().mute = false;
			soundOff.SetActive(false);
		}

		if(PlayerPrefs.GetInt("musicOff") == 1) {
			GameObject.Find("music").GetComponent<AudioSource> ().mute = true;
			musicOff.SetActive(true);
		}else {
			GameObject.Find("music").GetComponent<AudioSource> ().mute = false;
			musicOff.SetActive(false);
		}
	}

	public void GameStart() {
		PlayerPrefs.SetInt("gamesPlayed", PlayerPrefs.GetInt("gamesPlayed") + 1);
		PlayerPrefs.SetInt("lastScore", 0);
		PlayerLogic.collision = 0;
		foreach (GameObject obstacle in obstacles)
		{
			obstacle.GetComponent<Obstacle>().enabled = true;
		}
		if (!_useAdapterMode)
		{
			mainMenuUI.SetActive(false);
			gameplayUI.SetActive(true);
		}
		if (player != null)
		{
			if (player.GetComponent<PlayerLogic>() != null)
				player.GetComponent<PlayerLogic>().enabled = true;
			if (player.GetComponent<ColorSwap>() != null)
				player.GetComponent<ColorSwap>().enabled = true;
		}
		if(buttonClick == null) {
			buttonClick = GameObject.Find("buttonClickSound")?.GetComponent<AudioSource>();
		}
		if (buttonClick != null)
			buttonClick.Play();
	}

	public void GameContinue() {
		Time.timeScale = 1;
		buttonClick.Play();
		PlayerLogic.collision++;
		Destroy(GameObject.Find("explosion"));
		Destroy(GameObject.Find("Main Camera"));
		gameOverMenuUI.SetActive(false);
		gameplayUI.SetActive(true);
		GameObject player = Instantiate (Resources.Load ("player") as GameObject);
		player.transform.localPosition = new Vector3(0,0, (PlayerPrefs.GetInt("lastScore") + PlayerLogic.collision - 1) * 100 + 80);
		player.name = "player";
		pauseButton.SetActive(true);
		scoreText.SetActive(true);
	}

	public void OpenStatisticsMenu() {
		lastScoreText.GetComponent<Text> ().text = "LAST SCORE: " + PlayerPrefs.GetInt("lastScore", 0);
		bestScoreText.GetComponent<Text> ().text = "BEST SCORE: " + PlayerPrefs.GetInt("bestScore", 0);
		gamesPlayedText.GetComponent<Text> ().text = "GAMES PLAYED: " + PlayerPrefs.GetInt("gamesPlayed", 0);
		colorChangeText.GetComponent<Text> ().text = "COLOR SWAP: " + PlayerPrefs.GetInt("colorChange", 0);
		statisticsUI.SetActive(true);
		buttonClick.Play();
	}

	public void CloseStatisticsMenu() {
		statisticsUI.SetActive(false);
		buttonClick.Play();
	}

	public void TurnOnOffSound() {
		if(PlayerPrefs.GetInt("soundOff") == 1) {
			PlayerPrefs.SetInt("soundOff", 0);
			GameObject.Find("explosionSound").GetComponent<AudioSource> ().mute = false;
			GameObject.Find("colorChangeSound").GetComponent<AudioSource> ().mute = false;
			GameObject.Find("successSound").GetComponent<AudioSource> ().mute = false;
			GameObject.Find("buttonClickSound").GetComponent<AudioSource> ().mute = false;
			soundOff.SetActive(false);
		}else {
			PlayerPrefs.SetInt("soundOff", 1);
			GameObject.Find("explosionSound").GetComponent<AudioSource> ().mute = true;
			GameObject.Find("colorChangeSound").GetComponent<AudioSource> ().mute = true;
			GameObject.Find("successSound").GetComponent<AudioSource> ().mute = true;
			GameObject.Find("buttonClickSound").GetComponent<AudioSource> ().mute = true;
			soundOff.SetActive(true);
		}
		buttonClick.Play();
	}

	public void TurnOnOffMusic() {
		if(PlayerPrefs.GetInt("musicOff") == 1) {
			PlayerPrefs.SetInt("musicOff", 0);
			GameObject.Find("music").GetComponent<AudioSource> ().mute = false;
			musicOff.SetActive(false);
		}else {
			PlayerPrefs.SetInt("musicOff", 1);
			GameObject.Find("music").GetComponent<AudioSource> ().mute = true;
			musicOff.SetActive(true);
		}
		buttonClick.Play();
	}

	public void OpenSettingsMenu() {
		settingsUI.SetActive(true);
		if(GameObject.Find("explosionSound").GetComponent<AudioSource> ().mute == true) {
			soundOff.SetActive(true);
		}
		if(GameObject.Find("music").GetComponent<AudioSource> ().mute == true) {
			musicOff.SetActive(true);
		}
		buttonClick.Play();
	}

	public void CloseSettingsMenu() {
		settingsUI.SetActive(false);
		buttonClick.Play();
	}

	public void PauseMenu() {
		Time.timeScale = 0;
		pauseButton.SetActive(false);
		scoreText.SetActive(false);
		pauseMenuUI.SetActive(true);
		if(player == null) 
			player = GameObject.Find("player");
		player.GetComponent<ColorSwap>().enabled = false;
		buttonClick.Play();
	}
	public void GameOver() {
		pauseButton.SetActive(false);
		scoreText.SetActive(false);
		if (!_useAdapterMode)
			Invoke("GameOverMenu", 1.5f);
	}
	public void GameOverMenu() {
		pauseButton.SetActive(false);
		scoreText.SetActive(false);
		gameOverMenuUI.SetActive(true);
		gameOverScore.GetComponent<Text> ().text = "SCORE: " + PlayerPrefs.GetInt("lastScore");
		gameOverBestScore.GetComponent<Text> ().text = "BEST SCORE: " + PlayerPrefs.GetInt("bestScore");
	}
	public void Resume() {
		Time.timeScale = 1;
		pauseButton.SetActive(true);
		scoreText.SetActive(true);
		pauseMenuUI.SetActive(false);
		player.GetComponent<ColorSwap>().enabled = true;
		buttonClick.Play();
	}
	public void ExitToMainMenu() {
		buttonClick.Play();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
	public void RestartGame() {
		PlayerPrefs.SetInt ("restartTheGame", 1);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
	public void ExitTheGame() {
		buttonClick.Play();
		Application.Quit();
	}
}

}
