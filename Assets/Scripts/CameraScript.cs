using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;                    //target to follow
    public float targetHeight = 1.7f;           //Vertical offset adjustment
    public float distance = 12.0f;              //Default distance
    public float offsetFromWall = 0.1f;         //Bring camera away from any colliding pbjects
    public float maxDistance = 20.0f;           //Maximum zoom distance
    public float minDistance = 0.6f;            //Minimum zoom distance
    public float xSpeed = 200.0f;               //Orbit speed (Left/Right)
    public float ySpeed = 200.0f;               //Orbit speed (Up/Down)
    public float yMinLimit = -80.0f;            //Looking up limit
    public float xMinLimit = 80.0f;             //Looking down limit
    public float zoomRate = 40.0f;              //Zoom speed
    public float rotationDampening = 3.0f;      //Auto rotation speed (highter = faster)
    public float zoomDampening = 5.0f;          //Auto zoom speed (higher = faster)

    LayerMask collisionLayers = -1;             //What the camera will collide with

    public bool lockToRearOfTarget;
    public bool allowMouseInputX = true;
    public bool allowMouseInputY = true;

    float xDeg = 0.0f;
    float yDeg = 0.0f;
    float currentDistance;
    public float desiredDistance;
    float correctedDistance;
    bool rotateBehind;

    public GameObject userModel;
    public bool inFirstPerson;
    Rigidbody rb;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;
        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        if (rb)
            rb.freezeRotation = true;
        if (lockToRearOfTarget)
            rotateBehind = true;
    }
    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (inFirstPerson == true)
            {
                minDistance = 10;
                desiredDistance = 15;
                userModel.SetActive(true);
                inFirstPerson = false;
            }
        }
        if (desiredDistance == 10)
        {
            minDistance = 0;
            desiredDistance = 0;
            userModel.SetActive(false);
            inFirstPerson = true;
        }
    }
    private void LateUpdate()
    {
        if (!target)
            return;

        //Vector3 vTargetOffset3;
        //If either mouse buttons are down, left the mouse govern camera position
        if (GUIUtility.hotControl == 0)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Debug.Log("Aprieto el control izquierdo");
            }
            else
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    //Check to see if mouse input allowed on the axis
                    if (allowMouseInputX)
                        xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                    if (allowMouseInputY)
                        yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                }
            }
        }
        ClampAngle(yDeg);
        //Set camera rotation
        Quaternion quaternionRotation = Quaternion.Euler(yDeg, xDeg, 0);

        //Calculate the desired distance
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;

        //Calculate desired camera position
        Vector3 vTargetOffset = new Vector3(0, -targetHeight, 0);
        Vector3 position = target.position - (quaternionRotation * Vector3.forward * desiredDistance + vTargetOffset);

        //Check for collision using the true target's desired registration point as set by user using height
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);

        //If there was a collision, correct the camera position and calculate the corrected distance
        bool isCorrected = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers))
        {
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
            isCorrected = true;
        }

        //For smoothing, lerp distance only if eihter distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

        //Keep within limits
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        //Recalculate position based on the new currentDistance
        position = target.position - (quaternionRotation * Vector3.forward * currentDistance + vTargetOffset);

        //Finally set rotation and position of camera
        transform.rotation = quaternionRotation;
        transform.position = position;
    }

    void ClampAngle(float angle)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        yDeg = Mathf.Clamp(angle, -60, 80);
    }
}
