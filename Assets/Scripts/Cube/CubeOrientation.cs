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

    public int speed = 10;

    public float test = 10;
    public float gyroScale = 1 ;
    public float Alpha = .98f;

    private cOrientation orient;

    // Start is called before the first frame update
    void Start()
    {
        orient = new(DeltaT);
        orient.setAlpha(Alpha);
    }

    public Quaternion Orientate(RubixData data)
    {

        //orient.setElapsedTime(DeltaT);

        Vector3 accelerometer = data.accelerometer;
        Vector3 gyroscope = data.gyroscope;


        var quaternions = orient.ComplementaryFilter(
            accelerometer.x, accelerometer.y, accelerometer.z,
            gyroscope.x, gyroscope.y, gyroscope.z);
        Quaternion q = new(
                    quaternions[3],
                    quaternions[2],
                    quaternions[1],
                    quaternions[0]
                    );

        q = Quaternion.Inverse(q);

        /*Vector3 angle = q.eulerAngles;
        angle.x = -angle.x;

        Quaternion quat = Quaternion.Euler(angle);*/
        // transform.rotation = q;
        return q;

    }

    public void TogglePause()
    {
        Pause = !Pause;
        if (Pause)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }
}