using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent((typeof(Rigidbody)))]
[RequireComponent((typeof(CapsuleCollider)))]
public class CharacterMovementController : MonoBehaviour {

    const float FORWARD_TO_BACKWARD_RATIO = 1.0f;

    public float inputDelay = 0.1f;
    public float forwardVelocity = 7.0f, rotateVelocity = 2.2f;
    public float leftRightVelocity = 5.0f, jumpVelocity = 0.05f;
    public float dashVelocity = 220.0f, dashCooldownTime = 12.0f;


    float lockedForwardInput = 0, lockedLeftRightInput = 0;
    Vector3 transformForwardOnLastLock = Vector3.zero, transformRightOnLastLock = Vector3.zero;

    Rigidbody rigidBody;
    CapsuleCollider charCollider;
    public Animator animator;

    public float currentX = 0.0f;
    public float forwardInput, leftRightInput;
    float sensitivity = 1.0f;
    bool mouseHold, mouseDown, rightMouseDown, jump, dash;

    bool playerIsInControl = true;
    float distToGround;
    float jumpSpeed;





    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
        charCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        forwardInput = leftRightInput = 0;
        jumpSpeed = 3.5f;
    }

    void GetInput() {
        if (playerIsInControl) {
            if (animator) {
                animator.Play("Movement");
            }
            
            leftRightInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical") * (1 - Mathf.Abs(leftRightInput) / 3);
            //mouseDown = Input.GetButtonDown("Fire1");
            //mouseHold = Input.GetButton("Fire1");
            //rightMouseDown = Input.GetButtonDown("Fire2");
            jump = Input.GetKeyDown(KeyCode.Space);
            //dash = Input.GetButtonDown("Dash");
            currentX += Input.GetAxis("Mouse X");
        }

    }


    void Update() {
        GetInput();
    }

    void FixedUpdate() {
        MoveForward();
        MoveLeftRight();
        Turn();
        Jump();
        //Dash();
    }

    void MoveForward() {
        float forwardInputToUse = forwardInput;
        //float forwardInputToUse = isGrounded() ? forwardInput : lockedForwardInput;
        if (animator) animator.SetFloat("InputZ", forwardInputToUse);

        if (Mathf.Abs(forwardInput) > inputDelay) {
            Vector3 transformForwardToUse = isGrounded() ? transform.forward : transformForwardOnLastLock;
            float forwardVelocityToUse = forwardInputToUse > 0 ? forwardVelocity : forwardVelocity * FORWARD_TO_BACKWARD_RATIO;
            rigidBody.MovePosition(rigidBody.position + transform.forward * forwardVelocityToUse * forwardInputToUse * Time.fixedDeltaTime);
        }
    }

    void MoveLeftRight() {
        float leftRightInputToUse = leftRightInput;
        //float leftRightInputToUse = isGrounded() ? leftRightInput : lockedLeftRightInput;
        if (animator) animator.SetFloat("InputX", leftRightInputToUse);

        if (Mathf.Abs(leftRightInput) > inputDelay) {
            Vector3 transformRightToUse = isGrounded() ? transform.right : transformRightOnLastLock;
            rigidBody.MovePosition(rigidBody.position + transform.right * leftRightVelocity * leftRightInputToUse * Time.fixedDeltaTime);
        }
    }

    void Turn() {
        Quaternion mouseRotation = Quaternion.Euler(0, currentX * sensitivity * 2.0f, 0.0f);
        rigidBody.MoveRotation(mouseRotation);
    }



    void Jump() {
        if (jump && isGrounded()) {
            rigidBody.AddForce(new Vector3(0.0f, 2.0f, 0.0f) * jumpSpeed, ForceMode.Impulse);

            /*
            var velocity = GetComponent<Rigidbody>().angularVelocity;
            velocity.y = jumpSpeed;
            GetComponent<Rigidbody>().angularVelocity = velocity;*/
            /*
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
            rigidBody.AddForce(transform.up * jumpVelocity, ForceMode.Impulse);
            if (animator) animator.Play("Jump");

            lockedForwardInput = forwardInput;
            lockedLeftRightInput = leftRightInput;
            transformForwardOnLastLock = transform.forward;
            transformRightOnLastLock = transform.right;*/
        }
    }



    /*Tells us if player is currently on a hard surface - THIS NEEDS TO BE FIXED*/
    bool isGrounded() {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        /*
        float radius = charCollider.radius * .4f;
        Vector3 pos = transform.position + Vector3.up * (radius * 0.2f);
        LayerMask ignorePlayerMask = ~(1 << 8);
        return Physics.CheckSphere(pos, radius, ignorePlayerMask);*/
    }

    float dashNextAllowedTimeStamp = -100;
    void Dash() {
        if (dash && dashNextAllowedTimeStamp <= Time.time) {
            dashNextAllowedTimeStamp = Time.time + dashCooldownTime;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x / 2, rigidBody.velocity.y, rigidBody.velocity.z / 2);
            rigidBody.AddForce(transform.forward * dashVelocity, ForceMode.Impulse);
        }
    }

    /*Takes away or grants control over movement to local player*/
    public void playerHasControl(bool hasControl) {
        playerIsInControl = hasControl;
    }

}
