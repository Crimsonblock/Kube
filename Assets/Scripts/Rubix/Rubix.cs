using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Assertions.Must;
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

    colliderFace []faceSelectionRotation = { colliderFace.FRONT, colliderFace.BACK, colliderFace.LEFT, colliderFace.RIGHT, colliderFace.TOP, colliderFace.BOTTOM } ;
    int currentSelectedFace = 0;
    int []facesSupposedRotation = new int[6];


    Color m_highlightColor = new Color(.95f, .95f, .80f);

    // Center cubes (used for rotation)
    Transform frontCenterCube;
    Transform backCenterCube;
    Transform leftCenterCube;
    Transform rightCenterCube;
    Transform topCenterCube;
    Transform bottomCenterCube;

    public float timeBeforeSnap;

    ConnectionManager connMgr = null;
    public GameManager gameMgr = null;

    bool areCenterCubesSelected = false;

    CubeOrientation or = null;

    bool isKeyboardPlayed = false;
    public bool canBeGyroPlayed = false;

    public float step = 2f;

    float []timeNotCentered = new float[6];

    Vector3 keysStatus = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = false;
        gameMgr.registerRubix();
        or = GetComponent<CubeOrientation>();

        gameMgr.registerToggleModeHandler(handleToggle);
    }


    ~Rubix()
    {
        connMgr = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (canBeGyroPlayed && !isKeyboardPlayed && connMgr == null)
        {
            if (gameMgr != null) connMgr = gameMgr.getConnectionManager();
            return;
        }

        if (areCenterCubesSelected)
        {
            if (canBeGyroPlayed && !isKeyboardPlayed && connMgr.hasNewData())
            {
                // Step 1: get the data from the connMgr
                RubixData newData = connMgr.getNewData();

                // Step 2: update the orientation of the cube
                or.Orientate(newData);
                

                // Step 3:  update the faces of the cube
                updateFaces(newData.rotation);
            }
            else
            {
                // if can be gyroplayed, sets the "crappy" commands"
                if (canBeGyroPlayed)
                {
                    if(isKeyboardPlayed) okayKeyboardControls();
                    else snapFaces();
                }
                // Otherwise, sets the better commands
                else
                {
                    goodKeyboardControls();
                }
            }
        }
        else
        {
            selectCenterCubes();
        }



    }


    void snapFaces()
    {
        // Creates the correction data
        RotaryEncoder correction = new RotaryEncoder();

        // Front snapping
        if (frontCenterCube.transform.localEulerAngles.z % 90 != 0)
        {
            int angle = (int)Mathf.Round(frontCenterCube.transform.localEulerAngles.z % 90);
            timeNotCentered[0] += Time.deltaTime;
            // Snaps back
            if (timeNotCentered[0] >= timeBeforeSnap)
            {
                if (angle >= 50) correction.front = angle-90;
                else correction.front = angle;
                correction.front = (int) (correction.front / step);
            }
        }

        // Back snapping
        else if(backCenterCube.transform.localEulerAngles.z % 90 != 0)
        {
            int angle = (int)Mathf.Round(backCenterCube.transform.localEulerAngles.z % 90);
            timeNotCentered[1] += Time.deltaTime;
            // Snaps back
            if (timeNotCentered[1] >= timeBeforeSnap)
            {
                if (angle >= 50) correction.back = 90 - angle;
                else correction.back = -angle;
                correction.back = correction.back / step;
            }
        }

        // Left snapping
        else if (leftCenterCube.transform.localEulerAngles.x % 90 != 0)
        {
            int angle = (int)Mathf.Round(leftCenterCube.transform.localEulerAngles.x % 90);
            timeNotCentered[2] += Time.deltaTime;
            // Snaps back
            if (timeNotCentered[2] >= timeBeforeSnap)
            {
                if (angle >= 50) correction.left = angle-90;
                else correction.left = angle;
                correction.left = correction.left / step;
            }
        }

        // Right snapping
        else if (rightCenterCube.transform.localEulerAngles.x % 90 != 0)
        {
            int angle = (int)Mathf.Round(rightCenterCube.transform.localEulerAngles.x % 90);
            timeNotCentered[3] += Time.deltaTime;
            // Snaps back
            if (timeNotCentered[3] >= timeBeforeSnap)
            {
                if (angle % 90 >= 50) correction.right = 90 - angle;
                else correction.right = -angle;
                correction.right = correction.right / step;
            }
        }

        // Top snapping
        else if (topCenterCube.transform.localEulerAngles.y % 90 != 0)
        {
            int angle = (int)Mathf.Round(topCenterCube.transform.localEulerAngles.y % 90);
            timeNotCentered[4] += Time.deltaTime;
            // Snaps back
            if (timeNotCentered[4] >= timeBeforeSnap)
            {
                if (angle >= 50) correction.top = 90-angle;
                else correction.top = -angle;
                correction.top = correction.top / step;
            }
        }

        // Bottom snapping
        else if (bottomCenterCube.transform.localEulerAngles.y % 90 != 0)
        {
            int angle = (int)Mathf.Round(bottomCenterCube.transform.localEulerAngles.y % 90);
            timeNotCentered[5] += Time.deltaTime;
            // Snaps back
            if (timeNotCentered[5] >= timeBeforeSnap)
            {
                if (angle >= 50) correction.bottom = angle - 90;
                else correction.bottom = angle;
                correction.bottom = correction.bottom / step;
            }
        }

        // Updates the faces with the correction to snap everything back to place
        updateFaces(correction);
    }

    // Controls for the cube that is non gyro playable
    void goodKeyboardControls()
    {
        // Pitch
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(new Vector3(1, 0, 0), 1, Space.World);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(new Vector3(1, 0, 0), -1, Space.World);
        }

        // Yaw
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(new Vector3(0, 1, 0), 1, Space.World);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, 1, 0), -1, Space.World);
        }

        // Roll
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, 0, 1), 1, Space.World);
        }
        else if (Input.GetKey(KeyCode.D)){
            transform.Rotate(new Vector3(0, 0, 1), -1, Space.World);
            
        }

        //face Selection
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            cleanFace(faceSelectionRotation[currentSelectedFace]);
            currentSelectedFace++;
            if (currentSelectedFace > 5) currentSelectedFace = 0;
            highlightFace(faceSelectionRotation[currentSelectedFace]);
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            cleanFace(faceSelectionRotation[currentSelectedFace]);
            currentSelectedFace--;
            if (currentSelectedFace < 0) currentSelectedFace = 5;
            highlightFace(faceSelectionRotation[currentSelectedFace]);
        }

        // Face rotation setup
        if (Input.GetKeyDown(KeyCode.X))
        {
            facesSupposedRotation[currentSelectedFace] -= 90;
            if(facesSupposedRotation[currentSelectedFace] < 0) facesSupposedRotation[currentSelectedFace] += 360;
            //Debug.Log(facesSupposedRotation[currentSelectedFace]);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            facesSupposedRotation[currentSelectedFace] += 90;
            if (facesSupposedRotation[currentSelectedFace] >= 360) facesSupposedRotation[currentSelectedFace] -= 360;
            //Debug.Log(facesSupposedRotation[currentSelectedFace]);
        }

        rotateFacesK();
    }

    void okayKeyboardControls()
    {
        // Pitch
        if (Input.GetKey(KeyCode.I))
        {
            transform.Rotate(new Vector3(1, 0, 0), 1, Space.World);
        }
        else if (Input.GetKey(KeyCode.K))
        {
            transform.Rotate(new Vector3(1, 0, 0), -1, Space.World);
        }

        // Yaw
        if (Input.GetKey(KeyCode.U))
        {
            transform.Rotate(new Vector3(0, 1, 0), 1, Space.World);
        }
        else if (Input.GetKey(KeyCode.O))
        {
            transform.Rotate(new Vector3(0, 1, 0), -1, Space.World);
        }

        // Roll
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(new Vector3(0, 0, 1), +1, Space.World);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(new Vector3(0, 0, 1), -1, Space.World);
        }

        // Face Selection 
        //face Selection
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            cleanFace(faceSelectionRotation[currentSelectedFace]);
            currentSelectedFace++;
            if (currentSelectedFace > 5) currentSelectedFace = 0;
            highlightFace(faceSelectionRotation[currentSelectedFace]);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            cleanFace(faceSelectionRotation[currentSelectedFace]);
            currentSelectedFace--;
            if (currentSelectedFace < 0) currentSelectedFace = 5;
            highlightFace(faceSelectionRotation[currentSelectedFace]);
        }

        // Face rotation setup
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            facesSupposedRotation[currentSelectedFace] -= 90;
            if (facesSupposedRotation[currentSelectedFace] < 0) facesSupposedRotation[currentSelectedFace] += 360;
            //Debug.Log(facesSupposedRotation[currentSelectedFace]);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            facesSupposedRotation[currentSelectedFace] += 90;
            if (facesSupposedRotation[currentSelectedFace] >= 360) facesSupposedRotation[currentSelectedFace] -= 360;
            //Debug.Log(facesSupposedRotation[currentSelectedFace]);
        }

        rotateFacesK();
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
            if(t.tag == "Cube") faceCenter += t.position;
        }
        return faceCenter/9;
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
    /// <param name="steps">The number of steps to rotate. 1 step = 10�. Positive = cw, negative = ccw</param>
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
                    if(cube.tag == "Player") cube.GetComponent<Rigidbody>().isKinematic = true;
                    cube.SetParent(topCenterCube, true);
                }

                topCenterCube.Rotate(rotationAxis, 10*steps, Space.Self);

                foreach (Transform cube in topCubes)
                {
                    if(cube.tag == "Player") {
                        cube.SetParent(transform, true);
                        cube.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    else cube.SetParent(cubesParents, true);
                }
                timeNotCentered[4] = 0;
                break;
            case colliderFace.BOTTOM:
                rotationAxis = new Vector3(0, -1, 0);
                cubesParents = bottomCenterCube.parent;
                foreach (Transform cube in bottomCubes)
                {
                    if(cube.tag == "Player") cube.GetComponent<Rigidbody>().isKinematic = true;
                    cube.SetParent(bottomCenterCube, true);
                }

                bottomCenterCube.Rotate(rotationAxis, 10*steps, Space.Self);

                foreach (Transform cube in bottomCubes)
                {
                    if (cube.tag == "Player") {
                        cube.GetComponent<Rigidbody>().isKinematic = false;
                        cube.SetParent(transform, true);
                    }
                    else cube.SetParent(cubesParents, true);
                }
                timeNotCentered[5] = 0;
                break;
            case colliderFace.LEFT:
                rotationAxis = new Vector3(-1, 0, 0);
                cubesParents = leftCenterCube.parent;
                foreach (Transform cube in leftCubes)
                {
                    if(cube.tag == "Player") cube.GetComponent<Rigidbody>().isKinematic = true;
                    cube.SetParent(leftCenterCube, true);
                }

                leftCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in leftCubes)
                {
                    if (cube.tag == "Player") {
                        cube.SetParent(transform, true);
                        cube.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    else cube.SetParent(cubesParents, true);
                }
                timeNotCentered[2] = 0;
                break;
            case colliderFace.RIGHT:
                rotationAxis = new Vector3(1, 0, 0);
                cubesParents = rightCenterCube.parent;
                foreach (Transform cube in rightCubes)
                {
                    if(cube.tag == "Player") cube.GetComponent<Rigidbody>().isKinematic = true;
                    cube.SetParent(rightCenterCube, true);
                }

                rightCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in rightCubes)
                {
                    if (cube.tag == "Player") {
                        cube.SetParent(transform, true);
                        cube.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    else cube.SetParent(cubesParents, true);
                }
                timeNotCentered[3] = 0;
                break;
            case colliderFace.FRONT:
                rotationAxis = new Vector3(0, 0, -1);
                cubesParents = frontCenterCube.parent;
                foreach (Transform cube in frontCubes)
                {
                    if(cube.tag == "Player") cube.GetComponent<Rigidbody>().isKinematic = true;
                    cube.SetParent(frontCenterCube, true);
                }

                frontCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in frontCubes)
                {
                    if (cube.tag == "Player"){
                        cube.SetParent(transform, true);
                        cube.GetComponent<Rigidbody>().isKinematic = false;
                    } 
                    else cube.SetParent(cubesParents, true);
                }
                timeNotCentered[0] = 0;
                break;
            case colliderFace.BACK:
                rotationAxis = new Vector3(0, 0, 1);
                cubesParents = backCenterCube.parent;
                foreach (Transform cube in backCubes)
                {
                    if(cube.tag == "Player") cube.GetComponent<Rigidbody>().isKinematic = true;
                    cube.SetParent(backCenterCube, true);
                }

                backCenterCube.Rotate(rotationAxis, 10 * steps, Space.Self);

                foreach (Transform cube in backCubes)
                {
                    if (cube.tag == "Player"){
                        cube.SetParent(transform, true);
                        cube.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    else cube.SetParent(cubesParents, true);
                }
                timeNotCentered[1] = 0;
                break;
            default:
                break;
        }
    }


    void rotateFacesK()
    {
        Vector3 rotationAxis = Vector3.zero;
        Transform cubesParents;

        // Top Face
        if (canFaceRotate(colliderFace.TOP) && (int)(topCenterCube.localRotation.eulerAngles.y) != facesSupposedRotation[4])
        {
            rotationAxis = new Vector3(0, 1, 0);
            cubesParents = topCenterCube.parent;
            foreach (Transform cube in topCubes)
            {
                cube.SetParent(topCenterCube, true);
            }

            if (shouldRotateClockwise((int)topCenterCube.localRotation.eulerAngles.y, facesSupposedRotation[4]))
            {
                topCenterCube.Rotate(rotationAxis, 1, Space.Self);
            }
            else
            {
                topCenterCube.Rotate(rotationAxis, -1, Space.Self);
            }

            foreach (Transform cube in topCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = true;

                }
                else cube.SetParent(cubesParents, true);
            }
        }
        
        // Bottom Face
        if (canFaceRotate(colliderFace.BOTTOM) && (int)(bottomCenterCube.localRotation.eulerAngles.y) != facesSupposedRotation[5])
        {
            rotationAxis = new Vector3(0, 1, 0);
            cubesParents = bottomCenterCube.parent;
            foreach (Transform cube in bottomCubes)
            {
                cube.SetParent(bottomCenterCube, true);
            }

            if (shouldRotateClockwise((int)bottomCenterCube.localRotation.eulerAngles.y, facesSupposedRotation[5]))
            {
                bottomCenterCube.Rotate(rotationAxis, 1, Space.Self);
            }
            else
            {
                bottomCenterCube.Rotate(rotationAxis, -1, Space.Self);
            }

            foreach (Transform cube in bottomCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = true;

                }
                else cube.SetParent(cubesParents, true);
            }
        }

        // Front face
        if (canFaceRotate(colliderFace.FRONT) && (int)(frontCenterCube.localRotation.eulerAngles.z) != facesSupposedRotation[0])
        {
            rotationAxis = new Vector3(0, 0, 1);
            cubesParents = frontCenterCube.parent;
            foreach (Transform cube in frontCubes)
            {
                cube.SetParent(frontCenterCube, true);
            }

            if (shouldRotateClockwise((int)frontCenterCube.localRotation.eulerAngles.z, facesSupposedRotation[0]))
            {
                frontCenterCube.Rotate(rotationAxis, 1, Space.Self);
            }
            else
            {
                frontCenterCube.Rotate(rotationAxis, -1, Space.Self);
            }

            foreach (Transform cube in frontCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = true;

                }
                else cube.SetParent(cubesParents, true);
            }
        }

        // Back Face
        if (canFaceRotate(colliderFace.BACK) && (int)(backCenterCube.localRotation.eulerAngles.z) != facesSupposedRotation[1])
        {
            rotationAxis = new Vector3(0, 0, 1);
            cubesParents = backCenterCube.parent;
            foreach (Transform cube in backCubes)
            {
                cube.SetParent(backCenterCube, true);
            }

            if (shouldRotateClockwise((int)backCenterCube.localRotation.eulerAngles.z, facesSupposedRotation[1]))
            {
                backCenterCube.Rotate(rotationAxis, 1, Space.Self);
            }
            else
            {
                backCenterCube.Rotate(rotationAxis, -1, Space.Self);
            }

            foreach (Transform cube in backCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = true;

                }
                else cube.SetParent(cubesParents, true);
            }
        }



        float correctedAngle = leftCenterCube.localRotation.eulerAngles.x;
        bool isBugged = false;
        //bugCorrection
        if (leftCenterCube.localRotation.eulerAngles.y == 180 && leftCenterCube.localRotation.eulerAngles.z == 180)
        {
            //bugged
            isBugged = true;
            if ((int)(leftCenterCube.localEulerAngles.x) == 0)
            {
                correctedAngle = 180;
            }
            else if (leftCenterCube.localEulerAngles.x < 90 && leftCenterCube.localEulerAngles.x > 0)
            {
                correctedAngle = 180 - leftCenterCube.localEulerAngles.x;
            }
            else if (leftCenterCube.localEulerAngles.x >= 270)
            {
                correctedAngle = 540 - leftCenterCube.localEulerAngles.x;
            }
        }

        // Left Face
        if (canFaceRotate(colliderFace.LEFT) && (int)(correctedAngle) != facesSupposedRotation[2])
        {
            rotationAxis = new Vector3(1, 0, 0);
            cubesParents = leftCenterCube.parent;
            foreach (Transform cube in leftCubes)
            {
                cube.SetParent(leftCenterCube, true);
            }
            if (shouldRotateClockwise((int) correctedAngle, facesSupposedRotation[2]))
            {
                leftCenterCube.Rotate(rotationAxis, 1, Space.Self);
            }
            else
            {
                leftCenterCube.Rotate(rotationAxis, -1, Space.Self);
            }

            foreach (Transform cube in leftCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = true;
                }
                else cube.SetParent(cubesParents, true);
            }
        }

        // Unity bug correction
        correctedAngle = rightCenterCube.localRotation.eulerAngles.x;
        isBugged = false;
        if (rightCenterCube.localRotation.eulerAngles.y == 180 && rightCenterCube.localRotation.eulerAngles.z == 180)
        {
            //bugged
            isBugged = true;
            if ((int)(rightCenterCube.localEulerAngles.x) == 0)
            {
                correctedAngle = 180;
            }
            else if (rightCenterCube.localEulerAngles.x < 90 && rightCenterCube.localEulerAngles.x > 0)
            {
                correctedAngle = 180 - leftCenterCube.localEulerAngles.x;
            }
            else if (rightCenterCube.localEulerAngles.x >= 270)
            {
                correctedAngle = 540 - rightCenterCube.localEulerAngles.x;
            }
        }
        // Right Face
        if (canFaceRotate(colliderFace.RIGHT) && (int)(correctedAngle) != facesSupposedRotation[3])
        {
            rotationAxis = new Vector3(1, 0, 0);
            cubesParents = rightCenterCube.parent;
            foreach (Transform cube in rightCubes)
            {
                cube.SetParent(rightCenterCube, true);
            }

            if (shouldRotateClockwise((int)correctedAngle, facesSupposedRotation[3]))
            {
                rightCenterCube.Rotate(rotationAxis, 1, Space.Self);
            }
            else
            {
                rightCenterCube.Rotate(rotationAxis, -1, Space.Self);
            }

            foreach (Transform cube in rightCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = true;

                }
                else cube.SetParent(cubesParents, true);
            }
        }




        // Face rotation corrections: 
        // bottom
        if ((int)(bottomCenterCube.localRotation.eulerAngles.y) == facesSupposedRotation[5])
        {
            cubesParents = bottomCenterCube.parent;
            foreach (Transform cube in bottomCubes)
            {
                cube.SetParent(bottomCenterCube, true);
            }

            bottomCenterCube.localRotation = Quaternion.Euler(0, facesSupposedRotation[5], 0);
            foreach (Transform cube in bottomCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = false;
                }
                else cube.SetParent(cubesParents, true);
            }
        }

        //Top
        if ((int)(topCenterCube.localRotation.eulerAngles.y) == facesSupposedRotation[4])
        {
            cubesParents = topCenterCube.parent;
            foreach (Transform cube in topCubes)
            {
                cube.SetParent(topCenterCube, true);
            }

            topCenterCube.localRotation = Quaternion.Euler(0, facesSupposedRotation[4], 0);


            foreach (Transform cube in topCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = false;
                }
                else cube.SetParent(cubesParents, true);
            }
        }

        //Front
        if ((int)(frontCenterCube.localRotation.eulerAngles.z) == facesSupposedRotation[0])
        {
            cubesParents = frontCenterCube.parent;
            foreach (Transform cube in frontCubes)
            {
                cube.SetParent(frontCenterCube, true);
            }

            frontCenterCube.localRotation = Quaternion.Euler(0, 0, facesSupposedRotation[0]);


            foreach (Transform cube in frontCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = false;
                }
                else cube.SetParent(cubesParents, true);
            }
        }

        //Back
        if ((int)(backCenterCube.localRotation.eulerAngles.z) == facesSupposedRotation[1])
        {
            cubesParents = backCenterCube.parent;
            foreach (Transform cube in backCubes)
            {
                cube.SetParent(backCenterCube, true);
            }

            backCenterCube.localRotation = Quaternion.Euler(0, 0, facesSupposedRotation[1]);


            foreach (Transform cube in backCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = false;
                }
                else cube.SetParent(cubesParents, true);
            }
        }


        correctedAngle = leftCenterCube.localRotation.eulerAngles.x;
        isBugged = false;
        //bugCorrection
        if (leftCenterCube.localRotation.eulerAngles.y == 180 && leftCenterCube.localRotation.eulerAngles.z == 180)
        {
            //bugged
            isBugged = true;
            if ((int)(leftCenterCube.localEulerAngles.x) == 0)
            {
                correctedAngle = 180;
            }
            else if (leftCenterCube.localEulerAngles.x < 90 && leftCenterCube.localEulerAngles.x > 0)
            {
                correctedAngle = 180 - leftCenterCube.localEulerAngles.x;
            }
            else if (leftCenterCube.localEulerAngles.x >= 270)
            {
                correctedAngle = 540 - leftCenterCube.localEulerAngles.x;
            }
        }

        //Left
        if ((int)(correctedAngle) == facesSupposedRotation[2])
        {
            cubesParents = leftCenterCube.parent;
            foreach (Transform cube in leftCubes)
            {
                cube.SetParent(leftCenterCube, true);
            }

            //Debug.Log("Angles to correct: " + facesSupposedRotation[2]);

            leftCenterCube.localRotation = Quaternion.Euler((int)correctedAngle, 0, 0);

            foreach (Transform cube in leftCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = false;
                }
                else cube.SetParent(cubesParents, true);
            }
        }

        //bugCorrection
        correctedAngle = rightCenterCube.localRotation.eulerAngles.x;
        isBugged = false;
        if (rightCenterCube.localRotation.eulerAngles.y == 180 && rightCenterCube.localRotation.eulerAngles.z == 180)
        {
            //bugged
            isBugged = true;
            if ((int)(rightCenterCube.localEulerAngles.x) == 0)
            {
                correctedAngle = 180;
            }
            else if (rightCenterCube.localEulerAngles.x < 90 && rightCenterCube.localEulerAngles.x > 0)
            {
                correctedAngle = 180 - leftCenterCube.localEulerAngles.x;
            }
            else if (rightCenterCube.localEulerAngles.x >= 270)
            {
                correctedAngle = 540 - rightCenterCube.localEulerAngles.x;
            }
        }
        //Right
        if ((int)(correctedAngle) == facesSupposedRotation[3])
        {
            rotationAxis = new Vector3(1, 0, 0);
            cubesParents = rightCenterCube.parent;
            foreach (Transform cube in rightCubes)
            {
                cube.SetParent(rightCenterCube, true);
            }

            rightCenterCube.localRotation = Quaternion.Euler((int)correctedAngle, 0, 0);


            foreach (Transform cube in rightCubes)
            {
                if (cube.tag == "Player")
                {
                    cube.SetParent(transform, true);
                    cube.GetComponent<Rigidbody>().isKinematic = false;
                }
                else cube.SetParent(cubesParents, true);
            }
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
                if (cube.position == center)
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
                if (cube.position == center)
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
                if (cube.position == center)
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
            if (cube.position == center)
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
                if (cube.position == center)
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
                if (cube.position == center)
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



    // Keyboard playmode helber functions
    void cleanFace(colliderFace face)
    {
        switch (face)
        {
            case colliderFace.FRONT:
                foreach (var cube in frontCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                        }
                    }
                }
                for (int i = 0; i < frontCenterCube.childCount; i++)
                {
                    var c = frontCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
            case colliderFace.BACK:
                foreach (var cube in backCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                        }
                    }
                }
                for (int i = 0; i < backCenterCube.childCount; i++)
                {
                    var c = backCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
            case colliderFace.TOP:
                foreach (var cube in topCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                        }
                    }
                }
                for (int i = 0; i < topCenterCube.childCount; i++)
                {
                    var c = topCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
            case colliderFace.BOTTOM:
                foreach (var cube in bottomCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                        }
                    }
                }
                for (int i = 0; i < bottomCenterCube.childCount; i++)
                {
                    var c = bottomCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
            case colliderFace.LEFT:
                foreach (var cube in leftCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                        }
                    }
                }
                for (int i = 0; i < leftCenterCube.childCount; i++)
                {
                    var c = leftCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
            case colliderFace.RIGHT:
                foreach (var cube in rightCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                        }
                    }
                }
                for (int i = 0; i < rightCenterCube.childCount; i++)
                {
                    var c = rightCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = Color.white;
                }
                break;
        }
    }

    void highlightFace(colliderFace face)
    {
        switch (face)
        {
            case colliderFace.FRONT:
                foreach (var cube in frontCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for(int i = 0; i<cube.childCount; i++)
                        {
                            var c =cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                        }
                    }
                }
                for (int i = 0; i < frontCenterCube.childCount; i++)
                {
                    var c = frontCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                }
                break;
            case colliderFace.BACK:
                foreach (var cube in backCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                        }
                    }
                }
                for (int i = 0; i < backCenterCube.childCount; i++)
                {
                    var c = backCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                }
                break;
            case colliderFace.TOP:
                foreach (var cube in topCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                        }
                    }
                }
                for (int i = 0; i < topCenterCube.childCount; i++)
                {
                    var c = topCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                }
                break;
            case colliderFace.BOTTOM:
                foreach (var cube in bottomCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                        }
                    }
                }
                for (int i = 0; i < bottomCenterCube.childCount; i++)
                {
                    var c = bottomCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                }
                break;
            case colliderFace.LEFT:
                foreach (var cube in leftCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                        }
                    }
                }
                for (int i = 0; i < leftCenterCube.childCount; i++)
                {
                    var c = leftCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                }
                break;
            case colliderFace.RIGHT:
                foreach (var cube in rightCubes)
                {
                    if (cube.tag != "Player")
                    {
                        for (int i = 0; i < cube.childCount; i++)
                        {
                            var c = cube.GetChild(i);
                            if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                        }
                    }
                }
                for (int i = 0; i < rightCenterCube.childCount; i++)
                {
                    var c = rightCenterCube.GetChild(i);
                    if (c.tag == "Tile") c.GetComponent<Renderer>().material.color = m_highlightColor;
                }
                break;
        }
    }


    bool canFaceRotate(colliderFace face)
    {
        switch (face)
        {
            case colliderFace.TOP:
                if (frontCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && backCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && leftCenterCube.localRotation.eulerAngles.x % 90 == 0
                    && rightCenterCube.localRotation.eulerAngles.x % 90 == 0) return true;
                    break;
            case colliderFace.BOTTOM:
                if (frontCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && backCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && leftCenterCube.localRotation.eulerAngles.x % 90 == 0
                    && rightCenterCube.localRotation.eulerAngles.x % 90 == 0) return true;
                break;
            case colliderFace.LEFT:
                if (frontCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && backCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && topCenterCube.localRotation.eulerAngles.y % 90 == 0
                    && bottomCenterCube.localRotation.eulerAngles.y % 90 == 0) return true;
                break;
            case colliderFace.RIGHT:
                if (frontCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && backCenterCube.localRotation.eulerAngles.z % 90 == 0
                    && topCenterCube.localRotation.eulerAngles.y % 90 == 0
                    && bottomCenterCube.localRotation.eulerAngles.y % 90 == 0) return true;
                break;
            case colliderFace.FRONT:
                if (topCenterCube.localRotation.eulerAngles.y % 90 == 0
                    && bottomCenterCube.localRotation.eulerAngles.y % 90 == 0
                    && leftCenterCube.localRotation.eulerAngles.x % 90 == 0
                    && rightCenterCube.localRotation.eulerAngles.x % 90 == 0) return true;
                break;
            case colliderFace.BACK:
                if (topCenterCube.localRotation.eulerAngles.y % 90 == 0
                    && bottomCenterCube.localRotation.eulerAngles.y % 90 == 0
                    && leftCenterCube.localRotation.eulerAngles.x % 90 == 0
                    && rightCenterCube.localRotation.eulerAngles.x % 90 == 0) return true;
                break;
            default:
                Debug.Log("Unknown face to add the cube to");
                break;
        }
        return false;
    }


    public bool shouldRotateClockwise(int src, int dest)
    {
        if(src < dest)
        {
            if (Mathf.Abs(src - dest) < Mathf.Abs(src - dest + 360)) return true;
            return false;
        }
        else if(src > dest)
        {
            if (Mathf.Abs(dest - src) < Mathf.Abs(dest - src + 360)) return false;
            return true;
        }
        return false;
    }

    void handleToggle()
    {
        isKeyboardPlayed = !isKeyboardPlayed;
    }

    public void setKeyboardPlayed(bool newVal)
    {
        isKeyboardPlayed = newVal;
    }
    public void finish()
    {
        gameMgr.finish();
    }
}