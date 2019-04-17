using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2Movement : MonoBehaviour
{
	public float speed, jumpForce;
	private Rigidbody rb;
	float moveV, moveH;
	public Camera cam;
	Vector3 camForward, camRight;
	public bool isGrounded;
	public int numJumps;


	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody>();

		camForward = new Vector3(cam.transform.position.x, 0, cam.transform.position.z).normalized * -1;
		camRight = cam.transform.right.normalized;

	}

	// Update is called once per frame
	void FixedUpdate() {
		moveV = Input.GetAxis("P2Vertical") * speed;
		moveH = Input.GetAxis("P2Horizontal") * speed;

		Debug.Log(moveH + " ------------- " + moveV);

		

		Vector3 newPosition = transform.position + (camForward * moveV) + (camRight * moveH);

		transform.LookAt(newPosition);
		transform.position = newPosition;

		if (isGrounded) {
			numJumps = 1;
		}

		if (Input.GetKeyDown(KeyCode.RightControl) && numJumps > 0) {
			if (isGrounded) {
				rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			} else {
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				rb.AddForce(Vector3.up * jumpForce * .65f, ForceMode.Impulse);
			}
		}
	}

	private void OnTriggerEnter(Collider other) {

		if (other.CompareTag("Ground")) {
			isGrounded = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Ground")) {
			isGrounded = false;
		}
	}
}
