using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    //changeable but i'm not sure what sensitivity it directly correlates to
    public float mouseSensitivity = 400f;
    //declaring the playerbody function for rotation
    public Transform playerBody;

    //start the xRotation at 0
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //locks the cursor to the center of the screen and makes it invisible (will have to be changed for scene switch)
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //taking input from the x and y axis and putting it into a float for calculations
        //always multiply by Time.deltaTime to make sure it is framerate dependent
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        //Locks the rotation of camera so that you cannot move your camera past 90 degrees down and 90 degrees up
        xRotation = Mathf.Clamp(xRotation, -90F, 90F);
        
        //Actual portion of the script dedicated to rotating the player perspective.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
