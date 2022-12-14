using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orientation3D;
using System.CodeDom.Compiler;

public class CubeOrientation : MonoBehaviour
{
    public enum FilterType
    {
        Kalman,
        KalmanExrended,
        Complementary
    }

    public float DeltaT = 0.03f;
    public bool Pause = false;

    public FilterType Filter = FilterType.Complementary;

    public float Alpha = 0.98f;
    public float R = 0.01f;
    public float Q_quaternion = 0.00001f;
    public float Q_quatBias = 0.00001f;

    public float gyroscopeScale = 1000.0f;

    public int speed = 50;

    private cOrientation orient;

    // Start is called before the first frame update
    void Start()
    {
        orient = new(DeltaT);
    }

    public void Orientate(RubixData data)
    {
        Vector3 accelerometer = data.accelerometer == Vector3.zero ? new Vector3(0.01f, 0.01f, 0.01f) : data.accelerometer ;
        Vector3 gyroscope = data.gyroscope == Vector3.zero ? new Vector3(0.01f, 0.01f, 0.01f) :  data.gyroscope / gyroscopeScale;

        orient.setElapsedTime(DeltaT);
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
        Quaternion q = new(
                    quaternions[3],
                    quaternions[2],
                    quaternions[1],
                    quaternions[0]
                    );

        q = Quaternion.Inverse(q);

        Vector3 angle = q.eulerAngles;
        angle.x = -angle.x;

        Quaternion quat = Quaternion.Euler(angle);

        transform.SetLocalPositionAndRotation(transform.position, quat);

    }

    public void TogglePause()
    {
        Pause = !Pause;
        if (Pause)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}