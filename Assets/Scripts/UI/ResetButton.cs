using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResetButton : MonoBehaviour
{
    public CubeOrientation cube;
    public TMP_Text text;

    private bool pause = true;

    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ResetOnClick);
    }

    void ResetOnClick()
    {
        pause = !pause;
        if (pause)
        {
            text.SetText("Keyboard Input");
        }
        else
        {
            text.SetText("Orientation");
        }
        cube.TogglePause();
    }
}
