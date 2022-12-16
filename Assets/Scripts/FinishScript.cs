using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScript : MonoBehaviour
{

    Rubix rubix;

    private void Start()
    {
        Transform parent = transform.parent;
        while (parent.tag != "Rubix")
        {
            parent = parent.parent;
        }
        rubix = parent.GetComponent<Rubix>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            rubix.finish();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            rubix.unFinish();
        }
    }
}
