using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

//Makes it so this script will only run if there is InputManager attached to the car body
[RequireComponent(typeof(Rigidbody))]
public class Carcontroller : MonoBehaviour
{

    //Adding all the public stuff here. Throttlewheels is for applying physics to all the wheels, the steeringWheels is only going to be the front two.
    private IVehicleInput manager;

    [System.Serializable] public struct WheelPair
    {
        public WheelCollider WheelCollider;
        public Transform Mesh;
    }
    [Header("Wheel Settings")]
    public List<WheelPair> throttleWheels;
    public List<WheelPair> steeringWheels;

    [Header("Car Physics Settings")]
    public float maxSpeed = 100f;
    public float maxReverseSpeed = 30f;
    public float maxTurn = 30f;
    public float minTurn = 5f;
    public float maxMotorTorque = 1500f;
    public float maxBrakeTorque = 3000f; //Make sure brakes are 2x ish stronger than torque
    public Transform CoM;
    public Rigidbody rb;

    [Header("Engine Torque Curve")]
    public AnimationCurve torqueCurve = new AnimationCurve(
        new Keyframe(0f, 0.6f),
        new Keyframe(0.3f, 1f),
        new Keyframe(0.7f, 0.8f),
        new Keyframe(1f, 0.3f)
        );


    [Header("Aerodynamics")]
    //This is how much downforce is applied per (m/s)^2 of speed
    public float downforceCoefficient = 2f;
    //Air resistance coefficient, higher means more drag
    public float dragCoefficient = 0.5f;

    [Header("Stability Control")]
    public float minAngularDrag = 0.5f;
    public float maxAngularDrag = 5f;

    [Header("Drift Settings")]
    public float driftSidewaysFriction = 0.7f; //Lower means more drift
    public float normalSidewaysFriction = 1f; //Basically normal friction
    public float driftForwardFriction = 0.6f; //Same as sideways
    public float normalForwardFriction = 1f;
    public float currentAcceleration = 0f;
    public float accelerationRate = 200f;


    [Header("Handbrake")]
    [SerializeField] private float handbrakeTorque = 8000f;

    [Header("Anti-lock Braking")]
    //Prevents the car locking up when emergency braking
    public float absWheelRpmThreshold = 5f; //If the wheel RPM drops below this while braking, we consider it locked up
    public float absMinSpeed = 3f; //Minimum speed for ABS to activate, prevents it from activating when the car is almost stopped
    public float absBrakeReduction = 0.4f; //How much to reduce brake torque when ABS activates in percentage (0.4 means 40% reduction)


    [Header("Traction Control")]
    //Prevents excessive wheel spin during acceleration
    public float tcsSlipRatio = 1.4f; //If the slip ratio exceeds this, we consider it excessive wheel spin
    public float tcsReductionFactor = 0.3f; //How much to apply motor torque when TCS activates in percentage (0.3 means 30% reduction)
    [Header("Suspension Settings")]
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    private bool tcsActive = false;

    private List<WheelCollider> ThrottleColiders
    {
        get
        {
            var list = new List<WheelCollider>();
            foreach (var pair in throttleWheels) list.Add(pair.WheelCollider);
            return list;
        }
    }
    private List<WheelCollider> SteeringColiders
    {
        get
        {
            var list = new List<WheelCollider>();
            foreach (var pair in steeringWheels) list.Add(pair.WheelCollider);
            return list;
        }
    }
    private List<WheelCollider> SteeringColliders
    {
        get
        {
            var list = new List<WheelCollider>();
            foreach (var pair in steeringWheels) list.Add(pair.WheelCollider);
            return list;
        }
    }
    void Start()
    {
        //Essentially just inheriting from IvehicleInput.
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
        Aerodynamics();
        Acceleration();
        Braking();
        handbrake();
        ApplySteeringandFriction();
        UpdateAngularDrag();
        SyncWheelMeshes();
    }

    private void Aerodynamics()
    {

        float speedSquared = rb.linearVelocity.sqrMagnitude;

        //Apply downforce improving grip
        rb.AddForce(-transform.up * downforceCoefficient * speedSquared);

        //Aerodynamics drag opposes the velocity (separate from the rigidbody so we keep control
        rb.AddForce(-rb.linearVelocity.normalized * dragCoefficient * speedSquared);
    }

