using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public CubeOrientation orientation;
    public UpdateText updateText;
    private BleManager bleManager = null;
    private int numCubes = 0;
    private int numFinished = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set a first parameter to true to get .NET Library log in unity
        // Set a second parameter to true to get Native library log in unity
        bleManager = BleManager.getInstance(true);
        bleManager.startScan();
    }

    ~GameManager()
    {
       if(bleManager != null) bleManager.destroy();
    }

    public ConnectionManager getConnectionManager()
    {
        if (bleManager == null) return null;
        return bleManager.getConnectionManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (bleManager != null) bleManager.update();
    }

    public void registerRubix()
    {
        numCubes++;
    }


    public void finish()
    {
        numFinished++;
        if(numFinished == numCubes)
        {
            Debug.Log("Game finished !");
        }
    }

}
