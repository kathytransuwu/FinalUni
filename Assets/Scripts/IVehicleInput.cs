using UnityEngine;

public interface IVehicleInput
{
    public float throttle { get; }
    public float steering{ get; }
    public bool isDrifting { get; }
}
