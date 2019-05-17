using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1Movement : MonoBehaviour
{
	public float speed, jumpForce;
	private Rigidbody rb;
	float moveV, moveH;
	public Camera cam;
	Vector3 camForward, camRight;
	public bool isGrounded;
	public int numJumps;
    public float distToGround;

    public float fallDelay = 0.2f;

    // Start is called before the first frame update
    void Start() {
		rb = GetComponent<Rigidbody>();

		camForward = new Vector3(cam.transform.position.x, 0, cam.transform.position.z).normalized * -1;
		camRight = cam.transform.right.normalized;

        distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;

    }

    // Update is called once per frame
    void FixedUpdate() {
        //CheckForGrounded();
        moveV = Input.GetAxis("P1Vertical") * speed;
		moveH = Input.GetAxis("P1Horizontal") * speed;

		//Debug.Log(moveH + " ------------- " + moveV);

		
		
		Vector3 newPosition = transform.position + (camForward * moveV) + (camRight * moveH);
		Vector3 betterPos = new Vector3(newPosition.x, transform.position.y, newPosition.z);

		transform.LookAt(newPosition);
		transform.position = newPosition;
		

		if (isGrounded) {
			numJumps = 1;
		}

		if (Input.GetKeyDown(KeyCode.Space) && numJumps > 0) {
            
			if (isGrounded) {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			} else {
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				rb.AddForce(Vector3.up * jumpForce * .65f, ForceMode.Impulse);
			}
			
			numJumps--;
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

    //void CheckForGrounded() {
    //    isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround);
    //}

}
