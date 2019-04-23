using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour {

    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation;
    public Camera cam;
    public CharacterController controller;
    private float verticalVel;
    private Vector3 moveVector;

    public float gravity = 9.0f;
    public float jumpSpeed = 4.0f;
    private Vector3 moveDirection = Vector3.zero;
    private bool isJumping;
    int nrOfAlowedDJumps = 1;
    int dJumpCounter = 0;
    private bool isGrounded;


    // Use this for initialization
    void Start() {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        isJumping = false;
    }

    // Update is called once per frame
    void Update() {
        //isGrounded = checkGrounded();
        isGrounded = false;
        isJumping = !isGrounded;
        //print(isGrounded);
        InputMagnitude();
        Jump();
    }

    /*
    bool checkGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, 0.05f, 1 << LayerMask.NameToLayer("Ground"));
    }
    */

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {            
            if (isGrounded) {
                //anim.Play("Jump");
                moveDirection.y = jumpSpeed;
                dJumpCounter = 0;
            }
            if (!isGrounded && dJumpCounter < nrOfAlowedDJumps) {
                //anim.Play("Jump");
                moveDirection.y = jumpSpeed;
                dJumpCounter++;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

    }

    void InputMagnitude() {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        anim.SetFloat("InputZ", InputZ, 0.0f, Time.deltaTime * 2f);
        anim.SetFloat("InputX", InputX, 0.0f, Time.deltaTime * 2f);

        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Physically move player
        if (Speed > allowPlayerRotation) {
            anim.SetFloat("InputMagnitude", Speed, 0.0f, Time.deltaTime);
            PlayerMoveAndRotation();
        } else if (Speed < allowPlayerRotation) {
            anim.SetFloat("InputMagnitude", Speed, 0.0f, Time.deltaTime);
        }
    }

    void PlayerMoveAndRotation() {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * InputZ + right * InputX;

        if (blockRotationPlayer == false) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        }
    }
}