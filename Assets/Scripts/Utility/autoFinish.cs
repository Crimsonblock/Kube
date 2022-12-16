using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoFinish : MonoBehaviour
{

    public GameManager gm;
    bool t;


    // Start is called before the first frame update
    void Update()
    {
        if (!t)
        {
            t = true;
            gm.finish();
        }   
    }

}
