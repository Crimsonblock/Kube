using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Rubix : MonoBehaviour
{

    // Cubes faces
    List<Transform> topCubes = new List<Transform>();
    List<Transform> bottomCubes = new List<Transform>();
    List<Transform> leftCubes = new List<Transform>();
    List<Transform> rightCubes = new List<Transform>();
    List<Transform> frontCubes = new List<Transform>();
    List<Transform> backCubes = new List<Transform>();

    // Center cubes (used for rotation)
    Transform frontCenterCube;
    Transform backCenterCube;
    Transform leftCenterCube;
    Transform rightCenterCube;
    Transform topCenterCube;
    Transform bottomCenterCube;


    ConnectionManager connMgr = null;
    GameManager gameMgr = null;

    bool areCenterCubesSelected = false;

    public float step = 2f;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        gameMgr = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (connMgr == null)
        {
            if (gameMgr != null) connMgr = gameMgr.getConnectionManager();
            return;
        }
        if (areCenterCubesSelected)
        {
            if (connMgr.hasNewData())
            {
                // Step 1: get the data from the connMgr
                RubixData newData = connMgr.getNewData();
                // Step 2: update the orientation of the cube

                // Step 3:  update the faces of the cube
                updateFaces(newData.rotation);
            }
            
        }
    }




    /// <summary>
    /// Calculates the position of the center of the cubes in a face.
    /// </summary>
    /// <param name="face">The face the center should be returned for</param>
    /// <returns>Returns a vector3 to the position of the center of the face</returns>
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
            faceCenter += t.localPosition;
        }
        return faceCenter/cubes.Count;
    }

    /// <summary>
    /// Updates the faces' rotation
    /// </summary>
    /// <param name="newFaces">The new faces rotation data.</param>
    void updateFaces(RotaryEncoder newFaces)
    {
        if (newFaces.left != 0) rotateFace(colliderFace.LEFT, newFaces.left);
        if (newFaces.right != 0) rotateFace(colliderFace.RIGHT, newFaces.right);
        if (newFaces.bottom != 0) rotateFace(colliderFace.BOTTOM, newFaces.bottom);
        if (newFaces.top != 0) rotateFace(colliderFace.TOP, newFaces.top);
        if (newFaces.front != 0) rotateFace(colliderFace.FRONT, newFaces.front);
        if (newFaces.back != 0) rotateFace(colliderFace.BACK, newFaces.back);
    }

    /// <summary>
    /// Rotates the face around its axis.
    /// </summary>
    /// <param name="face">The face to be rotated around</param>
    /// <param name="steps">The number of steps to rotate. 1 step = 10°. Positive = cw, negative = ccw</param>
    void rotateFace(colliderFace face,  float steps)
    {
        Vector3 faceCenter = Vector3.zero;
        Vector3 rotationAxis = Vector3.zero;
        Transform cubesParents = null;

        switch (face)
        {
            
            case colliderFace.TOP:
                rotationAxis = new Vector3(0, 1, 0);
                cubesParents = topCenterCube.parent;
                foreach (Transform cube in topCubes)
                {
                    cube.SetParent(topCenterCube, true);
                }

                topCenterCube.Rotate(rotationAxis, 10*steps, Space.Self);

                foreach (Transform cube in topCubes)
                {
                    cube.SetParent(cubesParents, true);
                }
                break;
            case colliderFace.BOTTOM:
                rotationAxis = new Vector3(0, -1, 0);
                cubesParents = bottomCenterCube.parent;
                foreach (Transform cube in bottomCubes)
                {
                    cube.SetParent(bottomCenterCube, true);
                }

                bottomCenterCube.Rotate(rotationAxis, 10*steps, Space.Self);

                foreach (Transform cube in bottomCubes)
                {
                    cube.SetParent(cubesParents, true);
                }
                break;
            case colliderFace.LEFT:
                rotationAxis = new Vector3(-1, 0, 0);
                cubesParents = leftCenterCube.parent;
                foreach (Transform cube in leftCubes)
                {
                    cube.SetParent(leftCenterCube, true);
                }

                leftCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in leftCubes)
                {
                    cube.SetParent(cubesParents, true);
                }
                break;
            case colliderFace.RIGHT:
                rotationAxis = new Vector3(1, 0, 0);
                cubesParents = rightCenterCube.parent;
                foreach (Transform cube in rightCubes)
                {
                    cube.SetParent(rightCenterCube, true);
                }

                rightCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in rightCubes)
                {
                    cube.SetParent(cubesParents, true);
                }
                break;
            case colliderFace.FRONT:
                rotationAxis = new Vector3(0, 0, -1);
                cubesParents = frontCenterCube.parent;
                foreach (Transform cube in frontCubes)
                {
                    cube.SetParent(frontCenterCube, true);
                }

                frontCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in frontCubes)
                {
                    cube.SetParent(cubesParents, true);
                }
                break;
            case colliderFace.BACK:
                rotationAxis = new Vector3(0, 0, 1);
                cubesParents = backCenterCube.parent;
                foreach (Transform cube in backCubes)
                {
                    cube.SetParent(backCenterCube, true);
                }

                backCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in backCubes)
                {
                    cube.SetParent(cubesParents, true);
                }
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// Retrieves the center cube of every face and removes it from the lists of each face
    /// </summary>
    private void selectCenterCubes()
    {
        if (topCubes.Count <= 0) return;
        // Top
        Vector3 center = getFaceCenter(colliderFace.TOP);
        foreach (Transform cube in topCubes)
            {
                if (cube.localPosition == center)
                {
                    topCenterCube = cube;
                    topCubes.Remove(cube);
                    break;
                }
            }
        if (topCenterCube == null) return;


        if (bottomCubes.Count <= 0) return;  
        // Bottom
        center = getFaceCenter(colliderFace.BOTTOM);
        foreach (Transform cube in bottomCubes)
            {
                if (cube.localPosition == center)
                {
                    bottomCenterCube = cube;
                    bottomCubes.Remove(cube);
                    break;
                }
            }
        if (bottomCenterCube == null) return;


        if (frontCubes.Count <=0) return;
        // Front
        center = getFaceCenter(colliderFace.FRONT);
        foreach (Transform cube in frontCubes)
            {
                if (cube.localPosition == center)
                {
                    frontCenterCube = cube;
                    frontCubes.Remove(cube);
                    
                    break;
                }
            }
        if (frontCenterCube == null) return;


        if (backCubes.Count <= 0) return;
        // Back
        center = getFaceCenter(colliderFace.BACK);
        foreach (Transform cube in backCubes)
        {
            if (cube.localPosition == center)
            {
                backCenterCube = cube;
                backCubes.Remove(cube);
                break;
            }
        }
        if (backCenterCube == null) return;

        if (leftCubes.Count <= 0) return;
        // Left
        center = getFaceCenter(colliderFace.LEFT);
        foreach (Transform cube in leftCubes)
            {
                if (cube.localPosition == center)
                {
                    leftCenterCube = cube;
                    leftCubes.Remove(cube);
                    break;
                }
        }
        if (leftCenterCube == null) return;

        if (rightCubes.Count <= 0) return;
        // Right
        center = getFaceCenter(colliderFace.RIGHT);
        foreach (Transform cube in rightCubes)
            {
                if (cube.localPosition == center)
                {
                    rightCenterCube = cube;
                    rightCubes.Remove(cube);
                    break;
                }
        }
        if (rightCenterCube == null) return;
        areCenterCubesSelected = true;
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
