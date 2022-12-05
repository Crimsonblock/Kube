using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GateScript gate;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            gate.activatorEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            gate.activatorLeave();
        }
    }
}
