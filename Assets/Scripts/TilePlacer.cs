using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour
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

    public colliderFace face;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Tile")
        {
            other.transform.SetParent(transform.parent, true);
        }
    }
}
