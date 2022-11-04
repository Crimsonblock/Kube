using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOrientation : MonoBehaviour
{
    private float orientX = 0;
    private float orientY = 0;
    private float orientZ = 0;

    private float deltaT = 0.02f;
    private float k = 0.999999f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void orientate(RubixData data)
    {
        Vector3 accelerometer = data.accelerometer;
        Vector3 gyroscope = data.gyroscope;

        orientX = k * (gyroscope.x * deltaT) + (1 - k) * accelerometer.x;
        orientY = k * (gyroscope.y * deltaT) + (1 - k) * accelerometer.y;
        orientZ = k * (gyroscope.z * deltaT) + (1 - k) * accelerometer.z;

        transform.Rotate(orientX, orientY, orientZ);
    }
}
