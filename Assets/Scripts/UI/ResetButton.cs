using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    public CubeOrientation cube;

    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ResetOnClick);
    }

    void ResetOnClick()
    {
        cube.PauseOrientation();
    }
}
