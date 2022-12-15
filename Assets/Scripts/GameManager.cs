using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public delegate void ToggleModeHandler();

public class GameManager : MonoBehaviour
{

    public CubeOrientation orientation;
    public UpdateText updateText;
    private BleManager bleManager = null;
    private int numCubes = 0;
    private int numFinished = 0;

    List<ToggleModeHandler> toggleModes = new List<ToggleModeHandler>();



    // Start is called before the first frame update
    void Start()
    {
        // Set a first parameter to true to get .NET Library log in unity
        // Set a second parameter to true to get Native library log in unity
        bleManager = BleManager.getInstance(true);
        bleManager.startScan();
        //bleManager = null;
        DontDestroyOnLoad(gameObject);
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

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Toggling mode");
            foreach (ToggleModeHandler t in toggleModes)
            {
                t();
            }
        }
    }

    public void registerRubix()
    {
        numCubes++;
    }


    public void registerToggleModeHandler(ToggleModeHandler newHandler)
    {
        toggleModes.Add(newHandler);
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
