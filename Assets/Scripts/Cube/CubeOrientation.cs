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

    public int speed = 50;

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
            float max = Mathf.Abs(accelerometer.x);
            char gravity = 'x';
            
            if (Mathf.Abs(accelerometer.y) > max)
            {
                max= Mathf.Abs(accelerometer.y);
                gravity = 'y';
            }
            if (Mathf.Abs(accelerometer.z) > max)
            {
                gravity = 'z';
            }

            if (gravity == 'x')
            {
                Quaternion q = new(
                    -quaternions[3],
                    -quaternions[1],
                    quaternions[2],
                    quaternions[0]
                    );


                transform.rotation = q;
            }
            else if (gravity == 'y')
            {
                Quaternion q = new(
                    -quaternions[1],
                    -quaternions[2],
                    quaternions[3],
                    quaternions[0]
                    );


                transform.rotation = q;
            }
            else if (gravity == 'z')
            {
                Quaternion q = new(
                    -quaternions[1],
                    -quaternions[3],
                    quaternions[2],
                    quaternions[0]
                    );


                transform.rotation = q;
            }
        }
        else // keyboard inputs
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Rotate(-Vector3.forward * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.left * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(-Vector3.left * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(Vector3.up * speed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(-Vector3.up * speed * Time.deltaTime);
            }
        }
    }

    public void TogglePause()
    {
        Pause = !Pause;
        if (Pause)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
