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
        }
    }
}
