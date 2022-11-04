using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private ConnectionManager arduino;


    // Start is called before the first frame update
    void Start()
    {
        arduino = new ConnectionManager();
        if (!arduino.isConnected()) arduino = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (arduino != null && arduino.hasNewData() )
        {
            RubixData data1 = arduino.getNewData();
            /*
               Debug.Log("xA: " + data1.accelerometer.x +
                        "yA: " + data1.accelerometer.y +
                        "zA: " + data1.accelerometer.z+
                        "xG: " + data1.gyroscope.x+
                        "yG: " + data1.gyroscope.y+
                        "zG: " + data1.gyroscope.z);
            */
        }
    }
}
