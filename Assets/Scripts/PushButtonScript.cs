using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButtonScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GateScript gate;
    public Color buttonColor;
    public AudioSource ButtonClick;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.tag == "btnActivator")
            {
                Debug.Log("Activator found");
                child.GetComponent<Renderer>().material.color = buttonColor;
            }
        }

        if (ButtonClick == null)
            ButtonClick = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gate.activatorEnter();
            ButtonClick.Stop();
            ButtonClick.Play();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            gate.activatorLeave();

            if (gate.getCloseOnActivatorExit())
            {
                ButtonClick.Stop();
                ButtonClick.Play();
            }
        }
    }
}
