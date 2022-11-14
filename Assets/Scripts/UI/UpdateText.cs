using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateText : MonoBehaviour
{
    public TMP_Text debugText;

    public void setText(string text)
    {
        if (debugText != null && text != null) debugText.SetText(text);
    }
}
