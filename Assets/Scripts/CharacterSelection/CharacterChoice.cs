using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterChoice : MonoBehaviour
{

	public Button[] p1, p2;
	public int p1Curr, p2Curr;
	Color p1Select, p2Select;

	public GameObject[] p1Characters, p2Characters;


    // Start is called before the first frame update
    void Start()
    {

		p1Curr = p2Curr = 0;

		p1Select = new Color(1, .5f, .5f);
		p2Select = new Color(.5f, .5f, 1);

	}

    // Update is called once per frame
    void Update()
    {
		p1[p1Curr].GetComponent<Image>().color = Color.white;
		p2[p2Curr].GetComponent<Image>().color = Color.white;

		//p1Characters[p1Curr].SetActive(false);
		//p2Characters[p2Curr].SetActive(false);

		if (Input.GetKeyDown(KeyCode.S)) {

			p1Characters[p1Curr].SetActive(false);
			p1Curr = p1Curr == p1.Length - 1 ? 0 : p1Curr + 1;
			p1Characters[p1Curr].SetActive(true);

		}
		if (Input.GetKeyDown(KeyCode.W)) {

			p1Characters[p1Curr].SetActive(false);
			p1Curr = p1Curr == 0 ? p1.Length - 1 : p1Curr - 1;
			p1Characters[p1Curr].SetActive(true);

		}

		if (Input.GetKeyDown(KeyCode.DownArrow)) {

			p2Characters[p2Curr].SetActive(false);
			p2Curr = p2Curr == p2.Length - 1 ? 0 : p2Curr + 1;
			p2Characters[p2Curr].SetActive(true);

		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {

			p2Characters[p2Curr].SetActive(false);
			p2Curr = p2Curr == 0 ? p2.Length - 1 : p2Curr - 1;
			p2Characters[p2Curr].SetActive(true);

		}


		p1[p1Curr].GetComponent<Image>().color = p1Select;
		p2[p2Curr].GetComponent<Image>().color = p2Select;

		//p1Characters[p1Curr].SetActive(true);
		//p2Characters[p2Curr].SetActive(true);

	}

	
}
