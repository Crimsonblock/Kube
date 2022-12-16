using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{

    private char numLoops;
    // Start is called before the first frame update
    void Start()
    {
        numLoops = (char)0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (numLoops > 10)
        {
            Destroy(this.gameObject);
        }
        numLoops++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cube")
        {
            other.transform.position = transform.position;
            other.transform.SetParent(transform.parent, true);
            Destroy(this.gameObject);
        }
    }
}
