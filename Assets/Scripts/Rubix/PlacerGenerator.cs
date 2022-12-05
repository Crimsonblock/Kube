using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacerGenerator : MonoBehaviour
{

    public GameObject cubePlacerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 scale = this.transform.parent.localScale;
        scale = Vector3.one;

        scale = scale / 3;

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                for(int k = -1; k < 2; k++)
                {
                    if ( !(i==0 && j==0 && k==0) )
                    {
                        GameObject cubePlacer = Instantiate(cubePlacerPrefab, this.gameObject.transform, false);
                        cubePlacer.transform.localPosition = new Vector3(i* scale.x, j*scale.y, k*scale.z);
                        cubePlacer.transform.localScale = scale/2;
                    }
                }
            }
        } 
    }
}
