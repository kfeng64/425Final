using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotation : MonoBehaviour {
	Collider col;


	// Start is called before the first frame update
	void Start() {
		col = GetComponent<Collider>();
	}

	void FixedUpdate() {

		//transform.position += Vector3.up * Mathf.Cos(Time.time) * .01f;
		if (gameObject.CompareTag("Item")) {
			transform.Rotate(Vector3.up, 5);
		}

		

	}
}
