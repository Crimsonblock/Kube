using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
<<<<<<< HEAD

    private ConnectionManager arduino;

=======
    public CubeOrientation orientation;

    float gyro = -0f;
    float accel = -0f;
>>>>>>> 851a7e0fae07fd7e6baff475160abaf083831953

    // Start is called before the first frame update
    void Start()
    {
        arduino = new ConnectionManager();
        if (!arduino.isConnected()) arduino = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
<<<<<<< HEAD
        if (arduino != null && arduino.hasNewData() )
        {
            RubixData data1 = arduino.getNewData();
        }
=======
        orientation.gyroscopeX = gyro;
        orientation.accelerationX = accel;
>>>>>>> 851a7e0fae07fd7e6baff475160abaf083831953
    }
}
