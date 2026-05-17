using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class CarDriverAI : MonoBehaviour
{
    [SerializeField] private WaypointPath waypointPath;
   [SerializeField] private float waypointRadius = 3f; // The radius within which the AI considers it has reached a waypoint
    private AIController controller;
    private Rigidbody rb;

    private int currentWaypointIndex = 0;

    [Header("Stuck failsafe")]
    [SerializeField] private float stuckTimeLimit = 5f; // Time in seconds before the AI considers itself stuck
    [SerializeField] private float stuckSpeedThreshold = 0.5f; //Below this speed, the AI considers itself stuck
    [SerializeField] private float recoveryTime = 2f; // Time in seconds to brake before retrying

    private float stuckTimer = 0f;
    private bool isRecovering = false;
    private float recoveryTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<AIController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(waypointPath == null)
        {
            return;
        }

        if (!isRecovering)
        {
            //If barely moving, start the stuck timer
            if (rb.linearVelocity.magnitude < stuckSpeedThreshold)
            {
                stuckTimer += Time.fixedDeltaTime;
            }
            else
            {
                stuckTimer = 0f; // Reset the timer if the AI is moving fast enough
            }

            if (stuckTimer >= stuckTimeLimit)
            {
                isRecovering = true;
                recoveryTimer = recoveryTime; // Start the recovery timer
                stuckTimer = 0f; // Reset the stuck timer
            }
        }

        //If in recovery mode, brake for the recovery time before trying to move again
        if (isRecovering)
        {
            recoveryTimer -= Time.fixedDeltaTime;
            controller.SetInputs(0f, 0f); // Brake, the carcontroller script will apply the brake logic to the car 

            if(recoveryTimer <= 0f && rb.linearVelocity.magnitude < 0.5f)
            {
                isRecovering = false; // End recovery mode and try to move again
                currentWaypointIndex = (currentWaypointIndex + 1) % waypointPath.length; // Move to the next waypoint to try a different path
            }
            return; // Skip the rest of the logic while recovering
        }

        Transform target = waypointPath.getWaypoints(currentWaypointIndex); //Set the target to the current waypoint

        float distanceToWaypoint = Vector3.Distance(transform.position, target.position);

        if (distanceToWaypoint < waypointRadius)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypointPath.length)
            {
                currentWaypointIndex = 0; // Loop back to the first waypoint
            }
        }
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float checkToTarget = Vector3.Dot(transform.forward, dirToTarget);
        float forwardAmount = checkToTarget > 0 ? 1f : -1f; // If the target is in front of the AI, go forward, otherwise go backward

        float angleToTarget = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.up);
        float turnAmount = Mathf.Clamp(angleToTarget / 45f, -1f, 1f); // Normalize the angle to a value between -1 and 1 for steering

        controller.SetInputs(forwardAmount, turnAmount);
    }
    private void OnDrawGizmos()
    {
        if (waypointPath == null)
        {
            return;
        }
        // Draw a sphere at the current waypoint for visualization
        Transform target = waypointPath.getWaypoints(currentWaypointIndex);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(target.position, waypointRadius);
    }
}

