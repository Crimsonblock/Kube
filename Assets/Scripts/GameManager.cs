using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public delegate void ToggleModeHandler();

public class GameManager : MonoBehaviour
{

    public CubeOrientation orientation;
    public UpdateText updateText;
    private BleManager bleManager = null;
    private int numCubes = 0;
    private int numFinished = 0;

    bool lastLevelFinished = false;

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
        else if (lastLevelFinished)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                lastLevelFinished = false;
                SceneManager.LoadScene(0);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                Application.Quit();
                Destroy(gameObject);
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
        if(numFinished >= numCubes)
        {
            Debug.Log("Finish");
            if(SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                Debug.Log("Loading next level");
                bleManager.resetConnMgrs();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                Debug.Log("Last Level");
                lastLevelFinished =  true;
            }
        }
    }

}
