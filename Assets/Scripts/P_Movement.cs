using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class P_Movement : MonoBehaviour
{
    public GameObject Player;

    // Movement Speeds
    public float speed;
    public float Acceleration;


    [HideInInspector]
    public float maxSpeed;

    // Vector2 positions
    private Vector2 TargetMousePosition;
    private Vector2 CurrentPlayerPosition;
    private Vector2 savedMousePos;
    private float MouseStoppedThreshold = 0.1f;

    // Bools
    private bool MouseStopped;
    private float timer;
    private bool PlayerIsAtMousePosition;

    // Camera
    private Camera cam;


    void Start()
    {
        maxSpeed = speed;
        cam = Camera.main;
    }

    void Update()
    {
        Move(MousePosition(), Acceleration);
    }

    public Vector2 MousePosition()
    {
        var screenPoint = (Vector3)Input.mousePosition;
        screenPoint.z = 10.0f; // distance of the plane from the camera

        Vector2 Mousepos = Camera.main.ScreenToWorldPoint(screenPoint);

        return Mousepos;
    }
    
    // Moving the player (Germ)
    public void Move(Vector2 targetPos, float acceleration)
    {
        // Calculate the distance between the player and the target position
        float distance = Vector2.Distance(Player.transform.position, targetPos);

        // If the mouse is still moving, accelerate the player
        if (distance > MouseStoppedThreshold)
        {
            // Accelerate the player
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            // Move the player towards the target position
            Player.transform.position = Vector2.MoveTowards(Player.transform.position, targetPos, speed * Time.deltaTime);

            // Rotate the player to face the mouse position
            RotateTowards(targetPos);

            // Update the player's state
            PlayerIsAtMousePosition = false;
            MouseStopped = false;
            timer = 0;
        }
        // If the player is close enough to the target position or the mouse has stopped, decelerate the player
        else
        {
            // Calculate the time it takes to stop the player
            float TimeToStop = speed / acceleration;

            // Decelerate the player
            speed = Mathf.Lerp(speed, 0, Time.deltaTime / TimeToStop);

            // Move the player towards the target position
            Player.transform.position = Vector2.MoveTowards(Player.transform.position, targetPos, speed * Time.deltaTime);

            // Update the player's state
            if (distance <= MouseStoppedThreshold)
            {
                speed = 0;
                PlayerIsAtMousePosition = true;
                savedMousePos = targetPos;
            }
            else
            {
                PlayerIsAtMousePosition = false;
            }

            MouseStopped = true;
        }
    }

    private void RotateTowards(Vector2 targetPos)
    {
        Vector2 direction = targetPos - (Vector2)Player.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Player.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}