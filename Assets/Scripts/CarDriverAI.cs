using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarDriverAI : MonoBehaviour
{
    [SerializeField] private Transform targetPositionTransform;

    private AIController controller;
    private Vector3 targetPosition;

    private void Awake()
    {
        controller = GetComponent<AIController>();
    }

    private void FixedUpdate()
    {
        //Sets the position of the target to the position of the targetPositionTransform, which is a gameobject that I can move around to make the AI drive towards it.
        SetTargetPosition(targetPositionTransform.position);
        //Speed and turn of the AI car
        float forwardAmount = 0f;
        float turnAmount = 0f;

        //Checks the position of the targetTest object compared to the position of the car. If its positive the target is in front of the car, if its negative its behind the car.
        Vector3 dirToMovePosition = (targetPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToMovePosition);

        if (dot > 0)
        {
            forwardAmount = 1f; // Move forward
        }
        else
        {
            forwardAmount = -1f; // Move backward
        }

        Debug.Log(dot);

        float angleToTarget = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);

        if (angleToTarget > 0)
        {
            turnAmount = 1f; // Turn right
        }
        else
        {
            turnAmount = -1f; // Turn left
        }

        
        //AI logic for controlling the car. 
        controller.SetInputs(forwardAmount, turnAmount);

        Debug.Log($"Throttle: {controller.throttle}, Steering: {controller.steering}");
    }
    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

}
