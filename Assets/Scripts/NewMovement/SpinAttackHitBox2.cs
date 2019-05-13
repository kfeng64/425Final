using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttackHitBox2 : MonoBehaviour
{
    public Animator anim;

    KeyCode spin = KeyCode.O;
    string opponentTag = "Player1";

    SphereCollider col;
    public Player2Combat playerCombat;
    public Player1Movement opponent;
    float delayBetweenHits = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<SphereCollider>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(spin) && playerCombat.canAttack) {
            EnableHitBox();
        }


        if (col.enabled == false) {
            opponent.isInSpinCollider = false;
        } else {
            if (opponent.isInSpinCollider) {
                opponent.velocity = 0;
                if (delayBetweenHits <= 0) {
                    opponent.GotSpinHitted(transform.forward);
                    
                    opponent.sentAirborne = true;
                    delayBetweenHits = 0.25f;
                } else {
                    delayBetweenHits -= Time.deltaTime;
                }
            } else {
                Invoke("ResetHitDist", 1.0f);
            }
        }
    }

    void ResetHitDist() {
       opponent.ResetHitDist();
    }

    void EnableHitBox() {
        float attackTime = -1.0f;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "SpinAttack":
                    attackTime = clip.length;
                    break;
            }
        }
        col.enabled = true;
        Invoke("DisableHitBox", attackTime - .5f);

    }

    void DisableHitBox() {
        col.enabled = false;
        
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == opponentTag) {
            opponent.isInSpinCollider = true;
        }
    }
}
