using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.Video;
using UnityEditor;

[System.Serializable]
public class RubixData
{
    public Vector3 accelerometer;
    public Vector3 gyroscope;
}


public class ConnectionManager
{
    static List<string> connectedComs = new List<string>();
    SerialPort sp;
    bool connected;
    public ConnectionManager()
    {
        string com = "";
        connectedComs.Sort();
        foreach (string spName in SerialPort.GetPortNames())
        {
            if (spName != "COM1")
            {
                if (connectedComs.BinarySearch(spName) < 0)
                {
                    connectedComs.Add(spName);
                    com = spName;
                    break;
                }
            }
        }


        sp = new SerialPort("\\\\.\\" + com, 9600);

        if (!sp.IsOpen && com != "")
        {
            sp.Open();
            sp.ReadTimeout = 100;
            sp.Handshake = Handshake.None;
            
        }
        connected = sp.IsOpen;
    }
    ~ConnectionManager()
    {
        if (connected)
        {
            connectedComs.Remove(sp.PortName);
            sp.Close();
        }
    }
    public bool isConnected()
    {
        return connected;
    }

    public RubixData getNewData()
    {
        RubixData rubixData = null;

        if (!sp.IsOpen)
        {
            sp.Open();
        }
        if (sp.IsOpen)
        {
            string json = sp.ReadLine();
            rubixData = JsonUtility.FromJson<RubixData>(json);
        }

        return rubixData;
    }

    public bool hasNewData()
    {
        return true;
    }
}
