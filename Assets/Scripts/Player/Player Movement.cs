using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float cameraSensitivity = 100f;
    public float movementSpeed = 10f;
    public float runMultiplier = 1.4f;
    public float jumpForce = 5f;
    public float smoothTransition = 0.1f;
    public Vector3 startingPosition; // Store the starting position
    public TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI object

    private Rigidbody rb;
    private Vector3 velocity;
    private float speed;
    private float gravity = -9.81f;
    private bool isGrounded;
    private Vector3 cameraOffset;
    private Vector3 zoomOffset = new Vector3(0, 0.8f, 0);
    private float zoomLerpSpeed = 10f;

    private float xRotation = 0f;
    private float timer = 0f;
    private bool isTimerRunning = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze rotation to prevent unwanted rotation

        cameraOffset = cameraTransform.localPosition;
        speed = movementSpeed;
        Cursor.lockState = CursorLockMode.Locked;

        // Store the starting position
        startingPosition = transform.position;

        // Initialize the timer text if it's assigned
        if (timerText != null)
        {
            timerText.text = "Time: 0";
        }
    }

    void Update()
    {
        // Camera Rotation
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        move = transform.TransformDirection(move);
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);

        // Run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = movementSpeed * runMultiplier;
        }
        else
        {
            speed = movementSpeed;
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * gravity), ForceMode.VelocityChange);
        }

        // Camera Zoom
        if (Input.GetMouseButton(1))
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, zoomOffset, Time.deltaTime * zoomLerpSpeed);
        }
        else
        {
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraOffset, Time.deltaTime * zoomLerpSpeed);
        }

        // Timer
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = "TIME: " + Mathf.FloorToInt(timer).ToString();
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the player is grounded
        isGrounded = true;
    }

    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the tag "DeathPit"
        if (collision.gameObject.CompareTag("DeathPit"))
        {
            // Teleport the player back to the starting position
            transform.position = startingPosition;
        }

        // Check if the collided object has the tag "Finish"
        if (collision.gameObject.CompareTag("Finish"))
        {
            // Teleport the player back to the starting position
            transform.position = startingPosition;
            // Stop the timer
            isTimerRunning = false;
        }
    }
}
