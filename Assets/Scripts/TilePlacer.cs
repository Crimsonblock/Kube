using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    public colliderFace face;
    public GameObject emptyTile;
    public bool placeEmpty;

    private bool shouldTrigger;
    private char numLoops;
    private const float tileOffset = .227f;
    private const float tileScale = 0.56f;
    // Start is called before the first frame update
    void Start()
    {
        numLoops = (char)0;
        shouldTrigger = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (numLoops > 3)
        {
            shouldTrigger = false;
        }
        if (numLoops > 4)
        {
            if (placeEmpty)
            {
                GameObject newEmptyTile = Instantiate(emptyTile);
                newEmptyTile.transform.SetParent(transform.parent, true);
                newEmptyTile.transform.localScale = Vector3.one * tileScale;
                Quaternion rotation = Quaternion.identity;
                switch (face)
                {
                    case colliderFace.FRONT:
                        rotation.eulerAngles = new Vector3(0, 180, 0);
                        newEmptyTile.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + tileOffset);
                        newEmptyTile.transform.rotation = rotation;
                        break;
                    case colliderFace.BACK:
                        newEmptyTile.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - tileOffset);
                        newEmptyTile.transform.rotation = rotation;
                        break;
                    case colliderFace.LEFT:
                        rotation.eulerAngles = new Vector3(0, -90, 0);
                        newEmptyTile.transform.position = new Vector3(transform.position.x + tileOffset, transform.position.y, transform.position.z);
                        newEmptyTile.transform.rotation = rotation;
                        break;
                    case colliderFace.RIGHT:
                        rotation.eulerAngles = new Vector3(0, +90, 0);
                        newEmptyTile.transform.position = new Vector3(transform.position.x - tileOffset, transform.position.y, transform.position.z);
                        newEmptyTile.transform.rotation = rotation;
                        break;
                    case colliderFace.TOP:
                        rotation.eulerAngles = new Vector3(-90, 0, 0);
                        newEmptyTile.transform.position = new Vector3(transform.position.x, transform.position.y - tileOffset, transform.position.z);
                        newEmptyTile.transform.rotation = rotation;
                        break;
                    case colliderFace.BOTTOM:
                        rotation.eulerAngles = new Vector3(90, 0, 0);
                        newEmptyTile.transform.position = new Vector3(transform.position.x, transform.position.y + tileOffset, transform.position.z);
                        newEmptyTile.transform.rotation = rotation;
                        break;
                    default:
                        Debug.Log("Invalid face value");
                        break;
                };
            }
            Destroy(gameObject);
        }
        numLoops++;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (shouldTrigger)
        {
            if (other.tag == "Tile")
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
                        other.transform.position = new Vector3(otherPosition.x, otherPosition.y - tileOffset, otherPosition.z);
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
}
