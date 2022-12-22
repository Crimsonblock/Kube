using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color normalTextColor;
    public Color highlightedTextColor;
    public TextMeshProUGUI text;


    // Start is called before the first frame update
    void Start()
    {
        text.color = normalTextColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightedTextColor;
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalTextColor;
        Debug.Log("OnPointerExit");
    }
}
