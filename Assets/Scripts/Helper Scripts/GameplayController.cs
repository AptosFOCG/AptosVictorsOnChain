﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayController : MonoBehaviour {

	public static GameplayController instance;
    public string ModuleAddress = "0xd36ee9d2883da4b1eb018b1f6d7eab57588e5e273c49c919927a8c54b4c647b9";

	public float moveSpeed, distance_Factor = 1f;
	private float distance_Move;
	private bool gameJustStarted;

	public GameObject obstacles_Obj;
	public GameObject[] obstacle_List;

	[HideInInspector]
	public bool obstacles_Is_Active;

	private string Coroutine_Name = "SpawnObstacles";

	private Text score_Text;
	private Text star_Score_Text;

	private int star_Score_Count, score_Count;

	public GameObject pause_Panel;
	public Animator pause_Anim;

	public GameObject gameOver_Panel;
	public Animator gameOver_Anim;

	public Text final_Score_Text, best_Score_Text, final_Star_Score_Text;

	public bool IsMenu;

	void Awake () {
		MakeInstance ();
try {
		score_Text = GameObject.Find ("ScoreText").GetComponent<Text> ();
		star_Score_Text = GameObject.Find ("StarText").GetComponent<Text> ();
}
catch {
	Debug.Log("Inside Menu");
}
	}

	void Start() {
		gameJustStarted = true;

		GetObstacles ();
		StartCoroutine (Coroutine_Name);
		
	}
	void OnEnable() {
		// wallet = new Wallet(PlayerPrefs.GetString("MnemonicsKey"));

	}

	void Update () {
		MoveCamera ();
	}

	void MakeInstance() {
		if (instance == null) {
			instance = this;

		} else if (instance != null) {
			Destroy (gameObject);
		}
	}

	void MoveCamera() {

		if (gameJustStarted) {

			if (!PlayerController.instance.player_Died) {
				// check if player is alive
				if (moveSpeed < 12.0f) {
					moveSpeed += Time.deltaTime * 5.0f;

				} else {
					moveSpeed = 12f;
					gameJustStarted = false;
				}
			}
		}

		// check if player is alive
		if(!PlayerController.instance.player_Died) {
			Camera.main.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
			UpdateDistance ();
		}

	}

void UpdateDistance() {
    distance_Move += Time.deltaTime * distance_Factor;
    float round = Mathf.Round(distance_Move);

    // COUNT AND SHOW THE SCORE
    score_Count = (int)round;
    score_Text.text = round.ToString();

    // Create the transaction payload

    // Rest of your existing code...
    if (round >= 30.0f && round < 60.0f) {
        moveSpeed = 14f;
    } else if (round >= 60f) {
        moveSpeed = 16f;
    }
}

	void GetObstacles() {
		obstacle_List = new GameObject[obstacles_Obj.transform.childCount];

		for (int i = 0; i < obstacle_List.Length; i++) {
			obstacle_List [i] = 
				obstacles_Obj.GetComponentsInChildren<ObstacleHolder> (true) [i].gameObject;
		}
	}

	IEnumerator SpawnObstacles() {

		while (true) {

			if (!PlayerController.instance.player_Died) {

				if (!obstacles_Is_Active) {

					if (Random.value <= 0.85f) {

						int randomIndex = 0;

						do {
							
							randomIndex = Random.Range(0, obstacle_List.Length);

						} while(obstacle_List[randomIndex].activeInHierarchy);

						obstacle_List [randomIndex].SetActive (true);
						obstacles_Is_Active = true;

					}

				}

			}

			yield return new WaitForSeconds (0.6f);
		}
	}

	public async void UpdateStarScore() {
		star_Score_Count++;
		star_Score_Text.text = star_Score_Count.ToString ();

		StartCoroutine(WalletManager.Instance.CollectStar());



	}

	public void PauseGame() {
		Time.timeScale = 0f;
		pause_Panel.SetActive (true);
		pause_Anim.Play ("SlideIn");
	}

	public  void ResumeGame() {
		pause_Anim.Play ("SlideOut");

	}

	public async void RestartGame() {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("Gameplay");
						StartCoroutine(WalletManager.Instance.EndGame());
						StartCoroutine(WalletManager.Instance.StartGame());
	}

	public void HomeButton() {
		Time.timeScale = 1f;
		SceneManager.LoadScene ("MainMenu");
	}

	public async void GameOver() {
		Time.timeScale = 0f;
		gameOver_Panel.SetActive (true);
		gameOver_Anim.Play ("SlideIn");

		final_Score_Text.text = score_Count.ToString ();
		final_Star_Score_Text.text = star_Score_Count.ToString ();

		if (GameManager.instance.score_Count < score_Count) {
			GameManager.instance.score_Count = score_Count;
		}

		best_Score_Text.text = GameManager.instance.score_Count.ToString ();

		StartCoroutine(WalletManager.Instance.UpdateScore(ulong.Parse(score_Count.ToString())));
		StartCoroutine(WalletManager.Instance.EndGame());
		GameManager.instance.starScore += star_Score_Count;

		GameManager.instance.SaveGameData ();

	}

   
} // class























































