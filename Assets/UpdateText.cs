using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateText : MonoBehaviour
{
    public Text textDebug;

    public void setText(string text)
    {
        if(textDebug != null && text != null) textDebug.text = text;
    }
}
