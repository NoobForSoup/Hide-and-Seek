using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public IngamePlayer player;

    public float sensitivity = 3.4f;
    public float movementSpeed = 4f;
    public float runningSpeed = 8f;
    public float jumpHeight = 3f;

    public Camera camera;
    private CharacterController cc;

    private float yRotation = 0f;

    private float gravity = -9.81f;
    public Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundLayer;

    private bool isGrounded;

    private bool moveable = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        InputHandling();
    }

    public void SetMoveable(bool moveable)
    {
        this.moveable = moveable;
    }
    
    private void InputHandling()
    {
        //groundcheck
        isGrounded = Physics.CheckBox(groundCheck.position, new Vector3(0.3f, groundDistance, 0.3f), Quaternion.identity, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //rotation

        if (moveable)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * 50f * Time.deltaTime;
            float mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y") * sensitivity * 50f * Time.deltaTime, -90f, 90f);

            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            transform.Rotate(Vector3.up * mouseX);
            camera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

            //movement
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 move = transform.right * horizontal + transform.forward * vertical;

            float speed = movementSpeed;

            if (Input.GetKey(KeyBinds.Run))
            {
                speed = runningSpeed;
            }

            if(player.attacking)
            {
                speed /= 2;
            }

            if(player.hurt)
            {
                speed *= 1.2f;
            }

            cc.Move(move * speed * Time.deltaTime);

            //jump
            if (Input.GetKeyDown(KeyBinds.Jump) && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }
}
