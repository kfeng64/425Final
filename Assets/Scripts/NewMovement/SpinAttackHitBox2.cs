using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttackHitBox2 : MonoBehaviour {
    public Animator anim;
    SphereCollider col;
    public Player2Combat playerCombat;
    public Player1Movement opponent;
    string opponentTag = "Player1";
    float delayBetweenHits = 0.0f;

    // Start is called before the first frame update
    void Start() {
        col = GetComponent<SphereCollider>();
        col.enabled = false;
    }

    // Update is called once per frame
    void Update() {

        if (col.enabled == false) {
            opponent.isInSpinCollider = false;
        } else {
            if (!opponent.invincible) {
                if (opponent.isInSpinCollider) {
                    opponent.velocity = 0;
                    if (delayBetweenHits <= 0) {

                        // PLAY HIT SOUND
                        if (!opponent.invincible && !opponent.isBlocking)
                            playerCombat.hitSound.Play();
                        if (!opponent.invincible && opponent.isBlocking) {
                            playerCombat.hitSound.Play();
                            playerCombat.blockSound.Play();
                        }

                        if (opponent.isBlocking) {
                            opponent.isHit = true;
                            opponent.SetHitDistOpponent(0.2f, 1, -2);
                            opponent.GotKnockBacked(-opponent.transform.forward);
                        } else {
                            opponent.GotSpinHitted(transform.forward);
                            opponent.sentAirborne = true;
                            DealDamage(5);
                        }
                        delayBetweenHits = 0.25f;

                    } else {
                        delayBetweenHits -= Time.deltaTime;
                    }
                } else {

                    Invoke("ResetHitDist", 1.0f);
                }
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

    public void EnableHitBox() {
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
