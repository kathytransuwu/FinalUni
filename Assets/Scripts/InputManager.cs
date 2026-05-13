using UnityEngine;

public class InputManager : MonoBehaviour, IVehicleInput
{
    public float throttle { get; private set; }
    public float steering { get; private set; }
    public bool isDrifting { get; private set; }
    void Update()
        {
            //Makes it so "W" and "S" keys control the throttle. "D" and "A" control the steering
            throttle = Input.GetAxis("Vertical");
            steering = Input.GetAxis("Horizontal");
            isDrifting = Input.GetKey(KeyCode.Space);
        
        Debug.Log($"Throttle: {throttle}, Steering: {steering}, IsDrifting: {isDrifting}");
    }
    }


