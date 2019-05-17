using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Combat : MonoBehaviour {

    public Player1Movement player;
    public Player2Movement opponent;
    public SpinAttackHitBox spinHitBox;
    string opponentTag = "Player2";
    KeyCode punch = KeyCode.E;
    KeyCode strongHit = KeyCode.Q;
    KeyCode spin = KeyCode.R;
    KeyCode block = KeyCode.C;

    //public GameObject o1, o2;

    public bool canAttack, isBlocking = false;
    bool isComboing;
    float comboTime;
    int combo;
    public Animator anim;
    public AudioSource attackSound;
    public AudioSource hitSound;
    public AudioSource hitSound2;


    // Start is called before the first frame update
    void Start() {
        canAttack = true;
        isComboing = false;
        comboTime = 0.0f;
        combo = 0;
        
    }

    // Update is called once per frame
    void Update() {
        
        if (combo == 0 && !player.hasControl && player.hitStunTimer <= 0 && !player.sentAirborne) {
            player.hasControl = true;
        }

        if (player.hitStunTimer > 0) {
            canAttack = false;
        }

        if (player.sentAirborne) {
            canAttack = false;
        }

        if (Input.GetKeyDown(punch) && player.isSprinting && canAttack) {
            BackFist();
        }

        if (Input.GetKeyDown(punch) && !player.isSprinting && canAttack) {
            Punch();
        }

        if (Input.GetKeyDown(strongHit) && canAttack) {
            StrongHit();
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

    void StrongHit() {
        

        player.hasControl = false;
        player.currentlyAttacking = true;
        isComboing = true;
        float attackTime = -1.0f;
        combo++;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "KB_m_Overhand_L":
                    if (combo == 1)
                        attackTime = clip.length;
                    break;
                case "KB_m_Overhand_R":
                    if (combo == 2)
                        attackTime = clip.length;
                    break;
                case "KB_m_Overhand_L_Copy":
                    if (combo == 3)
                        attackTime = clip.length;
                    break;
                case "KB_Superpunch":
                    if (combo == 4)
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
                        anim.Play("KB_m_Overhand_L");
                        player.SetHitDistPlayer(0.2f, 1, -2);
                        opponent.SetHitDistOpponent(0.2f, 1, -2);
                        break;
                    case 2:
                        anim.Play("KB_m_Overhand_R");
                        player.SetHitDistPlayer(0.2f, 1, -2);
                        opponent.SetHitDistOpponent(0.2f, 1, -2);
                        break;
                    case 3:
                        anim.Play("KB_m_Overhand_L_Copy");
                        player.SetHitDistPlayer(0.2f, 1, -2);
                        opponent.SetHitDistOpponent(0.2f, 1, -2);
                        break;
                    case 4:
                        anim.Play("KB_Superpunch");
                        player.SetHitDistPlayer(0.5f, 10, -2);
                        opponent.SetHitDistOpponent(3.0f, 5, -2);
                        break;
                }
            }

            player.startedAttack = true;
            if (opponent.isInHitCollider) {
                if (opponent.isBlocking && combo == 4) {
                    opponent.SetHitDistOpponent(0.2f, 1, -2);
                }
                opponent.GotKnockBacked(transform.forward);
            }


            if (opponent.isInHitCollider && !opponent.isBlocking) {
                if (combo == 4) {
                    opponent.sentAirborne = true;
                    opponent.GotKnockBacked(transform.forward);
                    //Invoke("ResetHitDist", 1.0f);
                }
                opponent.hitStunTimer = 0.45f;
                DealDamage(5);
            }

            if (opponent.isInHitCollider) {
                // PLAY HIT SOUND
                if (!opponent.invincible)
                    hitSound2.Play();
                else
                    attackSound.Play();
            } else {
                attackSound.Play();
            }


            if (comboTime > 0) {
                CancelInvoke("CurrentlyAttacking");
                CancelInvoke("SetHasControlTrue");
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
                        player.SetHitDistPlayer(0.2f, 1, -2);
                        opponent.SetHitDistOpponent(0.2f, 1, -2);
                        break;
                    case 2:
                        anim.Play("KB_m_Jab_L");
                        player.SetHitDistPlayer(0.2f, 1, -2);
                        opponent.SetHitDistOpponent(0.2f, 1, -2);
                        break;
                    case 3:
                        anim.Play("KB_p_Uppercut_R");
                        player.SetHitDistPlayer(0.5f, 10, -2);
                        opponent.SetHitDistOpponent(0.025f, .5f, 7.0f);
                        
                        break;
                }
            }
            
            player.startedAttack = true;
            if (opponent.isInHitCollider) {
                if (opponent.isBlocking && combo == 3) {
                    opponent.SetHitDistOpponent(0.2f, 1, -2);
                }
                opponent.GotKnockBacked(transform.forward);
            }


            if (opponent.isInHitCollider && !opponent.isBlocking) {
                if (combo == 3) {
                    opponent.sentAirborne = true;
                    opponent.GotKnockBacked(transform.forward);
                }
                DealDamage(5);
                opponent.hitStunTimer = 0.45f;
            }

            if (opponent.isInHitCollider) {
                // PLAY HIT SOUND
                if (!opponent.invincible)
                    hitSound.Play();
                else
                    attackSound.Play();
            } else {
                attackSound.Play();

            }


            if (comboTime > 0) {
                CancelInvoke("CurrentlyAttacking");
                CancelInvoke("SetHasControlTrue");
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
                    attackTime = clip.length / 2;
                    break;
            }
        }

        if (attackTime != -1.0f) {
            canAttack = false;
            Invoke("AttackCoolDown", attackTime);

            comboTime = attackTime;

            if (comboTime > 0) {
                anim.Play("KB_m_BackfistRoundFar_R");
                player.SetHitDistPlayer(1.7f, 10, -2);
                opponent.SetHitDistOpponent(3.0f, 5, -2);
            }

            player.startedAttack = true;

            if (opponent.isInHitCollider) {
                // PLAY HIT SOUND
                if (!opponent.invincible)
                    hitSound.Play();
                else
                    attackSound.Play();
            } else {
                attackSound.Play();
            }

            Invoke("CurrentlyAttacking", attackTime);
            Invoke("SetHasControlTrue", attackTime);
            player.oldPosition = transform.position;
            //Invoke("ResetHitDist", 1.0f);
        }
    }

    void DealDamage(int damage) {
        opponent.health -= damage;
        print(opponent.health);
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

    void ResetHitDist() {
        opponent.ResetHitDist();
    }

    private void OnTriggerStay(Collider other) {
        if (Input.GetKeyDown(punch) && other.CompareTag(opponentTag)) {
            transform.LookAt(other.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }
}
