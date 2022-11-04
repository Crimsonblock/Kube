using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOrientation : MonoBehaviour
{
    public float accelerationX;
    public float accelerationY;
    public float accelerationZ;

    public float gyroscopeX;
    public float gyroscopeY;
    public float gyroscopeZ;

    private float orientX = 0;
    private float orientY = 0;
    private float orientZ = 0;

    private float deltaT = 0.02f;
    private float k = 0.98f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        orientX = k * (orientX + gyroscopeX * deltaT) + (1 - k) * accelerationX;
        orientY = k * (orientY + gyroscopeY * deltaT) + (1 - k) * accelerationY;
        orientZ = k * (orientZ + gyroscopeZ * deltaT) + (1 - k) * accelerationZ;

        transform.Rotate(orientX, orientY, orientZ);
    }
}
