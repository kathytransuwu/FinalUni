using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //This is the thing that will let us focus on the vehicle the entire time.
    public GameObject focus;
    //This one is how far the camera is from the vehicle.
    public float distance = 2f;
    //This is the height from the vehicle.
    public float height = 2f;
    public float dampening = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Changes the position of the camera. 
        //Lerp is a linear interpreter. A, B and C, A is the location at the start of the transform. B is where we move it to and C is the time it takes between them.
        transform.position = Vector3.Lerp(transform.position, focus.transform.position + focus.transform.TransformDirection(new Vector3(0f, height, -distance)), dampening/2 * Time.deltaTime);
        transform.LookAt(focus.transform);
    }
}