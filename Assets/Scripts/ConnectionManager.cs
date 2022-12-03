using UnityEngine;

[System.Serializable]
public class RubixData
{
    public Vector3 accelerometer;
    public Vector3 gyroscope;
    public RotaryEncoder rotaryEncoder;

    public RubixData()
    {
        accelerometer = Vector3.zero;
        gyroscope = Vector3.zero;
        rotaryEncoder = new RotaryEncoder(0, 0, 0, 0, 0, 0);
    }
}

[System.Serializable]
public class RotaryEncoder
{
    public int front { get; set; }
    public int back { get; set; }
    public int left { get; set; }
    public int right { get; set; }
    public int top { get; set; }
    public int bottom { get; set; }

    public RotaryEncoder(int front, int back, int left, int right, int top, int bottom)
    {
        this.front = front;
        this.back = back;
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }
}

public class ConnectionManager
{

    int bleHandle = -1;
    GattCallbacks callbacks;
    bool debug;
    RubixData data;
    bool newDataReceived = false;
    LibCallback onFinished;


    public ConnectionManager(int bleHandle, bool debug)
    {
        this.bleHandle = bleHandle;
        this.debug = debug;
        data = new RubixData();

        BleManager manager = BleManager.getInstance();
        callbacks = createCallbacks();
        onFinished = onCallbacksFinished;
        manager.bindConnectionHandlerToDevice(bleHandle, callbacks, onFinished);
    }


    void onCallbacksFinished(int result)
    {
        switch (result)
        {
            case 0:
                if(debug) 
                    Debug.Log("Successfully registered the callbacks");
                break;
            case -1:
                if (debug)
                    Debug.Log("Invalid device handle");
                break;
            default:
                if (debug)
                    Debug.Log("Unknown error happened");
                break;
        }
    }

    ~ConnectionManager()
    {
        if(debug) 
            Debug.Log("Destructing connection manager: "+bleHandle);
    }

    private GattCallbacks createCallbacks()
    {
        GattCallbacks callbacks = new GattCallbacks();
        callbacks.accel = newAccel;
        callbacks.gyro = newGyro;

        callbacks.encoderLeft = null;
        callbacks.encoderRight = null;
        callbacks.encoderFront = null;
        callbacks.encoderBack = null;
        callbacks.encoderTop = null;
        callbacks.encoderBottom = null;

        return callbacks;
    }
    private void newAccel(float x, float y, float z)
    {
        this.data.accelerometer.x = x;
        this.data.accelerometer.y = y;
        this.data.accelerometer.z = z;
        newDataReceived = true;
    }
    private void newGyro(float x, float y, float z)
    {
        this.data.gyroscope.x = x;
        this.data.gyroscope.y = y;
        this.data.gyroscope.z = z;
        newDataReceived = true;
    }
    public bool isConnected()
    {
        return true;
    }

    public RubixData getNewData()
    {
        newDataReceived = false;
        return data;
    }

    public bool hasNewData()
    {
        return newDataReceived;
    }
}
