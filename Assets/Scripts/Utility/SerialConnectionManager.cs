using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using UnityEngine;

public class SerialConnectionManager
{

    bool hasNewImuData = false;
    bool hasNewFacesData = false;
    SerialPort sp;

    Vector3 accel = Vector3.one;
    Vector3 gyro = Vector3.one;

    RotaryEncoder rotData;


    // Start is called before the first frame update
    public SerialConnectionManager()
    {
        foreach (string port in SerialPort.GetPortNames())
        {
            Debug.Log(port);
            sp = new SerialPort("\\\\.\\" + "COM4", 9600);
            break;
        }
        if (sp!= null && !sp.IsOpen)
        {
            Debug.Log("Opening " + ", baud 9600");
            sp.Open();
            sp.ReadTimeout = 100;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { Debug.Log("Open"); }
        }
    }

    ~SerialConnectionManager()
    {
        sp.Close();
        sp = null;
    }

    public void Update()
    {
        if (sp != null && !sp.IsOpen) sp.Open();
        if (sp != null && sp.IsOpen)
        {
            string result = sp.ReadLine();
            if (result.StartsWith("i")){
                result = result.Remove(0, 1);

                string[] inputvec = result.Split(' ');
                for (int i = 0; i < inputvec.Length; i++)
                    if (inputvec[i] == "") return;

                accel.x = float.Parse(inputvec[0], new CultureInfo("en-US").NumberFormat);
                accel.y = float.Parse(inputvec[2], new CultureInfo("en-US").NumberFormat);
                accel.z = float.Parse(inputvec[1], new CultureInfo("en-US").NumberFormat);

                gyro.x = float.Parse(inputvec[3], new CultureInfo("en-US").NumberFormat);
                gyro.y = float.Parse(inputvec[5], new CultureInfo("en-US").NumberFormat);
                gyro.z = float.Parse(inputvec[4], new CultureInfo("en-US").NumberFormat);

                hasNewImuData = true;
            }
            else if(result.StartsWith("r"))
            {
                rotData = new();

                result = result.Remove(0, 1);
                string[] inputvec = result.Split(' ');
                for (int i = 0; i < inputvec.Length; i++)
                    if (inputvec[i] == "") return;
                Debug.Log(inputvec[0]);

                rotData.front = int.Parse(inputvec[1]);
                rotData.back = int.Parse(inputvec[0]);
                rotData.left = int.Parse(inputvec[2]);
                rotData.right = int.Parse(inputvec[3]);
                rotData.top = int.Parse(inputvec[4]);
                rotData.bottom = int.Parse(inputvec[5]);

                Debug.Log(rotData.front + " " + rotData.back + " " + rotData.left + " " + rotData.right + "  " + rotData.top + " " + rotData.bottom);

                hasNewFacesData = true;
            }
        }
    }

    public bool hasNewImu() { return hasNewImuData; }
    public bool hasNewFaces() { return hasNewFacesData; }

    public Vector3[] getImuData()
    {
        if (hasNewImuData)
        {
            hasNewImuData = false;
            Vector3[] result = { accel, gyro };
            return result;
        }
        else return null;
    }

    public RotaryEncoder getFaces()
    {
        if (hasNewFacesData)
        {
            hasNewFacesData = false;
            return rotData;
        }
        else return null;
    }
}
