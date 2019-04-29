using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player1Movement : MonoBehaviour
{
    public float
        gravity = 28f,
        jumpForce = 8f,
        wallJumpForce = 6f,
        terminalVelocity = -20f,
        maxSpeed = 4f;

    public Transform
        camTrans;

    private float
        yVelocity,
        velocity = 0f,
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

    private int numJumps;
    private CharacterController controller;
    private Animator anim;
    private Vector3 originalPos, localMove, colNormal, airMovement;

    KeyCode left = KeyCode.A;
    KeyCode right = KeyCode.D;
    KeyCode up = KeyCode.W;
    KeyCode down = KeyCode.S;
    KeyCode jump = KeyCode.Space;
    KeyCode roll = KeyCode.LeftShift;
    float Horizontal, Vertical;

    private void Start() {
        controller = GetComponent<CharacterController>();
        originalPos = transform.position;
        colNormal = Vector3.zero;
        anim = GetComponent<Animator>();
    }

    private void Update() {
        Horizontal = Input.GetAxis("P1Horizontal");
        Vertical = Input.GetAxis("P1Vertical");
        anim.SetFloat("InputX", Horizontal);
        anim.SetFloat("InputZ", Vertical);

        if (yVelocity >= 0 && !controller.isGrounded) {
            anim.SetBool("isJumping", true);
        } else {
            anim.SetBool("isJumping", false);
        }
        if (yVelocity < 0 && !controller.isGrounded) {
            anim.SetBool("isFalling", true);
        } else {
            anim.SetBool("isFalling", false);
        }

        if (controller.isGrounded) {
            anim.SetBool("isGrounded", true);
        } else {
            anim.SetBool("isGrounded", false);
        }

        if (isRolling) {
            anim.SetBool("isRolling", true);
        } else {
            anim.SetBool("isRolling", false);
        }

        anim.SetFloat("yVelocity", yVelocity);

        // By default, character cant wall jump. Will be set to true if controller hits wall
        canWallJump = false;

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
        if (transform.position.y <= -1.3) {
            Restart();
        }

        // Gravity
        controller.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);
        Gravity();

        // Rolling
        if (controller.isGrounded && !isRolling) {
            canRoll = true;
        }

        if (Input.GetKeyDown(roll) && canRoll == true) {
            Roll();
        }

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
            

        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, localMove, Color.green);

        colNormal.y = 0;

        Util1.StartTimer(3);
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
    void MovementInput()
    {
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
    void Gravity()
    {
        //Set isHoldingJump relative to if the player is holding the jump button
        if(Input.GetKey(jump)) {
            isHoldingJump = true;
        } else {
            isHoldingJump = false;
        }


        // If character is grounded and he isn't jumping, apply a miniscule amount of gravity.
        if (controller.isGrounded && !startedJump)
        {
            gravity = 14f;
            yVelocity = -gravity * Time.deltaTime;
        }

        // If the player is jumping (not falling) and they aren't holding the jump button, increase their gravity
        // Creates short-hop like jump
        if(!isFalling() && !controller.isGrounded)
        {
            if (!isHoldingJump)
            {
                gravity = 24f;
            }
            else
                gravity = 14f;
        }

        // If character is airborne, subtract gravity from velocity.
        if(!controller.isGrounded) {
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
            numJumps = 2;
            yVelocity = jumpForce;
            startedJump = true;
            numJumps--;
        } else {
            if (numJumps > 0) {
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
        

        if(extraSpeed == 1) {
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

    void OnCollisionEnter(Collision collision) {
        
    }

    void OnCollisionExit(Collision collision) {
        
    }



}
