using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation3D;

public class CubeOrientation : MonoBehaviour
{
    public float DeltaT = 0.02f;
    public float Scalar = 2.5f;
    public bool Pause = true;

    private cOrientation orient;

    // Start is called before the first frame update
    void Start()
    {
        orient = new(DeltaT);
    }

    public void Orientate(RubixData data)
    {
        Vector3 accelerometer = data.accelerometer;
        Vector3 gyroscope = data.gyroscope;
        
        float[] quaternions = orient.KalmanFilter(
            accelerometer.x,
            accelerometer.z,
            accelerometer.y,
            gyroscope.x,
            gyroscope.z,
            gyroscope.y
            );

        if (!Pause)
        {
            Quaternion q = new(
                quaternions[1],
                quaternions[2],
                quaternions[3],
                quaternions[0]);


            transform.rotation = q;
        }
    }

    public void PauseOrientation()
    {
        Pause = !Pause;
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
