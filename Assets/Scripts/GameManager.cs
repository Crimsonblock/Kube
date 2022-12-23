using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public delegate void ToggleModeHandler();

public class GameManager : MonoBehaviour
{

    public AudioSource Background;
    public AudioSource CelebrationSound;

    private BleManager bleManager = null;
    private int numCubes = 0;
    private int numFinished = 0;
    SerialConnectionManager connmgr = null;

    bool lastLevelFinished = false;

    public GameObject NextLevel;

    List<ToggleModeHandler> toggleModes = new List<ToggleModeHandler>();



    // Start is called before the first frame update
    void Start()
    {
        connmgr = new();
        if(NextLevel != null) NextLevel.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                Background.Pause();
                CelebrationSound.Play();
                NextLevel.SetActive(true);

                return true;
            }
            else
            {
                Debug.Log("Last Level");
                lastLevelFinished =  true;
                if( NextLevel != null ) NextLevel.SetActive(false);
            }
        }

        return false;
    }

    public void unFinish()
    {
        numFinished--;
        NextLevel.SetActive(false);
    }
}
