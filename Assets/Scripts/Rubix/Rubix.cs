using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Rubix : MonoBehaviour
{

    List<Transform> topCubes = new List<Transform>();
    List<Transform> bottomCubes = new List<Transform>();
    List<Transform> leftCubes = new List<Transform>();
    List<Transform> rightCubes = new List<Transform>();
    List<Transform> frontCubes = new List<Transform>();
    List<Transform> backCubes = new List<Transform>();

    double step = 20f;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Step 1: get the data from the connMgr

        // Step 2: update the orientation of the cube

        // Step 3:  update the faces of the cube
        updateFaces();
    }




    /// <summary>
    /// Calculates the position of the center of the cubes in a face.
    /// </summary>
    /// <param name="face"></param>
    /// <returns></returns>
    Vector3 getFaceCenter(colliderFace face)
    {
        Vector3 faceCenter = Vector3.zero;
        List<Transform> cubes;
        switch (face)
        {
            case colliderFace.TOP:
                cubes = topCubes;
                break;
            case colliderFace.BOTTOM:
                cubes = bottomCubes;
                break;
            case colliderFace.LEFT:
                cubes = leftCubes;
                break;
            case colliderFace.RIGHT:
                cubes = rightCubes;
                break;
            case colliderFace.FRONT:
                cubes = frontCubes;
                break;
            case colliderFace.BACK:
                cubes = backCubes;
                break;
            default:
                cubes = null;
                break;
        }
        if (cubes == null) return Vector3.zero;


        foreach(Transform t in cubes)
        {
            faceCenter += t.position;
        }
        return faceCenter/cubes.Count;
    }


    void updateFaces()
    {
        Vector3 faceCenter = getFaceCenter(colliderFace.FRONT);
        Vector3 rotationAxis = faceCenter - transform.position;

        foreach(Transform cube in frontCubes)
        {
            cube.SetLocalPositionAndRotation(  , Quaternion.Euler(step*Time.deltaTime*rotationAxis.normalized)  )
        }
    }






    /// <summary>
    /// Adds a cube to the face List. This is used to rotate the face if needed later on.
    /// </summary>
    /// <param name="cube">The transform of the cube to be added</param>
    /// <param name="face">The face the cube should be added to</param>
    public void addCubeToFace(Transform cube, colliderFace face) {
        switch (face)
        {
            case colliderFace.TOP:
                topCubes.Add(cube);
                break;
            case colliderFace.BOTTOM:
                bottomCubes.Add(cube);
                break;
            case colliderFace.LEFT:
                leftCubes.Add(cube); 
                break;
            case colliderFace.RIGHT:
                rightCubes.Add(cube); 
                break;
            case colliderFace.FRONT:
                frontCubes.Add(cube);
                break;
            case colliderFace.BACK:
                backCubes.Add(cube);
                break;
            default:
                Debug.Log("Unknown face to add the cube to");
                break;
        }
    }


    /// <summary>
    /// Removes a cube from a list containing the cubes in a face.
    /// </summary>
    /// <param name="cube">A reference to the cube transform to be removed from the face</param>
    /// <param name="face">The face the cube should be removed from</param>
    public void removeCubeFromFace(Transform cube, colliderFace face) {
        switch (face)
        {
            case colliderFace.TOP:
                topCubes.Remove(cube);
                break;
            case colliderFace.BOTTOM:
                bottomCubes.Remove(cube);
                break;
            case colliderFace.LEFT:
                leftCubes.Remove(cube); 
                break;
            case colliderFace.RIGHT:
                rightCubes.Remove(cube); 
                break;
            case colliderFace.FRONT:
                frontCubes.Remove(cube);
                break;
            case colliderFace.BACK:
                backCubes.Remove(cube);
                break;
            default:
                Debug.Log("Unknown face to remove the cube from");
                break;
        }
    }
}
