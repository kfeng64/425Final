using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHitBox2 : MonoBehaviour
{
	public Animator anim;
	BoxCollider col;
	public Player2Combat playerCombat;
	public Player1Movement opponent;
	string opponentTag = "Player1";
	public bool grabSuccess;

	// Start is called before the first frame update
	void Start()
    {
		col = GetComponent<BoxCollider>();
		col.enabled = false;
		grabSuccess = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (col.enabled) {
			if (opponent.inGrabRange) {
				grabSuccess = true;
				anim.SetBool("grabSuccess", true);
				

			} 
		}
    }

	public void EnableHitBox() {
		anim = playerCombat.anim;

		float attackTime = -1.0f;
		AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
		foreach (AnimationClip clip in clips) {
			switch (clip.name) {
				case "Grabbing":
					attackTime = clip.length;
					break;
			}
		}
		col.enabled = true;

		anim.Play("Grabbing");

		
		opponent.gotGrabbed();


		StartCoroutine(DealDamage(2));


		Invoke("DisableHitBox", attackTime);

	}

	void DisableHitBox() {
		col.enabled = false;
		opponent.inGrabRange = false;
		grabSuccess = false;
		anim.SetBool("grabSuccess", false);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == opponentTag) {
			opponent.inGrabRange = true;
		}
	}

	IEnumerator DealDamage(int damage) {

		opponent.health -= damage;
		yield return new WaitForSeconds(.8f);
		opponent.health -= damage;
		//print(opponent.health);
	}
}
