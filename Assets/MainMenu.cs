using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() { 
    //loading screen
        SceneManager.LoadScene("level 1")
    }
    
    
    public void QuitGame() {
        Application.Quit();
    }

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
