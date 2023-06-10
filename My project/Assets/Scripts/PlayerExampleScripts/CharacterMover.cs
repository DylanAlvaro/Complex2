using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]

public class CharacterMover : MonoBehaviour
{
    // public fields
    public float movementSpeed = 2;
    public float jumpVelocity = 2.5f;
    public Vector3 velocity;
    public float rotationSpeed = 10f;

    // set on awake
    private Camera cam;
    private Animator animator;
    private CharacterController characterController;

    // private fields
    private Vector2 _playerVelocity;
    private Vector2 _smoothedPlayerVelocity;
    private Vector3 spawnPoint = new Vector3(-28.67f, 6.33f, -22.56f);

    // required for basic movement functionality
    private Vector2 moveInput = new Vector2();
    private bool jumpInput;
    public bool isGrounded = false;
    private Vector2 _direction;
    private bool isJumping;


    // public Vector3 respawnPoint = new Vector3(1.59f, 6.33f, 32.8f );

   // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        jumpInput = Input.GetButton("Jump");
        
       animator.SetFloat("Forwards", moveInput.x, 0.1f, Time.deltaTime);
       animator.SetFloat("Blend", moveInput.y, 0.1f, Time.deltaTime);
       animator.SetBool("Jump", !isGrounded);
    }

    public Vector3 hitDir;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitDir = hit.point - transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 delta;
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        
        delta = (moveInput.x * camRight + moveInput.y * camForward) * movementSpeed;

        if(isGrounded || moveInput.x != 0 || moveInput.y != 0)
        {
            velocity.x = delta.x;
            velocity.z = delta.z;
        }
        
        if(jumpInput && isGrounded)
        {
            velocity.y = jumpVelocity;
        }


        if(isGrounded && velocity.y < 0)
            velocity.y = 0;

        velocity += Physics.gravity * Time.fixedDeltaTime;

        if(!isGrounded)
            hitDir = Vector3.zero;

        if(moveInput.x == 0 && moveInput.y == 0)
        {
            Vector3 horizontalHitDir = hitDir;
            horizontalHitDir.y = 0;
            float displacement = horizontalHitDir.magnitude;

            if(displacement > 0)
            {
                velocity -= 0.2f * horizontalHitDir / displacement;
            }
        }
        
        transform.forward = camForward;
        
        characterController.Move(velocity * Time.deltaTime);
       
        isGrounded = characterController.isGrounded;
       
       Quaternion rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
       transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.fixedDeltaTime * rotationSpeed);
    }
}
