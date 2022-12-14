using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    bool centerCube = false;
    colliderFace centerOfFace;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        centerOfFace = colliderFace.NONE;
    }


    public void setCenterCube(bool newVal, colliderFace face)
    {
        centerCube = newVal;
        centerOfFace = face;
    }

    public bool isCenterCube()
    {
        return centerCube;
    }
    
    public colliderFace getFaceOfCenter()
    {
        return centerOfFace;
    }
}