using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //declaring playercontroller for movement (player controller is the parent object to the capsule, main camera, etc.)
    public CharacterController controller;
    public LayerMask groundMask;
    public Transform groundCheck;
    
    //possibly upgradeable float stats
    //delay per tick of stamina regening (may not be in seconds)
    public float staminaRegenRate = 2f;
    //delay per tick of stamine decreasing (may not be in seconds)
    public float staminaDecreaseRate = 2f; 
    //multiplier that the players speed is multiplied by to give their sprint speed
    public float sprintMultiplier = 1.5f;
    //multiplier that the players speed is multiplied by to give the player their speed while crouching
    public float crouchMultiplier = 0.5f;
    //max stamina for the player (should not be modified within this script)
    public float maxStamina = 8f;
    /*upgradeable value/changeable value that stores the players base speed. Should not be changed by this script and only called on to return the
    player to their original speed*/
    public float playerSpeed = 5f;
    

    //changeable values, but should stay consistent once correct values are found
    //gravity is used to increase velocity / simulate gravity
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    /*the distance the ground must be for the groundchecker (invisible child object under the player that generates a sphere to check for ground) 
    to determine if the plyer is touching the ground*/
    public float groundDistance = 0.4f;
    //normalHeight for the height while not crouching and crouchheight for the height while crouching
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    //speed variable that is used entirely for this script (playerSpeed is used as the way to store the players actual speed while this one changes)
    public float speed = 5f;
    //stamina will have to be set equal to maximum stamina at some point in the process
    public int stamina = 0;
    

    //added variable for gravity (gravity increases this number to create exponential drop/realistic gravity)
    Vector3 velocity;
    //added for jump function as well as use of tools/items (things you would need to be on the ground to use)
    bool isGrounded;
    //unneccessary in this script, but added so that other scripts can refer to this boolean to see if the player is currently sprinting
    bool isRunning;

    // Update is called once per frame
    void Update()
    {
        //groundchecker generating a sphere to check for the given ground layer, within the ground distance float, and if it's there, setting isgrounded to true
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        //removes any velocity when the player is on the ground to prevent player from constantly building up velocity while standing on the ground
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        /*finding input from WASD/arrow keys (keys return a -1 to 1 value where negative one is a pressed down, and positive one is d pressed down 
        (example only applies for horizontal))*/
        float x = Input.GetAxis("Horizontal");
        float z = Input. GetAxis("Vertical");
        
        //defining the function for move for use on the controller (y is not used because x and z are the two values that determine horizontal and vertical movement)
        Vector3 move = transform.right * x + transform.forward * z;

        //actual function that causes the player to move, multiplied by Time.deltaTime so it's framerate dependent
        controller.Move(move * speed * Time.deltaTime);

        //obviously the jump function
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //slightly more complicated but allows us to input the value in arbitrary units that we want the player to jump and have it do the math for us
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //crouch function (when player is crouching)
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            //sets the height of the actual colliders/controller to the designated crouch height
            controller.height = crouchHeight;
            /*sets the player speed to their current speed * their crouch multiplier, could go based off pre saved value but that would make it more difficult
            to add other functions such as the player being injured causing them to move slower. Only problem is this may cause the player to move faster while
            sprinting and crouching at the same time.*/
            speed = speed * crouchMultiplier;
        }
        
        //crouch function (when player stops crouching)
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            //sets players height to their dedicated normal height
            controller.height = normalHeight;
            //returns players speed to the saved value (upgradable variable)
            speed = playerSpeed;
        }

        //sprinting script to change speed and cause stamina drain
        if(Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0)
        {
            isRunning = true;
            //multipliers player speed by sprintmultiplier (allows singular speed value to be upgradable for sprint as well)
            speed = speed * sprintMultiplier;
            //Calls for a repeating function named "staminaDecrease" with 0 delay, and at the stamina decrease rate specified in the float.
            InvokeRepeating("staminaDecrease", 0f, staminaDecreaseRate);
            //cancels the repeating function that acts as stamina regeneration so the stamina can successfully drain.
            CancelInvoke("staminaIncrease");
        }
        //checks for stamina being equal to zero and returns the player to a walk
        if(stamina == 0)
        {
            isRunning = false;
            speed = playerSpeed;
            CancelInvoke("staminaDecrease");
            if(IsInvoking("staminaIncrease") == false)
            {
            InvokeRepeating("staminaIncrease", 1f, staminaRegenRate);
            }
        }
        //sprinting script for when player stops sprinting to change speed and cause stamina regeneration
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            //sets player speed back to their saved value (playerSpeed is the upgradable variable)
            speed = playerSpeed;
            //cancels the stamina decrease repeating function so player can regenerate stamina
            CancelInvoke("staminaDecrease");
            //calls for a repeating function called "staminaIncrease" with 0 delay, with a delay per completion (in seconds) according to the staminaRegenRate float
            if(IsInvoking("staminaIncrease") == false)
            InvokeRepeating("staminaIncrease", 1f, staminaRegenRate);
        }

        //line that changes the value of velocity based off the value of gravity and makes it framerate dependent
        velocity.y += gravity * Time.deltaTime;
        //line that actually applys the force to the player controller and makes it framerate dependent
        controller.Move(velocity * Time.deltaTime);
    }
    //Repeating function for stamina regeneration when player is not sprinting
    void staminaIncrease()
    {
        //checks if player's stamina is lower than max, and if they are currently not running
        if(stamina < maxStamina && isRunning == false)
        {
        //increases the stamina value by 1 (script dependent and shouldnt be modified by outside scripts (minus items that might restore stamina))
        stamina++;
        }
    }
    //repeating function for stamina decreasing while the player is sprinting
    void staminaDecrease()
    {
        /*checks if the player has stamina to spend and is currently sprinting (may need to be changed so player cannot rapidly tap shift to 
        not expend any stamina while sprinting)*/
        if(stamina > 0 && isRunning == true)
        {
        //decreases the stamina value by 1 (script dependent and shouldnt be modified by outside scripts (minus actions like taking damage or falling))
        stamina--;
        }
    }
}
