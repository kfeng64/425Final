using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
	public GameObject p1, p2, parent;
	Player1Movement p1Mov;
	Player2Movement p2Mov;



	bool isProjectile;


    // Start is called before the first frame update
    void Start()
    {
		p1 = GameObject.FindGameObjectWithTag("Player1");
		p2 = GameObject.FindGameObjectWithTag("Player2");
		p1Mov = p1.GetComponent<Player1Movement>();
		p2Mov = p2.GetComponent<Player2Movement>();

		//parent = null;
	}

    // Update is called once per frame
    void Update()
    {

        if (gameObject.CompareTag("Projectile")) {
			isProjectile = true;
		} else {
			isProjectile = false;
		}

		if (transform.parent != null) {
			gameObject.GetComponent<CapsuleCollider>().enabled = false;
		} else {
			gameObject.GetComponent<CapsuleCollider>().enabled = true;
		}


    }

	private void OnTriggerEnter(Collider other) {
		//if (!isProjectile && (other.CompareTag("Player1") || other.CompareTag("Player2"))) {
		//	parent = other.gameObject;
		//}

		//if (other != parent) {
		//	if (isProjectile) {
		//		if (parent == p1) {
		//			p2Mov.hitByProjectile();
		//		} else if (parent == p2) {
		//			p1Mov.hitByProjectile();
		//		}

				
		//		transform.SetParent(other.transform);
		//		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		//	}
		//}

		
	}


}
