using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Combat : MonoBehaviour
{
	public int health;
	public GameObject tester;
	Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
		health = 100;
    }

    // Update is called once per frame
    void Update()
    {
		tester.transform.position = transform.position + transform.forward.normalized;
		rb = GetComponent<Rigidbody>();

	}

	private void OnTriggerStay(Collider other) {

		if (Input.GetKeyDown(KeyCode.C) && other.CompareTag("Player2")) {
			transform.LookAt(other.transform);
			//other.GetComponent<Combat>().loseHealth(0);
			other.GetComponent<Rigidbody>().AddRelativeForce((transform.forward.normalized + Vector3.up) * .3f, ForceMode.Impulse);
			
			
		}
	}

	public void loseHealth(int attackType) {
		switch (attackType) {
			case 0:
				health -= 1;
			break;

		}
	}

}
