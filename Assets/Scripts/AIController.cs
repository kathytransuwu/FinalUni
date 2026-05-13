using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, IVehicleInput
{
    //Needs to be separated so the AI doesn't use inputs from the InputManager. Both get the throttle, steering and isDrifting from that tho but they're like their own versions of it. 
    public float throttle { get; private set; }
    public float steering { get; private set; }
    public bool isDrifting { get; private set; } = false;

    public void SetInputs(float forwardAmount, float turnAmount)
    {

        throttle = forwardAmount;
        steering = turnAmount;
    }
}
