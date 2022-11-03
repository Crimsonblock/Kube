using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    public colliderFace face;

    private char numLoops;
    private const float tileOffset = 0.28f;
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
            Destroy(gameObject);
        }
        numLoops++;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Tile")
        {
            other.transform.SetParent(transform.parent, true);
            other.transform.position = transform.position;
            Vector3 otherPosition = other.transform.position;
            switch (face)
            {
                case colliderFace.FRONT:
                    other.transform.position = new Vector3(otherPosition.x, otherPosition.y, otherPosition.z + tileOffset);
                    break;
                case colliderFace.BACK:
                    other.transform.position = new Vector3(otherPosition.x, otherPosition.y, otherPosition.z - tileOffset);
                    break;
                case colliderFace.LEFT:
                    other.transform.position = new Vector3(otherPosition.x + tileOffset, otherPosition.y, otherPosition.z);
                    break;
                case colliderFace.RIGHT:
                    other.transform.position = new Vector3(otherPosition.x - tileOffset, otherPosition.y, otherPosition.z);
                    break;
                case colliderFace.TOP:
                    other.transform.position = new Vector3(otherPosition.x, otherPosition.y-tileOffset, otherPosition.z);
                    break;
                case colliderFace.BOTTOM:
                    other.transform.position = new Vector3(otherPosition.x, otherPosition.y + tileOffset, otherPosition.z);
                    break;
                default:
                    Debug.Log("Invalid face value");
                    break;
            };

            Destroy(gameObject);
        }
    }
}
