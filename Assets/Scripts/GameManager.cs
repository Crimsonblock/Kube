using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public delegate void ToggleModeHandler();

public class GameManager : MonoBehaviour
{

    public AudioSource source1;
    public AudioClip yay;

    private BleManager bleManager = null;
    private int numCubes = 0;
    private int numFinished = 0;
    SerialConnectionManager connmgr = null;

    bool lastLevelFinished = false;

    public Button NextLevel;

    List<ToggleModeHandler> toggleModes = new List<ToggleModeHandler>();



    // Start is called before the first frame update
    void Start()
    {
        connmgr = new();
        NextLevel.gameObject.SetActive(false);
        source1 = GetComponent<AudioSource>();

    }

    ~GameManager()
    {
       if(bleManager != null) bleManager.destroy();
    }

    public SerialConnectionManager getConnectionManager()
    {
        return connmgr;
    }

    // Update is called once per frame
    void Update()
    {
        if (connmgr != null) connmgr.Update();

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


    public bool finish()
    {
        numFinished++;
        if(numFinished >= numCubes)
        {
            Debug.Log("Finish");
            if(SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                Debug.Log("Loading next level");
                //bleManager.resetConnMgrs();
                source1.Pause();
                AudioClip.PlayOneShot(yay, 1.0f);
                NextLevel.gameObject.SetActive(true);

                return true;
            }
            else
            {
                Debug.Log("Last Level");
                lastLevelFinished =  true;
                NextLevel.gameObject.SetActive(false);
            }
        }

        return false;
    }

    public void unFinish()
    {
        numFinished--;
        NextLevel.gameObject.SetActive(false);
    }
}
