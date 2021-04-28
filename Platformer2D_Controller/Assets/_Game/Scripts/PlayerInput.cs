using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (PlayerController2D))]
public class PlayerInput : MonoBehaviour
{
    PlayerController2D controller;

    Vector3 velocity;

    //Range, min and default values can be modified, they are mean to create as smoothly gamefeel 
    [Header("Player movement values")]
    [Min(0)][SerializeField]                float moveSpeed = 10f;
    [Range(0.01f,0.5f)][SerializeField]     float acceleration = 0.15f;
    [Range(0.05f, 0.5f)][SerializeField]    float deceleration = 0.1f;
    [Min(0)][SerializeField]                float accelartionAirboneMultiply = 2f;
    [Min(0)] [SerializeField]               float minJumpHeight = 0.5f;
    [Min(0)][SerializeField]                float maxJumpHeight = 5.5f;
    [Min(0.1f)][SerializeField]             float timeToJumpApex = 0.3f;


    float gravity;
    float minJumpVelocity;
    float maxJumpVelocity;
    float velocityXSmoothing;
    Vector2 input;

    void Start()
    {
        controller = GetComponent<PlayerController2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0")) && controller.collisions.below)
        {
            Jump();
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp("joystick button 0"))
        {
            JumpRelease();
        }

        CalculatePlayerVelocity();
        controller.Move(velocity * Time.deltaTime,input);
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }

        }
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.slidingDownMaxSlope)
        {
            velocity.x = 0;
        }

    }

    void CalculatePlayerVelocity()
    {
        float targetVeloctyX = input.x * moveSpeed;
        if (input.x != 0)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVeloctyX, ref velocityXSmoothing, (controller.collisions.below) ? acceleration : (acceleration * accelartionAirboneMultiply));
        }
        else if (controller.collisions.below)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVeloctyX, ref velocityXSmoothing, deceleration);
        }

        velocity.y += gravity * Time.deltaTime;
    }

    void Jump()
    {
        velocity.y = maxJumpVelocity;
    }

    void JumpRelease()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }
}
