using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float jumpSpeed = 30.0f;
    public float gravity = 55.0f;
    public float runSpeed = 70.0f;
    public float runSpeed2 = 70.0f;
    public float runSpeed3 = 140.0f;
    float walkSpeed = 90.0f;
    float rotateSpeed = 150.0f;

    public bool grounded;
    Vector3 moveDirection = Vector3.zero;
    bool isWalking;
    string moveStatus = "idle";

    public GameObject cam;
    public CharacterController controller;
    public bool isJumping;
    public float myAng = 0.0f;
    public bool canJump = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        if (myAng > 50)
            canJump = false;
        else
            canJump = true;
        if (grounded)
        {
            isJumping = false;
            if (cam.transform.gameObject.transform.GetComponent<CameraScript>().inFirstPerson == true)
            {
                moveDirection = new Vector3((Input.GetMouseButton(0) ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));
            }
            moveDirection = new Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0), 0, Input.GetAxis("Vertical"));

            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= isWalking ? walkSpeed : runSpeed;

            moveStatus = "idle";

            if (moveDirection != Vector3.zero)
                moveStatus = isWalking ? "walking" : "running";
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                moveDirection.y = jumpSpeed;
                isJumping = true;
            }


            //Allow turning at anytime. Keep the character facing in the same direction as the camera ir the right mouse button is down
            if (cam.transform.GetComponent<CameraScript>().inFirstPerson == false)
            {
                if (Input.GetMouseButton(1))
                    transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                else
                    transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
            }
            else
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                    transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }
            //Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;

            //Move Controller
            CollisionFlags flags;
            if (isJumping)
                flags = controller.Move(moveDirection * Time.deltaTime);
            else
                flags = controller.Move((moveDirection + new Vector3(0, -100, 0)) * Time.deltaTime);

            grounded = (flags & CollisionFlags.Below) != 0;
        }
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            myAng = Vector3.Angle(Vector3.up, hit.normal);
        }
    }
}
