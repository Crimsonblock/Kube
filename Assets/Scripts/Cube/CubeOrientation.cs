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
    public float Alpha = .98f;

    public float AngleDeviation = 0;

    private cOrientation orient;

    bool rubixReady = false;
    bool wasCompensated = false;

    // Start is called before the first frame update
    void Start()
    {
        orient = new(DeltaT);
        orient.setAlpha(Alpha);

        Quaternion q = Quaternion.Euler(0, AngleDeviation, 0);
        float[] libQ = { q.w, q.z, q.y, q.x };
        orient.setOrientation(libQ);

    }

    private void Update()
    {
        if(rubixReady && !wasCompensated)
        {
            wasCompensated = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).Rotate(0, AngleDeviation, 0, Space.Self);
            }
        }
    }

    public void setRotation(Quaternion q)
    {
        float[] libQ = { q.w, q.z, q.y, q.x };
        orient.setOrientation(libQ);
    }


    public Quaternion Orientate(Vector3 []data)
    {

        //orient.setElapsedTime(DeltaT);

        Vector3 accelerometer = data[0];
        Vector3 gyroscope = data[1];


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

    public float getAngleDeviation() { return AngleDeviation; }

    public void isRubixReady(bool setting)
    {
        rubixReady = setting;
    }
}