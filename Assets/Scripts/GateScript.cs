using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    int counter = 0;
    bool isOpen = false;
    public bool CloseOnActivatorExit = false;
    public Color gateColor;
    public AudioSource GateSwing;

    void Start()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Renderer>().material.color = gateColor;
        }

        if (GateSwing == null)
            GateSwing = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (isOpen && counter <45)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                int direction = child.tag == "gateR" ? 1 : -1;
                child.transform.Rotate(Vector3.forward, direction*2, Space.Self);
            }
            counter++;
        }
        else if(!isOpen && counter > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                int direction = child.tag == "gateR" ? -1 : 1;
                child.transform.Rotate(Vector3.forward, direction*2, Space.Self);
            }
            counter--;
        }
    }


    public void activatorEnter()
    {
        if (!isOpen)
        {
            GateSwing.Stop();
            GateSwing.Play();
        }

        isOpen = true;
    }


    public void activatorLeave()
    {
        if (CloseOnActivatorExit)
        {
            isOpen = false;

            GateSwing.Stop();
            GateSwing.Play();
        }
    }
}
