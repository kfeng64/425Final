using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotation : MonoBehaviour {
	Collider col;
	Rigidbody itemRB;
	bool falling;

	// Start is called before the first frame update
	void Start() {
		col = GetComponent<Collider>();
		//itemRB = GetComponent<Rigidbody>();
		//itemRB.useGravity = true;
		falling = true;
	}

	void FixedUpdate() {

		//transform.position += Vector3.up * Mathf.Cos(Time.time) * .01f;
		if (gameObject.CompareTag("Item")) {
			transform.Rotate(Vector3.up, 5);
		}

		if (transform.position.y < 1.5f) {
			falling = false;
		}

		if (falling) {
			transform.Translate(Vector3.down * .1f);
		}
		

	}

	//private void OnTriggerEnter(Collider other) {
	//	itemRB.useGravity = false;
	//}
}
