using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Combat : MonoBehaviour {

    public Player2Movement player;
    public Player1Movement opponent;
    public SpinAttackHitBox2 spinHitBox;
    string opponentTag = "Player1";
    KeyCode punch = KeyCode.Keypad1;
    KeyCode strongHit = KeyCode.Keypad2;
    KeyCode spin = KeyCode.Keypad3;
    KeyCode block = KeyCode.Keypad4;
	KeyCode grab = KeyCode.Keypad5;

	//public GameObject o1, o2;

	public bool canAttack, isBlocking = false;
    bool isComboing;
    float comboTime;
    int combo;
    public Animator anim;
    public AudioSource attackSound;
    public AudioSource hitSound;
    public AudioSource hitSound2;
    public AudioSource blockSound;
    GameObject spongebob, shrek, shaggy, sasuke;
    public bool spongebobPicked, shrekPicked, shaggyPicked, sasukePicked;

	public bool isGrabbing;
	public GrabHitBox2 grabHitBox;

	// Start is called before the first frame update
	void Start() {
        canAttack = true;
        isComboing = false;
        comboTime = 0.0f;
        combo = 0;

        spongebob = transform.GetChild(0).gameObject;
        shrek = transform.GetChild(1).gameObject;
        shaggy = transform.GetChild(2).gameObject;
        sasuke = transform.GetChild(3).gameObject;

        GameObject characterPicked = null;

		if (PlayerSelection.P2Choice == 0) {
			spongebobPicked = true;
		} else if (PlayerSelection.P2Choice == 1) {
			shrekPicked = true;
		} else if (PlayerSelection.P2Choice == 2) {
			shaggyPicked = true;
		} else if (PlayerSelection.P2Choice == 3) {
			sasukePicked = true;
		}
		//shrekPicked = true;

        if (spongebobPicked) {
            characterPicked = spongebob;
            shrek.SetActive(false);
            shaggy.SetActive(false);
            sasuke.SetActive(false);
        }
        if (shrekPicked) {
            characterPicked = shrek;
            spongebob.SetActive(false);
            shaggy.SetActive(false);
            sasuke.SetActive(false);
        }
        if (shaggyPicked) {
            characterPicked = shaggy;
            shrek.SetActive(false);
            spongebob.SetActive(false);
            sasuke.SetActive(false);
        }
        if (sasukePicked) {
            characterPicked = sasuke;
            shrek.SetActive(false);
            shaggy.SetActive(false);
            spongebob.SetActive(false);
        }

		characterPicked.SetActive(true);
		anim.avatar = characterPicked.GetComponent<Animator>().avatar;
	}

    // Update is called once per frame
    void Update() {

        if (combo == 0 && !player.hasControl && player.hitStunTimer <= 0 && !player.sentAirborne) {
            player.hasControl = true;
        }

        if (player.hitStunTimer > 0) {
            canAttack = false;
        }

        if (player.hasControl && comboTime <= 0) {
            canAttack = true;
        }

        if (player.sentAirborne || anim.GetBool("isJumping") || anim.GetBool("isFalling")) {
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

		if (Input.GetKeyDown(grab) && canAttack) {
			Grab();
		}

	}

	void Grab() {

		player.hasControl = false;
		float attackTime = -1.0f;


		AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
		foreach (AnimationClip clip in clips) {
			switch (clip.name) {
				case "Grabbing":
					attackTime = clip.length;
					break;
			}
		}

		grabHitBox.EnableHitBox();

		if (grabHitBox.grabSuccess) {
			Debug.Log("SUCCESS GRAB");
		}


		if (attackTime != -1.0f) {


			Invoke("AttackCoolDown", attackTime);

			comboTime = attackTime;

			//anim.Play("Grabbing");


			player.startedAttack = true;
			//player.oldPosition = transform.position;
			canAttack = false;
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
            Invoke("AttackCoolDown", attackTime / 5.0f);

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
                if (opponent.isBlocking) {
                    hitSound.Play();
                    blockSound.Play();
                } else if (!opponent.invincible)
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
                if (opponent.isBlocking) {
                    hitSound.Play();
                    blockSound.Play();
                } else if (!opponent.invincible)
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

            Invoke("CurrentlyAttacking", attackTime);
            Invoke("SetHasControlTrue", attackTime);
            player.oldPosition = transform.position;
            //Invoke("ResetHitDist", 1.0f);
        }
    }

    void DealDamage(int damage) {
        opponent.health -= damage;
        //print(opponent.health);
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
