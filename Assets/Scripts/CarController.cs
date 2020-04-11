using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    private float steeringAngle;
    private float forwardVelocity;
    private float horizontalInput;
    private float verticalInput;
    private float brakeInput;


    public Material material;
    [SerializeField] private Transform wheels;
    [SerializeField] private float maxSteerAngle = 40;
    [SerializeField] private float acceleration = 50;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float drag = 3;
    [SerializeField] private float boost = 1.2f;
    [SerializeField] private float brake = 3;
    [SerializeField] private float normalMultiplyer = 0.02f;
    [SerializeField] private float collisionPower = 30;

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
            Accelerate();
            Steer();
            Drag();
        }

        Brake();
        UpdateWheelPoses();
        AddGravity();
    }

    private void OnCollisionEnter(Collision other)
    {
        var otherRb = other.gameObject.GetComponent<Rigidbody>();
        if (otherRb != null)
        {
            // otherRb.AddForce((otherRb.position - rb.position) * collisionPower, ForceMode.Impulse);
            otherRb.AddForceAtPosition(Vector3.one * collisionPower, otherRb.position, ForceMode.Impulse);
        }
    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis("HorizontalMovement");
        verticalInput = Input.GetAxis("Vertical");
        brakeInput = Input.GetAxis("Brake");
    }

    private void Steer()
    {
        var rotateMultiplier = Mathf.Clamp(forwardVelocity * 0.1f, -1, 1);
        rotateMultiplier *= Mathf.Lerp(1, 2, brakeInput);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * steeringAngle * rotationSpeed * rotateMultiplier * normalMultiplyer));
    }

    private void Accelerate()
    {
        // rb.AddForceAtPosition(Vector3.forward * verticalInput * acceleration * normalMultiplyer, Vector3.back, ForceMode.VelocityChange);
        var accelerationForce = verticalInput * acceleration * normalMultiplyer;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            accelerationForce *= boost;
        }
        rb.AddRelativeForce(Vector3.forward * accelerationForce, ForceMode.VelocityChange);
    }

    private void Drag()
    {
        // TODO: Consider lowering the drag (suggested by Benjamin)
        float dragForce = 1 - drag / 100;
        var vel = rb.velocity;
        vel.x *= dragForce;
        vel.z *= dragForce;
        rb.velocity = vel;
    }

    private void Brake()
    {
        float brakeForce = 1 - (brakeInput * brake) / 100;
        var vel = rb.velocity;
        vel.x *= brakeForce;
        vel.z *= brakeForce;
        rb.velocity = vel;
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

    private void AddGravity()
    {
        rb.AddForce(Physics.gravity * rb.mass);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(rb.position, -rb.transform.up, 0.5f);
    }
}