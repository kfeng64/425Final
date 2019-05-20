using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Winning : MonoBehaviour {
	public Player1Movement p1;
	public Player2Movement p2;
	TextMeshProUGUI KO;
	public AudioSource KOSound;

	bool checkHealth;

	// Start is called before the first frame update
	void Start() {
		KO = GetComponent<TextMeshProUGUI>();

		KO.text = "";
		checkHealth = true;
	}

	// Update is called once per frame
	void Update() {

		if (checkHealth) {

			if (p1.health <= 0 || p2.health <= 0) {
				KO.text = "KO";

				checkHealth = false;

				KOSound.Play();

				Invoke("backToSelect", 5);
			}

		}




	}


	void backToSelect() {
		if (p1.health <= 0) {
			PlayerSelection.p2WinCount += 1;
		} else {
			PlayerSelection.p1WinCount += 1;
		}
		SceneManager.LoadScene(0);
	}
}
