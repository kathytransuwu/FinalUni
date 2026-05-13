using UnityEngine;

public class Rotate_CarSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        transform.Rotate(0, 0.5f, 0);
    }
}
