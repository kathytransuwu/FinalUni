using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//Makes it so this script will only run if there is InputManager attached to the car body
[RequireComponent(typeof(Rigidbody))]
public class Carcontroller : MonoBehaviour
{

    //Adding all the public stuff here. Throttlewheels is for applying physics to all the wheels, the steeringWheels is only going to be the front two.
    private IVehicleInput manager;
    public List<WheelCollider> throttleWheels;
    public List<WheelCollider> steeringWheels;

    [Header("Car Physics Settings")]
    public float maxSpeed = 100f;
    public float maxTurn = 30f;
    public float minTurn = 5f;
    public float maxMotorTorque = 1500f;
    public float maxBrakeTorque = 3000f; //Make sure brakes are 2x ish stronger than torque
    public Transform CoM;
    public Rigidbody rb;

    [Header("Drift Settings")]
    public float driftSidewaysFriction = 0.7f; //Lower means more drift
    public float normalSidewaysFriction = 1f; //Basically normal friction
    public float driftForwardFriction = 0.6f; //Same as sideways
    public float normalForwardFriction = 1f;
    public float currentAcceleration = 0f;
    public float accelerationRate = 200f;
    [SerializeField] private Transform[] rayPoints;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    void Start()
    {
        //Essentially just inheriting from IvehicleInput. The 
        manager = GetComponent<IVehicleInput>();
        rb = GetComponent<Rigidbody>();

        //This is to be able to manipulate the Center of Mass using a gameobject, to make it easier on me lol. 
        //Note: Use localPosition for this otherwise it uses the global position which is like 323 on the x-axis for the CoM. Write about this on the assignment.
        if (CoM)
        {
            rb.centerOfMass = CoM.localPosition;
        }

    }
    void FixedUpdate()
    {
        Suspension();
        Acceleration();
        Braking();

        bool isDrifting = manager.isDrifting;

        {
            foreach (WheelCollider wheel in steeringWheels)
            {
                applyDriftFriction(wheel, isDrifting);
                float currentSpeed = rb.linearVelocity.magnitude;
                float speedFactor = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);
                float turnAngle = Mathf.Lerp(maxTurn, minTurn, speedFactor);
                wheel.steerAngle = manager.steering * turnAngle;

            }
            foreach (WheelCollider wheel in throttleWheels)
            {
                applyDriftFriction(wheel, isDrifting);
            }
        }
        void applyDriftFriction(WheelCollider wheel, bool isDrifting)
        {
            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            if (isDrifting)
            {
                sidewaysFriction.stiffness = driftSidewaysFriction;
                forwardFriction.stiffness = driftForwardFriction;
            }
            else
            {
                sidewaysFriction.stiffness = normalSidewaysFriction;
                forwardFriction.stiffness = normalForwardFriction;
            }
            wheel.sidewaysFriction = sidewaysFriction;
            wheel.forwardFriction = forwardFriction;
        }
    }
    private void Suspension()
    {
        foreach (Transform rayPoint in rayPoints)
        {
            RaycastHit hit;
            float MaxLength = restLength + springTravel;


            if (Physics.Raycast(rayPoint.position, -rayPoint.up, out hit, MaxLength + wheelRadius))
            {
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = restLength - currentSpringLength;

                float springVelocity = Vector3.Dot(rb.GetPointVelocity(rayPoint.position), rayPoint.up);
                float springForce = springCompression * springStiffness;
                float dampForce = damperStiffness * springVelocity;
                float netForce = springForce - dampForce;

                rb.AddForceAtPosition(netForce * rayPoint.up, rayPoint.position);

                Debug.DrawLine(rayPoint.position, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(rayPoint.position, rayPoint.position + (wheelRadius + MaxLength) * -rayPoint.up, Color.green);
            }
        }
    }

    private void Acceleration()
    {

        float currentSpeed = rb.linearVelocity.magnitude; 

        float throttleInput = currentSpeed < maxSpeed ? manager.throttle : 0f; 

        float targetTorque = maxMotorTorque * throttleInput;


        currentAcceleration = Mathf.MoveTowards(currentAcceleration,targetTorque,accelerationRate * Time.fixedDeltaTime);

        
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.motorTorque = currentAcceleration;
        }
    }
    private void Braking()
    {
        //Brake when throttle (Or player input) is 0 or pressing S (Opposite direction) whilst moving
        float currentSpeed = rb.linearVelocity.magnitude;
        bool movingForward = Vector3.Dot(rb.linearVelocity, transform.forward) > 0;

        float brakeTorque = 0f;

        //This happens after the player lets go of the input
        if (manager.throttle == 0f)
        {
            brakeTorque = maxBrakeTorque * 0.1f;
        }
        //If the player holds S whilst moving forward
        else if(movingForward && manager.throttle < 0f)
        {
            brakeTorque = maxBrakeTorque;
        }
        //Moving backwards but press forwards to full brake
        else if(!movingForward && manager.throttle > 0f)
        {
            brakeTorque = maxBrakeTorque;
        }
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.brakeTorque = brakeTorque;
        }
        foreach (WheelCollider wheel in steeringWheels)
        {
            wheel.brakeTorque = brakeTorque;
        }
    }
}


