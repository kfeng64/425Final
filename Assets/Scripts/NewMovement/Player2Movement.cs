using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Player2Movement : MonoBehaviour {
    public float
        gravity = 14f,
        jumpForce = 6f,
        wallJumpForce = 6f,
        terminalVelocity = -20f,
        maxSpeed = 4f;

    public Transform
        camTrans;

    private float
        airForward,
        airRight,
        acceleration = 2.5f,
        extraSpeed = 1,
        timer;

    private bool
        startedJump,
        startedWallJump,
        isHoldingJump,
        isRolling,
        canRoll,
        canWallJump,
        resettingExtraSpeed;
    public Vector3 opponentForward;

    public bool
        hasControl;
    public float
        yVelocity;


    private int numJumps;
    private CharacterController controller;
    public Animator anim;
    private Vector3 originalPos, localMove, colNormal, airMovement;

    KeyCode left = KeyCode.LeftArrow;
    KeyCode right = KeyCode.RightArrow;
    KeyCode up = KeyCode.UpArrow;
    KeyCode down = KeyCode.DownArrow;
    KeyCode jump = KeyCode.Keypad0;
    //KeyCode roll = KeyCode.RightShift;
    KeyCode sprint = KeyCode.KeypadPeriod;
    String playerH = "P2Horizontal";
    String playerV = "P2Vertical";
    String opponentAtkArea = "AttackArea1";


    float Horizontal, Vertical;
    public bool isHit, startedAttack, currentlyAttacking = false;
    public Vector3 oldPosition;
    public float horizontalHitDist = 0.0f, horizontalHitSpeed = 1;
    public bool isInHitCollider = false, isInSpinCollider, sentAirborne = false, onGroundAfterAirborne = false, isBlocking = false;
    public GameObject player, opponent;
    public float hitStunTimer = 0.0f;
    public bool invincible = false, isSprinting = false, isInBackFistCollider = false;
    public float velocity = 0f;
    public int health = 100;
    public AudioSource jumpSound;
    public Slider healthBar;
    GameObject spongebob, shrek, shaggy, sasuke;
    public bool spongebobPicked, shrekPicked, shaggyPicked, sasukePicked;
	public AudioSource hitsound;

	public bool inGrabRange;

    private AnimatorClipInfo[] clipInfo;

    //public string GetCurrentClipName() {
    //    clipInfo = anim.GetCurrentAnimatorClipInfo(0);
    //    float time = clipInfo[0].clip.length;
    //    return clipInfo[0].clip.name + " " + time;
    //}

    private void Start() {
        controller = GetComponent<CharacterController>();
        originalPos = transform.position;
        colNormal = Vector3.zero;
        hasControl = true;

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

    private void Update() {

        Horizontal = Input.GetAxis(playerH);
        Vertical = Input.GetAxis(playerV);

        healthBar.value = health;
        if (health <= 0) {
            hasControl = false;
            invincible = true;
            anim.Play("Dead2");
        }

        AnimationUpdate();
        if (hasControl) {
            InControl();
        } else {
            NotInControl();
        }



        if (sentAirborne && controller.isGrounded) {
            hitStunTimer = 1.3f;
            invincible = true;
            Invoke("DisableInvincibility", 1.5f);
            sentAirborne = false;
            onGroundAfterAirborne = true;
            anim.SetBool("onGroundAfterAirborne", true);
        }

        if (onGroundAfterAirborne) {
            Invoke("OnGroundAfterAirborne", .75f);
        }

        if (sentAirborne) {
            hasControl = false;
            Physics.IgnoreCollision(player.GetComponent<CharacterController>(), opponent.GetComponent<CharacterController>(), true);
        } else {
            Physics.IgnoreCollision(player.GetComponent<CharacterController>(), opponent.GetComponent<CharacterController>(), false);
        }

        if (!opponent.GetComponent<CharacterController>().isGrounded) {
            Physics.IgnoreCollision(player.GetComponent<CharacterController>(), opponent.GetComponent<CharacterController>(), true);
        } else {
            Physics.IgnoreCollision(player.GetComponent<CharacterController>(), opponent.GetComponent<CharacterController>(), false);
        }

        if (!hasControl && !currentlyAttacking) {
            hitStunTimer -= Time.deltaTime;
            if (hitStunTimer <= 0 && !sentAirborne) {
                hasControl = true;
                hitStunTimer = 0;
            }
        }

		

    }

	public void gotGrabbed() {

		Invoke("grabAnim", .1f);

		
	}

	void grabAnim() {
		if (inGrabRange) {
			hasControl = false;
			transform.LookAt(opponent.transform);
			transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
			hitStunTimer = 1.5f;

			anim.Play("Grabbed");

			StartCoroutine(GrabDamage(2));

		}
	}

	IEnumerator GrabDamage(int damage) {

		health -= damage;
		yield return new WaitForSeconds(.8f);
		health -= damage;
		//print(opponent.health);
	}



	public void hitByProjectile() {
		hasControl = false;
		
		hitStunTimer = .9f;

		hitsound.Play();
		anim.Play("GroundHitStun");
		health -= 5;
	}


    void OnGroundAfterAirborne() {
        if (CheckForMovementKeys()) {
            hasControl = true;

            // Rolling
            if (controller.isGrounded && !isRolling) {
                canRoll = true;
            }

            if (canRoll == true) {
                Roll();
            }

        } else {
            canRoll = false;
        }
        Invoke("NormalGetUp", .1f);
        Invoke("OnGroundAfterAirborneFalse", .7f);
        onGroundAfterAirborne = false;
    }

    void NormalGetUp() {
        anim.SetBool("NormalGetUp", true);
        Invoke("DisableNormalGetUp", .7f);
    }

    void DisableNormalGetUp() {
        anim.SetBool("NormalGetUp", false);
    }

    void OnGroundAfterAirborneFalse() {
        anim.SetBool("onGroundAfterAirborne", false);
    }

    void AnimationUpdate() {
        isBlocking = anim.GetBool("isBlocking");

        if (isSprinting && CheckForMovementKeys()) {
            anim.SetBool("isSprinting", true);
        } else {
            anim.SetBool("isSprinting", false);
        }

        if (hasControl) {
            anim.SetFloat("InputX", Horizontal);
            anim.SetFloat("InputZ", Vertical);
        } else {
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputZ", 0);
        }

        if (hasControl) {
            anim.SetBool("hasControl", true);
        } else {
            anim.SetBool("hasControl", false);
        }

        if (sentAirborne) {
            anim.SetBool("sentAirborne", true);
        } else {
            anim.SetBool("sentAirborne", false);
        }

        if (controller.isGrounded) {
            anim.SetBool("isGrounded", true);
        } else {
            anim.SetBool("isGrounded", false);
        }

        if (hasControl) {
            if (yVelocity > -2 && !controller.isGrounded) {
                anim.SetBool("isJumping", true);
            } else {
                anim.SetBool("isJumping", false);
            }
            if (yVelocity <= -2 && !controller.isGrounded) {
                anim.SetBool("isFalling", true);
            } else {
                anim.SetBool("isFalling", false);
            }

            if (isRolling) {
                anim.SetBool("isRolling", true);
            } else {
                anim.SetBool("isRolling", false);
            }

            anim.SetFloat("yVelocity", yVelocity);
        }



        if (CheckForMovementKeys()) {
            anim.SetBool("MovementPressed", true);
        } else {
            anim.SetBool("MovementPressed", false);
        }

    }

    void InControl() {
        hitStunTimer = 0;
        // By default, character cant wall jump. Will be set to true if controller hits wall
        canWallJump = false;

        // Sprinting
        if (Input.GetKey(sprint) && controller.isGrounded) {
            isSprinting = true;
            maxSpeed = 5.5f;
        } else {
            isSprinting = false;
            maxSpeed = 4.0f;
        }

        // Jumping
        if (Input.GetKeyDown(jump)) {

            Jump();
        }

        // Get the localMove vector. Handles movement based on camera
        MovementInput();

        // Ground Movement
        if (controller.isGrounded) {
            MoveCharGround();

        }
        // Air Movement
        else if (!controller.isGrounded) {
            canRoll = false;
            MoveCharAir();
        }

        // Restart if controller falls out of map
        if (transform.position.y <= -5) {
            Restart();
        }

        // Rolling
        if (controller.isGrounded && !isRolling) {
            canRoll = true;
        }

        //if (Input.GetKeyDown(roll) && canRoll == true) {
        //    Roll();
        //}

        if (isRolling == true) {
            WhileRolling();
        }

        // WallJump
        if (canWallJump) {
            WallJump();
        }

        if (resettingExtraSpeed) {
            ResetExtraSpeed();
        }


        // Gravity
        controller.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
        Gravity();


        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, localMove, Color.green);

        colNormal.y = 0;

        Util1.StartTimer(3);
    }

    void NotInControl() {
        velocity = 0;
        canWallJump = false;
        MovementInput();


        if (!controller.isGrounded) {
            canRoll = false;
            MoveCharAir();
        }

        if (transform.position.y <= -5) {
            Restart();
        }

        if (isRolling == true) {
            WhileRolling();
        }

        if (startedAttack) {
            controller.Move(transform.forward * Time.deltaTime * horizontalHitSpeed);
            if (Vector3.Distance(oldPosition, transform.position) >= horizontalHitDist) {
                startedAttack = false;
            }
        }

        if (isHit) {
            if (isBlocking) {
                transform.LookAt(opponent.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            controller.Move(opponentForward * Time.deltaTime * horizontalHitSpeed);
            if (Vector3.Distance(oldPosition, transform.position) >= horizontalHitDist) {
                isHit = false;
                anim.SetBool("isHit", false);
            }
        }

        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, localMove, Color.green);


        // Gravity
        controller.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
        Gravity();
    }

    public void SetHitDistOpponent(float distH, float speedFactorH, float yVelocity) {
        if (!invincible && isInHitCollider) {
            isHit = true;
            horizontalHitDist = distH;
            horizontalHitSpeed = speedFactorH;
            if (isInHitCollider && !isBlocking) {
                this.yVelocity = yVelocity;
            }
        }
    }

    public void SetHitDistPlayer(float distH, float speedFactorH, float yVelocity) {
        horizontalHitDist = distH;
        horizontalHitSpeed = speedFactorH;
        if (isInHitCollider && !isBlocking) {
            this.yVelocity = yVelocity;
        }
    }

    public void ResetHitDist() {
        horizontalHitDist = 0;
        horizontalHitSpeed = 0;
    }

    public void GotSpinHitted(Vector3 vec) {
        if (isInSpinCollider) {
            isHit = true;
        }
        if (isInSpinCollider && !invincible && !isBlocking) {
            sentAirborne = true;
            KnockBackAnimation();
            hasControl = false;
            yVelocity = 3.7f;
            controller.Move(-vec * (Time.deltaTime * 5));
        }
    }

    public void GotKnockBacked(Vector3 vec) {
        isHit = true;
        if (!invincible || !isBlocking) {
            KnockBackAnimation();
            hasControl = false;
            opponentForward = vec;
            oldPosition = transform.position;
        }

    }

    void KnockBackAnimation() {
        if (!invincible && !isBlocking) {
            if (!sentAirborne) {
                anim.SetBool("isHit", true);
                anim.Play("GroundHitStun");
            } else {
                anim.SetBool("isHit", true);
                anim.Play("KB_HighKO_Air");
            }
        }

    }
    // Lerp extra speed back to normal speed
    void ResetExtraSpeed() {
        if (extraSpeed == 1.0f) {
            resettingExtraSpeed = false;
        }
        extraSpeed = Mathf.Lerp(extraSpeed, 1.0f, 0.05f);
    }


    // Move the character on the ground by the given vector movement
    void MoveCharGround() {

        // Rotate character to localMove vector
        RotateChar(localMove);

        // Get new speed value
        velocity = DampSpeed(velocity);

        controller.Move(((transform.forward * Time.deltaTime) * velocity) * extraSpeed);

        //If there is no input from the player, and char's speed is < 0.3, then set speed to 0.
        if (!checkForInput() && velocity <= 0.3f) {
            velocity = 0;
        }

    }

    void RotateChar(Vector3 original) {
        Vector3 facingRot = ((transform.forward) + (original.normalized)).normalized;
        facingRot.y = 0;

        transform.forward = facingRot;

    }

    void MoveCharAir() {
        if (localMove.sqrMagnitude > 1f) {
            localMove = localMove.normalized;
        }

        velocity = DampSpeedAir(velocity);

        airForward = Vector3.Dot(transform.forward, localMove);
        airRight = Vector3.Dot(transform.right, localMove);

        airMovement = (transform.forward * velocity) + ((transform.right) * (airRight * 0.9f));

        controller.Move((airMovement * extraSpeed) * Time.deltaTime);
    }




    float DampSpeed(float originalVelocity) {
        if (!checkForInput() && controller.isGrounded) {
            acceleration = 10;
        } else {
            acceleration = 2.5f;
        }

        float desiredSpeed = maxSpeed * new Vector3(getXAxis(), 0, getZAxis()).magnitude;
        float new_ratio = 0.9f * Time.deltaTime * acceleration;
        float old_ratio = 1 - new_ratio;
        float newSpeed = (originalVelocity * old_ratio) + (desiredSpeed * new_ratio);
        newSpeed = Mathf.Clamp(newSpeed, -maxSpeed * extraSpeed, maxSpeed * extraSpeed);

        return newSpeed;
    }

    //Dampen speed for aerial mobility
    float DampSpeedAir(float originalVelocity) {
        acceleration = 1.5f;
        float desiredSpeed = maxSpeed * airForward;
        float new_ratio = 0.9f * Time.deltaTime * acceleration;
        float old_ratio = 1 - new_ratio;
        float newSpeed = (originalVelocity * old_ratio) + (desiredSpeed * new_ratio);
        newSpeed = Mathf.Clamp(newSpeed, -maxSpeed * extraSpeed, maxSpeed * extraSpeed);

        return newSpeed;
    }


    //Translate character's movement from world to camera perspective 
    void MovementInput() {
        if (Input.GetKey(left) || Input.GetKey(up) || Input.GetKey(down) || Input.GetKey(right)) {
            //Get the forward vector of camera
            Vector3 lookDir = camTrans.forward;
            lookDir.y = 0;
            lookDir = lookDir.normalized;

            //Get the right vector of the camera
            Vector3 right = camTrans.right;
            right.y = 0;
            right = right.normalized;

            localMove.x = (Horizontal * right.x) + (Vertical * lookDir.x);
            localMove.z = (Horizontal * right.z) + (Vertical * lookDir.z);
            localMove.y = 0;
            localMove = localMove.normalized;
        } else {
            // Prevent Jumping forward while standing still
            localMove.x /= 2.0f;
            localMove.z /= 2.0f;
        }
    }

    //Apply gravity to the character
    void Gravity() {
        //Set isHoldingJump relative to if the player is holding the jump button
        if (Input.GetKey(jump)) {
            isHoldingJump = true;
        } else {
            isHoldingJump = false;
        }


        // If character is grounded and he isn't jumping, apply a miniscule amount of gravity.
        if (controller.isGrounded && !startedJump) {
            gravity = 14f;
            yVelocity = -gravity * Time.deltaTime;
        }

        // If the player is jumping (not falling) and they aren't holding the jump button, increase their gravity
        // Creates short-hop like jump
        if (!isFalling() && !controller.isGrounded) {
            if (!isHoldingJump) {
                gravity = 24f;
            } else
                gravity = 14f;
        }

        // If character is airborne, subtract gravity from velocity.
        if (!controller.isGrounded) {
            yVelocity -= gravity * Time.deltaTime;
            startedJump = false;
        }

        // If the character is falling (not jumping), revert gravity to normal.
        if (isFalling()) {
            gravity = 14f;
        }

        // Ensure the character doesn't fall faster than their terminalVelocity.
        if (isFalling() && yVelocity < terminalVelocity) {
            yVelocity = terminalVelocity;
        }
    }

    void WallJump() {
        if (Input.GetKeyDown(jump)) {
            if (colNormal.normalized.y < 0.01f && colNormal.normalized.y > -0.01f) {
                jumpSound.Play();
                startedWallJump = true;
                transform.forward = colNormal.normalized;
                yVelocity = wallJumpForce;
                extraSpeed = 4.0f;
                resettingExtraSpeed = true;
                startedWallJump = false;
            }


        }
    }

    //Change the value of yVelocity, allowing the character to jump when MoveChar() is called
    void Jump() {

        if (controller.isGrounded) {
            jumpSound.Play();
            anim.SetBool("isJumping", true);
            numJumps = 2;
            yVelocity = jumpForce;
            startedJump = true;
            numJumps--;

        } else {
            if (numJumps > 0) {
                jumpSound.Play();
                anim.SetBool("isJumping", true);
                if (localMove != Vector3.zero && !startedWallJump) {
                    transform.forward = AngledDoubleJump();
                }

                startedJump = true;
                yVelocity = jumpForce;
                numJumps--;

            }
        }
    }

    void SetLocalMove(float x, float z) {
        Vector3 lookDir = camTrans.forward;
        lookDir.y = 0;
        lookDir = lookDir.normalized;

        Vector3 right = camTrans.right;
        right.y = 0;
        right = right.normalized;

        localMove.x = (x * right.x) + (z * lookDir.x);
        localMove.z = (x * right.z) + (z * lookDir.z);
    }

    Vector3 AngledDoubleJump() {
        if (Input.GetKey(left)) {
            if (Input.GetKey(up)) {
                SetLocalMove(-1, 1);
            } else if (Input.GetKey(down)) {
                SetLocalMove(-1, -1);
            } else if (Input.GetKey(right)) {
                SetLocalMove(1, 0);
            } else {
                SetLocalMove(-1, 0);
            }
        }

        if (Input.GetKey(up)) {
            if (Input.GetKey(left)) {
                SetLocalMove(-1, 1);
            } else if (Input.GetKey(down)) {
                SetLocalMove(-1, 0);
            } else if (Input.GetKey(right)) {
                SetLocalMove(1, 1);
            } else {
                SetLocalMove(0, 1);
            }
        }

        if (Input.GetKey(down)) {
            if (Input.GetKey(up)) {
                SetLocalMove(0, 1);
            } else if (Input.GetKey(left)) {
                SetLocalMove(-1, -1);
            } else if (Input.GetKey(right)) {
                SetLocalMove(1, -1);
            } else {
                SetLocalMove(0, -1);
            }
        }

        if (Input.GetKey(right)) {
            if (Input.GetKey(up)) {
                SetLocalMove(1, 1);
            } else if (Input.GetKey(down)) {
                SetLocalMove(1, -1);
            } else if (Input.GetKey(left)) {
                SetLocalMove(-1, 0);
            } else {
                SetLocalMove(1, 0);
            }
        }


        localMove.y = 0;
        localMove = localMove.normalized;
        return localMove;
    }



    void Roll() {
        extraSpeed = 4.0f;
        isRolling = true;
        canRoll = false;
        Util1.StartTimer(0.2f);
    }

    void WhileRolling() {
        extraSpeed = Mathf.Lerp(extraSpeed, 1, 0.1f);

        if (extraSpeed <= 1.02f) {
            extraSpeed = 1;
        }


        if (extraSpeed == 1) {
            isRolling = false;
            canRoll = true;
        }
    }

    //Reset character's position to original.
    void Restart() {
        transform.position = originalPos;
    }

    float getXAxis() {
        return Horizontal;
    }

    float getZAxis() {
        return Vertical;
    }

    //Check stick axis to check if there is any input from the player
    //True = input, false = no input
    bool checkForInput() {
        if (getXAxis() != 0 || getZAxis() != 0) {
            return true;
        } else {
            return false;
        }
    }

    //Check if character is falling
    bool isFalling() {
        if (!controller.isGrounded && yVelocity <= 0) {
            return true;
        } else {
            return false;
        }
    }

    //If the player collides with a "Wall" object, allow the player to walljump.
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.gameObject.tag == "Wall") {
            if (!controller.isGrounded) {
                canWallJump = true;
            }
            colNormal = hit.normal.normalized;
        }
    }

    public void PushOutFromOtherCollider() {
        controller.Move(transform.forward * -1.0f * (Time.deltaTime * 2.5f));
        // Gravity
        controller.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
        Gravity();
    }

    bool CheckForMovementKeys() {
        return Input.GetKey(left) || Input.GetKey(right) || Input.GetKey(up) || Input.GetKey(down);
    }

    void DisableInvincibility() {
        invincible = false;
    }

    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == opponentAtkArea) {
            isInHitCollider = true;
        }
		if (collision.gameObject.CompareTag("Projectile")) {
			if (collision.GetComponent<ProjectileDamage>().parent != gameObject) {
				hitByProjectile();


				Destroy(collision.gameObject);
			}
			
		}
	}


    void OnTriggerExit(Collider collision) {
        if (collision.gameObject.tag == opponentAtkArea) {
            isInHitCollider = false;
        }
    }



}