    private void UpdateAngularDrag()
    {
        //Do speed scale to prevent spin outs

        float speedFactor = Mathf.InverseLerp(0f, maxSpeed, rb.linearVelocity.magnitude);

        rb.angularDamping = Mathf.Lerp(minAngularDrag, maxAngularDrag, speedFactor);
    }

    private void ApplySteeringandFriction()
    {

        bool isDrifting = manager.isDrifting;

        {
            foreach (var wheel in steeringWheels)
            {
                applyDriftFriction(wheel.WheelCollider, isDrifting);
                float currentSpeed = rb.linearVelocity.magnitude;
                float speedFactor = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);
                float turnAngle = Mathf.Lerp(maxTurn, minTurn, speedFactor);

                wheel.WheelCollider.steerAngle = manager.steering * turnAngle;

            }
            foreach (var wheel in throttleWheels)
            {
                applyDriftFriction(wheel.WheelCollider, isDrifting);
                
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

        bool movingForward = Vector3.Dot(rb.linearVelocity, transform.forward) >= 0;

        float throttleInput = manager.throttle;

        if (movingForward && throttleInput > 0f && currentSpeed >= maxSpeed)
        {
            throttleInput = 0f;
        }
        else if (!movingForward && throttleInput < 0f && currentSpeed >= maxReverseSpeed)
        {
            throttleInput = 0f;
        }

        //Torque curve applied to forward drive only
        float curveMultiplier = 1f;
        if (throttleInput > 0f)
        {
            float speedNormalized = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);
            curveMultiplier = torqueCurve.Evaluate(speedNormalized);
        }

        float targetTorque = maxMotorTorque * throttleInput * curveMultiplier;

        tcsActive = false;
        if (throttleInput > 0f)
        {
            float expectedWheelRpm = (currentSpeed / (2f * Mathf.PI * wheelRadius)) * 60f;

            foreach (var wheel in throttleWheels)
            {
                if (Mathf.Abs(wheel.WheelCollider.rpm) > expectedWheelRpm * tcsSlipRatio)
                {
                    tcsActive = true;
                    break;
                }
            }
        }
        if (tcsActive) targetTorque *= tcsReductionFactor;

        currentAcceleration = Mathf.MoveTowards(currentAcceleration, targetTorque, accelerationRate * Time.fixedDeltaTime);

        foreach (var wheel in throttleWheels)
        {
            wheel.WheelCollider.motorTorque = currentAcceleration;
        }
    }
    private void Braking()
    {
        float currentSpeed = rb.linearVelocity.magnitude;
        bool movingForward = Vector3.Dot(rb.linearVelocity, transform.forward) > 0;

        float brakeTorque = 0f;

        if (manager.throttle == 0f)
        {
            //Gentle drag
            brakeTorque = maxBrakeTorque * 0.1f;
        }
        else if (movingForward && manager.throttle < 0f)
        {
            //Braking while moving forward
            brakeTorque = maxBrakeTorque;
        }
        else if (!movingForward && manager.throttle > 0f)
        {
            //Braking while moving backward
            brakeTorque = maxBrakeTorque;
        }
        if (brakeTorque > 0f && currentSpeed > absMinSpeed)
        {
            foreach (var wheel in throttleWheels)
            {
                if (Mathf.Abs(wheel.WheelCollider.rpm) < absWheelRpmThreshold)
                {
                    brakeTorque *= (1f - absBrakeReduction);
                }
            }
        }
        foreach (var wheel in throttleWheels)
        {
            wheel.WheelCollider.brakeTorque = brakeTorque;
        }
        foreach (var wheel in steeringWheels)
        {
            wheel.WheelCollider.brakeTorque = brakeTorque;
        }
    }
    private void handbrake()
    {
        //Only lock the rear throttle wheels. Steering wheels stay free to allow for drifting 

        float hbTorque = manager.isDrifting ? handbrakeTorque : 0f;

        foreach (var wheel in throttleWheels)
        {
            wheel.WheelCollider.brakeTorque += hbTorque;
        }
    }
    private void SyncWheelMeshes()
    {
        Syncgroup(throttleWheels);
        Syncgroup(steeringWheels);
    }
    private void Syncgroup(List<WheelPair> wheelPairs)
    {
        foreach (var wheel in wheelPairs)
        {
            if (wheel.Mesh == null)
            {
                continue;
            }
            wheel.WheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.Mesh.SetPositionAndRotation(pos, rot);

        }
    }
}


