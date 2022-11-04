using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CubeOrientation orientation;

    float gyro = -0f;
    float accel = -0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        orientation.gyroscopeX = gyro;
        orientation.accelerationX = accel;
    }
}
