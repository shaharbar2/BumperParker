using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConrollerv2 : MonoBehaviour
{
    private Rigidbody rb;
    private float steeringAngle;
    private float horizontalInput;
    private float verticalInput;

    public Transform wheels;
    public float maxSteerAngle = 40;
    public float acceleration = 50;
    public float rotationSpeed = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // targetRotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        GetInput();
        if (IsGrounded())
        {
            // Steer();
            // Accelerate();

            var forwardVelocity = transform.InverseTransformDirection(rb.velocity).z;

            // Steer
            steeringAngle = maxSteerAngle * horizontalInput;
            var rotateMultiplier = Mathf.Clamp(forwardVelocity * 0.1f, -1, 1);
            rb.rotation *= Quaternion.Euler(Vector3.up * steeringAngle * rotationSpeed * rotateMultiplier * Time.deltaTime);

            // Move Forward
            rb.AddRelativeForce(Vector3.forward * verticalInput * acceleration * Time.deltaTime, ForceMode.VelocityChange);

            foreach (Transform wheel in wheels)
            {
                if (wheel.tag == "FrontWheel")
                {
                    wheel.localEulerAngles = Vector3.up * steeringAngle;
                }
                wheel.GetChild(0).Rotate(Vector3.right * forwardVelocity * 2, Space.Self);
            }
        }

        Brake();
        UpdateWheelPoses();
    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private bool IsGrounded()
    {
        Debug.Log("Is Grounded = " + Physics.Raycast(rb.position, -rb.transform.up, 0.5f));
        return Physics.Raycast(rb.position, -rb.transform.up, 1f);
    }

    private void Steer()
    {
        // //transform.InverseTransformDirection(rb.velocity).normalized
        // //Vector3.Dot(rb.velocity, rb.transform.forward)
        // // var velocity = rb.velocity.normalized.x + rb.velocity.normalized.z;
        // // var velocity = Mathf.Abs(rb.velocity.normalized.x) + Mathf.Abs(rb.velocity.normalized.z);
        // var velocity = Mathf.Abs(Vector3.Dot(rb.velocity, Vector3.forward));
        // Debug.Log("Velocity: " + velocity);
        // // Debug.Log(Vector3.Dot(rb.velocity, rb.transform.forward));

        // // if (velocity != 0)
        // // {
        // steeringAngle = maxSteerAngle * horizontalInput;
        // // Quaternion deltaRotation = Quaternion.Euler(Vector3.up * steeringAngle);
        // // rb.MoveRotation(rb.rotation * deltaRotation);

        // // if (velocity > 0.0001f)
        // // {
        // // verticalInput is added to lower the rotation when slowing down
        // // Quaternion deltaRotation = Quaternion.Euler(Vector3.up * steeringAngle * steeringSpeed * Time.deltaTime);
        // // rb.rotation = Quaternion.RotateTowards(rb.rotation, rb.rotation * deltaRotation, maxSteerAngle);

        // Quaternion deltaRotation = Quaternion.Euler(Vector3.up * steeringAngle * steeringSpeed * Time.deltaTime);
        // // rb.rotation = Quaternion.RotateTowards(rb.rotation, rb.rotation * deltaRotation, Mathf.Lerp(maxSteerAngle * velocity, maxSteerAngle, velocity));
        // // Debug.Log("Yeet " + Mathf.Lerp(velocity, 100, velocity) * 0.01f);
        // // rb.rotation = Quaternion.Lerp(transform.rotation, rb.rotation * deltaRotation, Mathf.Lerp(0, maxSteerAngle, Mathf.Lerp(0, 1, velocity * Time.deltaTime)));

        // var steeringLerpT = velocity / maxSteerAngle;
        // var rotationLerp = Mathf.Lerp(0, maxSteerAngle, steeringLerpT);
        // Debug.Log("steeringLerpT " + steeringLerpT);
        // Debug.Log("rotationLerp " + rotationLerp);
        // rb.rotation = Quaternion.Lerp(transform.rotation, rb.rotation * deltaRotation, rotationLerp);
    }

    private void Accelerate()
    {
        // if (IsGrounded())
        // {
        //     rb.AddRelativeForce(Vector3.forward * verticalInput * motorForce * Time.deltaTime, ForceMode.VelocityChange);
        // }
    }

    private void Brake()
    {
        // GetComponent<Rigidbody>().velocity *= 0.9f;
    }

    private void UpdateWheelPoses()
    {
        var wheels = GameObject.FindGameObjectsWithTag("FrontWheel");
        foreach (var wheel in wheels)
        {
            UpdateWheelPose(wheel.transform);
        }
    }

    private void UpdateWheelPose(Transform wheel)
    {
        // wheel.localRotation = Quaternion.AngleAxis(steeringAngle, Vector3.up);
    }
}