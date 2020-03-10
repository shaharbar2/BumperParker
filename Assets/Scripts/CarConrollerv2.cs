using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConrollerv2 : MonoBehaviour
{
    private Rigidbody rb;
    private float steeringAngle;
    private float forwardVelocity;
    private float horizontalInput;
    private float verticalInput;

    public Transform wheels;
    public float maxSteerAngle = 40;
    public float acceleration = 50;
    public float rotationSpeed = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        GetInput();
        steeringAngle = maxSteerAngle * horizontalInput;
        forwardVelocity = transform.InverseTransformDirection(rb.velocity).z;
        if (IsGrounded())
        {
            Steer();
            Accelerate();
        }

        // TODO: Implement Brake();
        UpdateWheelPoses();
    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        var rotateMultiplier = Mathf.Clamp(forwardVelocity * 0.1f, -1, 1);
        rb.rotation *= Quaternion.Euler(Vector3.up * steeringAngle * rotationSpeed * rotateMultiplier * Time.deltaTime);
    }

    private void Accelerate()
    {
        rb.AddRelativeForce(Vector3.forward * verticalInput * acceleration * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void Brake()
    {
        // GetComponent<Rigidbody>().velocity *= 0.9f;
    }

    private void UpdateWheelPoses()
    {
        foreach (Transform wheel in wheels)
        {
            if (wheel.tag == "FrontWheel")
            {
                wheel.localEulerAngles = Vector3.up * steeringAngle;
            }
            // TODO: Allow wheel rotation with velocity even if not moving (upside down)
            wheel.GetChild(0).Rotate(Vector3.right * forwardVelocity * 2, Space.Self);
        }
    }

    private bool IsGrounded()
    {
        Debug.Log("Is Grounded = " + Physics.Raycast(rb.position, -rb.transform.up, 0.5f));
        return Physics.Raycast(rb.position, -rb.transform.up, 1f);
    }
}