using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private ConnectionManager arduino;
    public CubeOrientation orientation;
    public UpdateText updateText;

    // Start is called before the first frame update
    void Start()
    {
        arduino = new ConnectionManager();
        if (!arduino.isConnected()) arduino = null;
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
                        "fr: " + data1.rotaryEncoder.front.ToString() + "\t\t" +
                        "ba: " + data1.rotaryEncoder.back.ToString() + "\t\t" +
                        "le: " + data1.rotaryEncoder.left.ToString() + "\n" +
                        "ri: " + data1.rotaryEncoder.right.ToString() + "\t\t" +
                        "to: " + data1.rotaryEncoder.top.ToString() + "\t\t" +
                        "bo: " + data1.rotaryEncoder.bottom.ToString();

                updateText.setText(text);
            }
        }
    }
}
