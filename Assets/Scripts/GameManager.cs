using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private ConnectionManager arduino;
    public CubeOrientation orientation;
    public UpdateText updateText;
    private BleManager bleManager = null;

    // Start is called before the first frame update
    void Start()
    {
        // Set a first parameter to true to get .NET Library log in unity
        // Set a second parameter to true to get Native library log in unity
        bleManager = BleManager.getInstance();
        bleManager.startScan();
        bleManager.destroy();
        bleManager = null;
    }

    ~GameManager()
    {
       if(bleManager != null) bleManager.destroy();
       arduino = null;
    }

    public ConnectionManager getConnectionManager()
    {
        if (bleManager == null) return null;
        return bleManager.getConnectionManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (arduino != null && arduino.hasNewData() )
        {
            RubixData data1 = arduino.getNewData();
            if (data1 != null)
            {
                orientation.Orientate(data1);

                string text = "xA: " + data1.accelerometer.x.ToString("0.00") + "\t\t" +
                        "yA: " + data1.accelerometer.y.ToString("0.00") + "\t\t" +
                        "zA: " + data1.accelerometer.z.ToString("0.00") + "\n" +
                        "xG: " + data1.gyroscope.x.ToString("0.00") + "\t\t" +
                        "yG: " + data1.gyroscope.y.ToString("0.00") + "\t\t" +
                        "zG: " + data1.gyroscope.z.ToString("0.00") + "\n" +
                        "fr: " + data1.rotation.front.ToString() + "\t\t" +
                        "ba: " + data1.rotation.back.ToString() + "\t\t" +
                        "le: " + data1.rotation.left.ToString() + "\n" +
                        "ri: " + data1.rotation.right.ToString() + "\t\t" +
                        "to: " + data1.rotation.top.ToString() + "\t\t" +
                        "bo: " + data1.rotation.bottom.ToString();

                updateText.setText(text);
            }
        }
        else if(arduino == null && bleManager != null)
        {
            arduino = getConnectionManager();
        }
        if(bleManager != null) bleManager.update();
    }


}
