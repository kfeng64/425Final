using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterChoice : MonoBehaviour
{

	public Button[] p1, p2;
	public int p1Curr, p2Curr;
	Color p1Select, p2Select, confirm;

	public GameObject[] p1Characters, p2Characters;

	Animator p1Anim, p2Anim;

	bool p1Confirm, p2Confirm;

	public GameObject start;
	TextMeshProUGUI startText;

	float timer;

	bool startColor;


    // Start is called before the first frame update
    void Start()
    {

		p1Curr = p2Curr = 0;

		p1Select = new Color(1, .5f, .5f);
		p2Select = new Color(.5f, .5f, 1);
		confirm = new Color(1, 1, 0);

		p1Confirm = p2Confirm = false;

		startText = start.GetComponent<TextMeshProUGUI>();
		startText.text = "";

		timer = 0;
		startColor = true;
	}

    // Update is called once per frame
    void Update()
    {
		p1[p1Curr].GetComponent<Image>().color = Color.white;
		p2[p2Curr].GetComponent<Image>().color = Color.white;

		//p1Characters[p1Curr].SetActive(false);
		//p2Characters[p2Curr].SetActive(false);

		if (!p1Confirm && Input.GetKeyDown(KeyCode.S)) {

			p1Characters[p1Curr].SetActive(false);
			p1Curr = p1Curr == p1.Length - 1 ? 0 : p1Curr + 1;
			p1Characters[p1Curr].SetActive(true);

		}
		if (!p1Confirm && Input.GetKeyDown(KeyCode.W)) {

			p1Characters[p1Curr].SetActive(false);
			p1Curr = p1Curr == 0 ? p1.Length - 1 : p1Curr - 1;
			p1Characters[p1Curr].SetActive(true);

		}

		if (!p2Confirm && Input.GetKeyDown(KeyCode.DownArrow)) {

			p2Characters[p2Curr].SetActive(false);
			p2Curr = p2Curr == p2.Length - 1 ? 0 : p2Curr + 1;
			p2Characters[p2Curr].SetActive(true);

		}
		if (!p2Confirm && Input.GetKeyDown(KeyCode.UpArrow)) {

			p2Characters[p2Curr].SetActive(false);
			p2Curr = p2Curr == 0 ? p2.Length - 1 : p2Curr - 1;
			p2Characters[p2Curr].SetActive(true);

		}


		
		p2[p2Curr].GetComponent<Image>().color = p2Select;

		//p1Characters[p1Curr].SetActive(true);
		//p2Characters[p2Curr].SetActive(true);

		//Confirm Character

		if (Input.GetKeyDown(KeyCode.A)) {

			if (!p1Confirm) {
				p1Anim = p1Characters[p1Curr].GetComponent<Animator>();
				p1Anim.Play("KB_p_OneTwoThree");
				p1Confirm = true;
			} else {
				p1Confirm = false;
			}

		}

		if (p1Confirm) {
			p1[p1Curr].GetComponent<Image>().color = confirm;
			p1[p1Curr].GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
		} else {
			p1[p1Curr].GetComponent<Image>().color = p1Select;
			p1[p1Curr].GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;

		}

		if (Input.GetKeyDown(KeyCode.Period)) {

			if (!p2Confirm) {
				p2Anim = p2Characters[p2Curr].GetComponent<Animator>();
				p2Anim.Play("KB_p_OneTwoThree");
				p2Confirm = true;
			} else {
				p2Confirm = false;
			}
		}

		if (p2Confirm) {
			p2[p2Curr].GetComponent<Image>().color = confirm;
			p2[p2Curr].GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
		} else {
			p2[p2Curr].GetComponent<Image>().color = p2Select;
			p2[p2Curr].GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;

		}


		if (p1Confirm && p2Confirm) {
			startText.text = "Press SPACE to Start";

			if (Input.GetKey(KeyCode.Space)) {
				PlayerSelection.P1Choice = p1Curr;
				PlayerSelection.P2Choice = p2Curr;

				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			}

		} else {
			startText.text = "";
		}

		if (timer < 0) {
			timer = .825f;
			startColor = !startColor;
		} else {
			timer -= Time.deltaTime;
		}

		if (start.activeSelf && startColor) {
			startText.color = Color.red;
		} else {
			startText.color = Color.blue;
		}

	}


}
