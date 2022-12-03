using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;



public delegate void DataCallback(float newData);
public delegate void MultiDataCallback(float x, float y, float z);


public struct GattCallbacks
{
    public MultiDataCallback accel;

    public MultiDataCallback gyro;

    public DataCallback encoderTop;
    public DataCallback encoderBottom;
    public DataCallback encoderLeft;
    public DataCallback encoderRight;
    public DataCallback encoderFront;
    public DataCallback encoderBack;
};

public delegate void LibCallback(int result);
public delegate void DebugLog(string message);

public class BleManager
{
    // Library importation
    private const string lib = "Ble-2.0.18.dll";
    [DllImport(lib)]
    public extern static void startBleDeviceWatcher(DebugLog debugLog = null); //Done
    [DllImport(lib)]
    public extern static void stopBleDeviceWatcher(); // Done
    [DllImport(lib)]
    public extern static int isBleWatcherBusy(); // Done
    [DllImport(lib)]
    public extern static void destroyBleManager(); // Done

    [DllImport(lib)]
    public  extern static int getNumRegisteredBleDevices(); // Done

    [DllImport(lib)]
    public extern static int registerCallback(int deviceHandle, GattCallbacks callbacks, LibCallback callback=null);
    [DllImport(lib)]
    public extern static void getDeviceAtIndex(StringBuilder name, int nameSize, StringBuilder id, int idSize, int index); // Done
    [DllImport(lib)]
    public extern static void bindToDeviceAtIndex(int index, string serviceGuid, LibCallback callback = null);

    [DllImport(lib)]
    public extern static int getNumBoundDevices();
    [DllImport(lib)]
    public extern static int isInstanciated();





    // For singleton
    private static BleManager instance = null;


    List<string> boundDevices = null;
    List<ConnectionManager> connectionManagers = null;

    int assignedConnMgr;
    bool busy;
    bool debug;
    bool debugLib;

    const string kubeService = "{1f397834-bcee-45b2-b1bf-1ca0cacaf290}";



    // Delegates

    DebugLog libLogger = null;
    LibCallback onBindDone;

    private void debugLog(string msg)
    {
        Debug.Log("[BleLib] " + msg);
    }


    private void onConnectionDone(int result)
    {
        if(result < 0)
        {
            dLog("Pairing failed, removing the device.");
            boundDevices.RemoveAt(0);
        }
        else
        {
            dLog("Pairing success, creating the Connection manager");
            connectionManagers.Add(new ConnectionManager(result, debug));
            dLog("Connection manager created");
        }
    }

    private void dLog(string msg)
    {
        if (debug) Debug.Log(msg);
    }


    public void startScan()
    {
        if (debugLib)
        {
            dLog("Starting the device watcher in debug mode");
            libLogger = debugLog;
            startBleDeviceWatcher(libLogger);
        }
        else
        {
            dLog("Starting the watcher in silent mode");
            startBleDeviceWatcher();
        }
        busy = true;
    }
    public void stopScan() { stopBleDeviceWatcher(); }
    public bool isWatcherBusy() { return isBleWatcherBusy() == 1; }
    public void destroy()
    {
        dLog("Destroying the Native Bluetooth LE manager");
        destroyBleManager();
        while (isInstanciated() == 1)
        {
            dLog("Waiting for the manager to be killed");
        }
        dLog("Done, destructing the .NET Bluetooth LE manager");
        boundDevices.Clear();
        boundDevices = null;
        connectionManagers.Clear();
        connectionManagers = null;
        instance = null;
    }

    public void update()
    {
        if (!isWatcherBusy() && busy)
        {
            dLog("Device watcher finished scanning");
            busy = false;
            for (int i = 0; i < getNumRegisteredBleDevices(); i++)
            {
                StringBuilder name = new StringBuilder(100);
                StringBuilder id = new StringBuilder(100);
                getDeviceAtIndex(name, 100, id, 100, i);
                if (name.ToString() == "Kube" && !boundDevices.Contains(id.ToString()))
                {
                    boundDevices.Add(id.ToString());
                    dLog("Device found, trying to pair");
                    onBindDone = onConnectionDone;
                    bindToDeviceAtIndex(i, kubeService, onBindDone);
                }
            }
        }
    }

    public static BleManager getInstance(bool debug = false, bool debugLib = false)
    {
        if (instance == null) instance = new BleManager(debug, debugLib);
        return instance;
    }


    public int bindConnectionHandlerToDevice(int bleHandle, GattCallbacks callbacks, LibCallback onFinishedCallback=null)
    {
        return registerCallback(bleHandle, callbacks, onFinishedCallback);
    }

    public ConnectionManager getConnectionManager()
    {
        if (assignedConnMgr >= connectionManagers.Count) return null;
        assignedConnMgr++;
        return connectionManagers[assignedConnMgr-1];
    }

    private BleManager(bool debug, bool debugLib)
    {
        this.debug = debug;
        this.debugLib = debugLib;
        assignedConnMgr = 0;
        busy = false;
        connectionManagers = new List<ConnectionManager>();
        boundDevices = new List<string>();
    }
}
