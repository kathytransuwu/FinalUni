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
    private int currentWaypointIndex = 0;

    private void Awake()
    {
        controller = GetComponent<AIController>();
    }

    private void FixedUpdate()
    {
        if(waypointPath == null)
        {
            return;
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

