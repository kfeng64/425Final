using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackFistHitBox : MonoBehaviour {
    public Animator anim;
    KeyCode hit = KeyCode.E;
    KeyCode sprint = KeyCode.LeftShift;
    BoxCollider col;
    public Player1Combat playerCombat;
    public Player2Movement opponent;
    float delayBetweenHits = 0.0f;

    // Start is called before the first frame update
    void Start() {
        col = GetComponent<BoxCollider>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(hit) && Input.GetKey(sprint) && playerCombat.canAttack) {
            EnableHitBox();
        }


        if (col.enabled == false) {
            opponent.isInBackFistCollider = false;
        } else {
            if (opponent.isInBackFistCollider) {
                // PLAY HIT SOUND
                if (!opponent.invincible)
                    playerCombat.hitSound.Play();

                if (opponent.isBlocking) {
                    opponent.isHit = true;
                    opponent.SetHitDistOpponent(0.2f, 1, -2);
                    opponent.GotKnockBacked(-opponent.transform.forward);
                } else {
                    opponent.sentAirborne = true;
                    opponent.GotKnockBacked(transform.forward);
                    opponent.hasControl = false;
                    Invoke("ResetHitDist", 1.0f);
                    DealDamage(5);
                }
                col.enabled = false;

            }
            
        }

    }

    void DealDamage(int damage) {
        opponent.health -= damage;
        print(opponent.health);
    }

    void ResetHitDist() {
        opponent.ResetHitDist();
    }

    void EnableHitBox() {
        // PLAY HIT SOUND
        if (!opponent.invincible && playerCombat.canAttack)
            playerCombat.attackSound.Play();

        float attackTime = -1.0f;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "KB_m_BackfistRoundFar_R":
                    attackTime = clip.length / 3;
                    break;
            }
        }
        col.enabled = true;
        Invoke("DisableHitBox", attackTime);
    }

    void DisableHitBox() {
        col.enabled = false;
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player2") {
            opponent.isInBackFistCollider = true;
        }
    }
}
