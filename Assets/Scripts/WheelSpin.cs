using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpin : MonoBehaviour
{//Here, I am trying to make the colliders move exactly the same as the mesh, and also have the mesh rotate according to the speed.
 // Start is called once before the first execution of Update after the MonoBehaviour is created
 // 
 
    [System.Serializable]
    public class WheelPair {
        public Transform meshes;
        public WheelCollider WheelCollider;
    }
    public WheelPair[] wheels;


    void Start()
    {   //CollidertoMesh is one-time command that repositions the wheels to their proper wheel. Without this the wheel mesh teleport to the colliders
        CollidertoMesh();
    }

    // Update is called once per frame
    void Update()
    {
        //updatewheels basically gets the position and rotation of the colliders every frame and updates them to be the same as their associated wheels. 
        UpdateWheels();
    }

    void CollidertoMesh()
    {
        foreach (WheelPair wheel in wheels) {
            wheel.WheelCollider.transform.position = wheel.meshes.position;
        }
    }
    void UpdateWheels()
    {
        foreach (WheelPair wheel in wheels)
        {
            Vector3 position;
            Quaternion rotation;

            wheel.WheelCollider.GetWorldPose(out position, out rotation);
            wheel.meshes.position = position;
            wheel.meshes.rotation = rotation;

            

        }
    }
}

