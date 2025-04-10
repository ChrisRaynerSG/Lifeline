using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    // No target object for the main camera as it is free flying
    // This script allows the camera to move freely in the scene
    // The camera will be controlled by the user using the keyboard
    // The camera will be able to move forward, backward, left, right, up, and down
    // The camera will also be able to rotate around the Y axis and zoom in and out
    // The camera will be able to reset its position and rotation
    // The camera will be able to move in the direction it is facing
    // The camera will be able to move at a speed of 10 units per second

    public Camera mainCamera;
    
    [SerializeField]
    [Range(0, 100)]
    // Speed of the camera movement
    // This is a serialized field so it can be adjusted in the Unity editor
    float speed = 0.5f;
    float maxRampedUpSpeed = 2f;
    float initialSpeed = 1f;
    bool isShiftPressed = false;
    bool rampingUpSpeed = false;


    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            StartCoroutine(RampUpSpeed());
            MoveCamera(0, 0, 0.1f* speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            StartCoroutine(RampUpSpeed());
            MoveCamera(0, 0, -0.1f * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            StartCoroutine(RampUpSpeed());
            MoveCamera(-0.1f * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            StartCoroutine(RampUpSpeed());
            MoveCamera(0.1f * speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            StartCoroutine(RampUpSpeed());
            MoveCamera(0, 0.1f * (speed/2), 0);
        }
        if(Input.GetKey(KeyCode.X))
        {
            StartCoroutine(RampUpSpeed());
            MoveCamera(0, -0.1f * (speed/2), 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            RotateCamera(0, 1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            RotateCamera(0, -1);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            ZoomCamera(-1);
        }
        if (Input.GetKey(KeyCode.X))
        {
            ZoomCamera(1);
        }
        if (Input.GetKey(KeyCode.R))
        {
            ResetCamera();
        }

        if (Input.GetKey(KeyCode.LeftShift) && !isShiftPressed)
        {
            isShiftPressed = true;
            speed *= 2f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && isShiftPressed)
        {
            isShiftPressed = false;
            speed = initialSpeed;
        }
        if(Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            // these are the wrong way around, but thats how it works in Unity for some reason?
            RotateCamera(-mouseY,-mouseX);
        }
        if(Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            MoveCamera(-mouseX * speed, -mouseY * speed, 0);
        }
        // if no keys are pressed, reset the speed to the initial speed
        if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftControl))
        {
            speed = initialSpeed;
            rampingUpSpeed = false;
        }
    }

    void RotateCamera(float x, float y)
    {

        // I dont want the z axis to rotate at all, yet it will with these below lines
        // Rotate the camera around the Y axis
        mainCamera.transform.Rotate(0, -y, 0);
        // Rotate the camera around the X axis
        mainCamera.transform.Rotate(x, 0, 0);

        //make sure the camera does not rotate around the Z axis
        mainCamera.transform.rotation  = Quaternion.Euler(mainCamera.transform.rotation.eulerAngles.x, mainCamera.transform.rotation.eulerAngles.y, 0);
        
    }
    void MoveCamera(float x, float y, float z)
    {
        // Move the camera in the direction it is facing
        mainCamera.transform.Translate(x, y, z) ;
    }

    void ZoomCamera(float zoom)
    {
        // Zoom the camera in or out
        mainCamera.fieldOfView += zoom;
    }
    void ResetCamera()
    {
        // Reset the camera to its original position and rotation
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    IEnumerator RampUpSpeed()
    {
        if(rampingUpSpeed){
            yield break;
        }
        // Ramp up the speed of the camera
        while (speed < maxRampedUpSpeed)
        {
            // ensure that only one coroutine is running at a time
            // this prevents the speed from ramping up too quickly
            rampingUpSpeed = true;
            speed += Time.deltaTime * 5f;
            if(speed > maxRampedUpSpeed)
            {
                // ensure that the speed does not exceed the maximum ramped up speed
                // this prevents the speed from ramping up too quickly
            speed = maxRampedUpSpeed;
            }  
            // need to reset speed to initial speed when the camera is not moving
            // and kill the coroutine if we don't reach max speed before the user stops pressing keys
            if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftControl))
            {
                speed = initialSpeed;
                rampingUpSpeed = false;
                yield break;
            }
            // wait for the next frame before continuing
            // this prevents the speed from ramping up too quickly
            // this allows the speed to ramp up smoothly
            yield return null;
        }
        rampingUpSpeed = false;
    }
}
