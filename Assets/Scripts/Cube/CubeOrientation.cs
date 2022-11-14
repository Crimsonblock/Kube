using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOrientation : MonoBehaviour
{
    public float orientX = 0;
    public float orientY = 0;
    public float orientZ = 0;

    public float deltaT = 0.02f;
    public float k;
    public float scalar = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Orientate(RubixData data)
    {
        Vector3 accelerometer = data.accelerometer;
        Vector3 gyroscope = data.gyroscope / scalar;

        orientX = k * (orientX + gyroscope.x * deltaT) + (1 - k) * accelerometer.x;
        orientY = k * (orientY + gyroscope.y * deltaT) + (1 - k) * accelerometer.y;
        orientZ = k * (orientZ + gyroscope.z * deltaT) + (1 - k) * accelerometer.z;

        // Z axis in arduio is vertical axis
        transform.eulerAngles = new Vector3(orientX, -orientZ, -orientY);
    }

    public void ResetOrientation()
    {
        orientX = 0;
        orientZ = 0;
        orientY= 0;

        transform.eulerAngles = new Vector3(orientX, orientZ, orientY);
    }
}
