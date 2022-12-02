using UnityEngine;

[System.Serializable]
public class RubixData
{
    public Vector3 accelerometer;
    public Vector3 gyroscope;
    public RotaryEncoder rotation;

    public RubixData()
    {
        accelerometer = Vector3.zero;
        gyroscope = Vector3.zero;
        rotation = new RotaryEncoder();
    }
}

[System.Serializable]
public class RotaryEncoder
{
    public float front { get; set; }
    public float back { get; set; }
    public float left { get; set; }
    public float right { get; set; }
    public float top { get; set; }
    public float bottom { get; set; }

    public RotaryEncoder(float front = 0, float back = 0, float left = 0, float right = 0, float top=0, float bottom=0)
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

        callbacks.encoderLeft = newLeftEncoder;
        callbacks.encoderRight = newRightEncoder;
        callbacks.encoderFront = newFrontEncoder;
        callbacks.encoderBack = newBackEncoder;
        callbacks.encoderTop = newTopEncoder;
        callbacks.encoderBottom = newBottomEncoder;

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

    private void newFrontEncoder(float data)
    {
        this.data.rotation.front = data;
        this.newDataReceived = true;
    }
    private void newBackEncoder(float data)
    {
        this.data.rotation.back = data;
        this.newDataReceived = true;
    }
    private void newLeftEncoder(float data)
    {
        this.data.rotation.left = data;
        this.newDataReceived=true;
    }
    private void newRightEncoder(float data)
    {
        this.data.rotation.right = data;
        this.newDataReceived = true;
    }
    private void newTopEncoder(float data)
    {
        this.data.rotation.top = data;
        this.newDataReceived = true;
    }
    private void newBottomEncoder(float data)
    {
        this.data.rotation.bottom = data;
        this.newDataReceived = true;
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
