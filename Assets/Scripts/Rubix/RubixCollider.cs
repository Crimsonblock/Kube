using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum colliderFace
{
    FRONT,
    BACK,
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
    NONE
}

public class RubixCollider : MonoBehaviour
{
    //Public fields
    public colliderFace face;

    //Private fields
    private Rubix parentRubix;

    // Start is called before the first frame update
    void Start()
    {
        Transform rubixTransform = this.transform.parent;
        while (rubixTransform.tag != "Rubix")
        {
            rubixTransform = rubixTransform.parent;
        }
        parentRubix = rubixTransform.GetComponent<Rubix>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cube" || other.tag == "Player")
        {
            parentRubix.addCubeToFace(other.GetComponent<Transform>(), face);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Cube" || other.tag == "Player")
        {
            parentRubix.removeCubeFromFace(other.GetComponent<Transform>(), face);
        }
    }
}
