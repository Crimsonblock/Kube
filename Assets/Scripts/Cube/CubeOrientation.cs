using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOrientation : MonoBehaviour
{
    public float orientX = 0;
    public float orientY = 0;
    public float orientZ = 0;

    public float deltaT = 0.02f;
    public float k = 0.98f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void orientate(RubixData data)
    {
        Vector3 accelerometer = data.accelerometer;
        Vector3 gyroscope = data.gyroscope;

        orientX = k * (orientX + gyroscope.x * deltaT) + (1 - k) * accelerometer.x;
        orientY = k * (orientY + gyroscope.y * deltaT) + (1 - k) * accelerometer.y;
        orientZ = k * (orientZ + gyroscope.z * deltaT) + (1 - k) * accelerometer.z;

        // Z axis in arduio is vertical axis
        transform.eulerAngles = new Vector3(orientX, orientZ, orientY);
    }
}
