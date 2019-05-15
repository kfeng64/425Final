using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Combat : MonoBehaviour {

    public Player1Movement player;
    public Player2Movement opponent;
    public SpinAttackHitBox spinHitBox;
    string opponentTag = "Player2";
    KeyCode punch = KeyCode.E;
    KeyCode spin = KeyCode.R;
    KeyCode block = KeyCode.G;

    //public GameObject o1, o2;

    public bool canAttack, canBackFistOutOfSprint = false, isBlocking = false;
    bool isComboing;
    float comboTime;
    int combo;
    public Animator anim;
    

    // Start is called before the first frame update
    void Start() {
        canAttack = true;
        isComboing = false;
        comboTime = 0.0f;
        combo = 0;
        
    }

    // Update is called once per frame
    void Update() {
        
        if (player.isSprinting) {
            canAttack = false;
            canBackFistOutOfSprint = true;
        } else {
            canAttack = true;
            canBackFistOutOfSprint = false;
        }



        if (Input.GetKeyDown(punch) && canBackFistOutOfSprint) {
            BackFist();
        }

        if (Input.GetKeyDown(punch) && canAttack) {
            Punch();
        }

        if (Input.GetKeyDown(spin) && canAttack) {
            SpinAttack();
        }

        if (Input.GetKey(block) && canAttack && player.hitStunTimer <= 0) {
            isBlocking = true;
        } else {
            isBlocking = false;
        }
        anim.SetBool("isBlocking", isBlocking);
        Block();

        if (isComboing) {
            comboTime -= Time.deltaTime;
        }

        if (isComboing && comboTime <= 0) {
            isComboing = false;
            combo = 0;
        }

    }

    void Block() {
        if (isBlocking) {
            player.hasControl = false;
            
        }
        
    }

    void BackFist() {
        player.hasControl = false;
        player.currentlyAttacking = true;
        isComboing = true;
        float attackTime = -1.0f;
        combo++;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "KB_m_BackfistRoundFar_R":
                    attackTime = clip.length / 3;
                    break;
            }
        }

        if (attackTime != -1.0f) {
            canAttack = false;
            Invoke("AttackCoolDown", attackTime);

            comboTime = attackTime;

            if (comboTime > 0) {
                anim.Play("KB_m_BackfistRoundFar_R");
                player.SetHitDist(1.7f, 10, -2);
                opponent.SetHitDist(3.0f, 5, -2);
            }

            player.startedAttack = true;

            if (comboTime > 0) {
                CancelInvoke("CurrentlyAttacking");
                //CancelInvoke("SetHasControlTrue");
            }

            if (opponent.isInHitCollider) {
                // PLAY HIT SOUND
            }

            Invoke("CurrentlyAttacking", attackTime);
            Invoke("SetHasControlTrue", attackTime);
            player.oldPosition = transform.position;
        }
    }

    void Punch() {
        player.hasControl = false;
        player.currentlyAttacking = true;
        isComboing = true;
        float attackTime = -1.0f;
        combo++;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "KB_m_Jab_R":
                    if (combo == 1)
                        attackTime = clip.length;
                    break;
                case "KB_m_Jab_L":
                    if (combo == 2)
                        attackTime = clip.length;
                    break;
                case "KB_p_Uppercut_R":
                    if (combo == 3)
                        attackTime = clip.length;
                    break;
            }
        }

        if (attackTime != -1.0f) {
            canAttack = false;
            Invoke("AttackCoolDown", attackTime / 3.5f);

            comboTime = attackTime;

            if (comboTime > 0) {
                switch (combo) {
                    case 1:
                        anim.Play("KB_m_Jab_R");
                        player.SetHitDist(0.2f, 1, -2);
                        opponent.SetHitDist(0.2f, 1, -2);
                        break;
                    case 2:
                        anim.Play("KB_m_Jab_L");
                        player.SetHitDist(0.2f, 1, -2);
                        opponent.SetHitDist(0.2f, 1, -2);
                        break;
                    case 3:
                        anim.Play("KB_p_Uppercut_R");
                        player.SetHitDist(0.5f, 10, -2);
                        opponent.SetHitDist(0.025f, .5f, 7.0f);
                        
                        break;
                }
            }
            
            player.startedAttack = true;
            if (opponent.isInHitCollider) {
                if (opponent.isBlocking && combo == 3) {
                    opponent.SetHitDist(0.2f, 1, -2);
                }
                opponent.GotKnockBacked(transform.forward);
            }


            if (opponent.isInHitCollider && !opponent.isBlocking) {
                if (combo == 3) {
                    opponent.sentAirborne = true;
                    opponent.GotKnockBacked(transform.forward);
                }
                opponent.hitStunTimer = 0.45f;
            }

            if (opponent.isInHitCollider) {
                // PLAY HIT SOUND
            }


            if (comboTime > 0) {
                CancelInvoke("CurrentlyAttacking");
                //CancelInvoke("SetHasControlTrue");
            }
            Invoke("CurrentlyAttacking", attackTime);
            Invoke("SetHasControlTrue", attackTime);
            player.oldPosition = transform.position;
        }
    }

    void SpinAttack() {
        player.hasControl = true;
        isComboing = true;
        float attackTime = -1.0f;
        combo++;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "SpinAttack":
                    attackTime = clip.length;
                    break;
            }
        }

        if (attackTime != -1.0f) {

            
            Invoke("AttackCoolDown", attackTime);

            comboTime = attackTime;

            if (comboTime > 0) {
                anim.Play("SpinAttack");
            }

            player.startedAttack = true;
            player.oldPosition = transform.position;
            spinHitBox.EnableHitBox();
            canAttack = false;
        }
    }

    void SetHasControlTrue() {
        player.hasControl = true;
    }

    void CurrentlyAttacking() {
        player.currentlyAttacking = false;
    }

    void AttackCoolDown() {
        canAttack = true;
    }

    private void OnTriggerStay(Collider other) {
        if (Input.GetKeyDown(punch) && other.CompareTag(opponentTag)) {
            transform.LookAt(other.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
