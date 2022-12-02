using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation3D;

public class CubeOrientation : MonoBehaviour
{
    public enum FilterType
    {
        Kalman,
        KalmanExrended,
        Complementary
    }

    public float DeltaT = 0.02f;
    public bool Pause = true;

    public FilterType Filter = FilterType.Complementary;

    public float Alpha = 0.98f;
    public float R = 0.01f;
    public float Q_quaternion = 0.00001f;
    public float Q_quatBias = 0.00001f;

    public float gyroscopeScale = 1000.0f;

    private cOrientation orient;

    // Start is called before the first frame update
    void Start()
    {
        orient = new(DeltaT);
    }

    public void Orientate(RubixData data)
    {
        Vector3 accelerometer = data.accelerometer;
        Vector3 gyroscope = data.gyroscope / gyroscopeScale;

        orient.setAlpha(Alpha);
        orient.setR(R);
        orient.setQquaternion(Q_quaternion);
        orient.setQquatbias(Q_quatBias);

        float[] quaternions = new float[] { 0, 0, 0, 0 };

        if (Filter == FilterType.Complementary)
        {
            quaternions = orient.ComplementaryFilter(
                accelerometer.x, accelerometer.y, accelerometer.z,
                gyroscope.x, gyroscope.y, gyroscope.z
                );
        }
        else if (Filter == FilterType.Kalman)
        {
            quaternions = orient.KalmanFilter(
                accelerometer.x, accelerometer.y, accelerometer.z,
                gyroscope.x, gyroscope.y, gyroscope.z
                );
        }
        else if (Filter == FilterType.KalmanExrended)
        {
            quaternions = orient.KalmanFilterBias(
                accelerometer.x, accelerometer.y, accelerometer.z,
                gyroscope.x, gyroscope.y, gyroscope.z
                );
        }

        if (!Pause)
        {
            Quaternion q = new(
                -quaternions[1],
                -quaternions[3],
                quaternions[2],
                quaternions[0]);


            transform.rotation = q;
        }
    }

    public void PauseOrientation()
    {
        Pause = !Pause;
        if (Pause)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
