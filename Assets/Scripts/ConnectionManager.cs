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

        callbacks.accelerometerX = newXAccel;
        callbacks.accelerometerY = newYAccel;
        callbacks.accelerometerZ = newZAccel;

        callbacks.gyroscopeX = newXGyro;
        callbacks.gyroscopeY = newYGyro;
        callbacks.gyroscopeZ = newZGyro;

        callbacks.encoderLeft = null;
        callbacks.encoderRight = null;
        callbacks.encoderFront = null;
        callbacks.encoderBack = null;
        callbacks.encoderTop = null;
        callbacks.encoderBottom = null;

        return callbacks;
    }
    private void newXAccel(float data)
    {
        this.data.accelerometer.x = data;
        newDataReceived = true;
    }
    private void newYAccel(float data)
    {
        this.data.accelerometer.y = data;
        newDataReceived = true;
    }
    private void newZAccel(float data)
    {
        this.data.accelerometer.z = data;
        newDataReceived = true;
    }
    private void newXGyro(float data)
    {
        this.data.gyroscope.x = data;
        newDataReceived = true;
    }
    private void newYGyro(float data)
    {
        this.data.gyroscope.y = data;
        newDataReceived = true;
    }
    private void newZGyro(float data)
    {
        this.data.gyroscope.z = data;
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
