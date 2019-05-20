using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
	KeyCode throwKey = KeyCode.Z;

	public bool hasItem;
	Collider player;
	Transform itemSlot;
	GameObject item;
	Rigidbody itemRB;
	GameObject opponent;

	Animator anim;

    // Start is called before the first frame update
    void Start()
    {
		player = GetComponent<CharacterController>();
		hasItem = false;

		itemSlot = transform.GetChild(10);

		anim = GetComponent<Animator>();
		item = null;

		if (player.gameObject.CompareTag("Player1")) {
			opponent = GameObject.FindGameObjectWithTag("Player2");
			throwKey = KeyCode.Z;
		} else {
			opponent = GameObject.FindGameObjectWithTag("Player1");
			throwKey = KeyCode.M;
		}

    }

    // Update is called once per frame
    void Update()
    {
        if (hasItem && Input.GetKey(throwKey)) {
			

			StartCoroutine(throwItem());
		}
    }

	IEnumerator throwItem() {

		Vector3 towardsOpponent = opponent.transform.position - player.gameObject.transform.position.normalized;

		player.transform.LookAt(opponent.transform);
		anim.Play("KB_KnifeThrow");


		yield return new WaitForSeconds(.4f);

		item.transform.SetParent(null);

		item.GetComponent<Collider>().isTrigger = false;
		
		item.transform.LookAt(opponent.transform);

		itemRB.useGravity = true;
		itemRB.AddForce(item.transform.forward * 5, ForceMode.Impulse);

		itemRB.AddRelativeTorque(Vector3.right * 100);

		item.tag = "Projectile";

		hasItem = false;

		Destroy(item, 5);
	}

	


	private void OnTriggerEnter(Collider other) {
		if (!hasItem && other.CompareTag("Item")) {
			item = Instantiate(other.gameObject, itemSlot);
			item.transform.position = itemSlot.position;
			itemRB = item.GetComponent<Rigidbody>();
			item.GetComponent<ProjectileDamage>().parent = gameObject;
			Destroy(other.gameObject);
			hasItem = true;
		}
	}
}
