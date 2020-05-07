using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public bool ready = false;

    [SerializeField] private Transform wheels;
    [SerializeField] private HoverUIController hoverUI;
    [SerializeField] private float maxSteerAngle = 40;
    [SerializeField] private float acceleration = 50;
    [SerializeField] private float rotationSpeed = 5;
    [SerializeField] private float drag = 3;
    [SerializeField] private float boost = 1.2f;
    [SerializeField] private float brake = 3;
    [SerializeField] private float normalMultiplyer = 0.02f;
    [SerializeField] private float collisionPower = 30;
    [SerializeField] private float respawnTimer = 3;

    private PhotonView photonView;
    private Rigidbody rb;
    private float steeringAngle;
    private float forwardVelocity;
    private float horizontalInput = 0;
    private float verticalInput = 0;
    private bool brakeInput = false;
    private float brakeForceInput = 0;
    private bool boostInput = false;
    private bool respawnInput = false;
    private float offGroundTimer = 0;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Respawn();
            UpdateBrakeForceInput();
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
    }

    public void UpdateTimer(float fill)
    {
        hoverUI.fill = fill;
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

    void OnStartGame(InputValue value)
    {
        ready = true;
    }

    void OnAcceleration(InputValue value)
    {
        verticalInput = value.Get<float>();
    }

    void OnSteering(InputValue value)
    {
        horizontalInput = value.Get<float>();
    }

    void OnBrakeDrift(InputValue value)
    {
        brakeInput = value.isPressed;
    }

    void OnBoost(InputValue value)
    {
        boostInput = value.isPressed;
    }

    void OnRespawn(InputValue value)
    {
        respawnInput = value.isPressed;
    }

    private void Respawn()
    {
        if (!IsGrounded())
        {
            if (offGroundTimer >= respawnTimer)
            {
                hoverUI.text = "R/Square to respawn";
                if (respawnInput)
                {
                    rb.rotation = Quaternion.identity;
                    rb.position = new Vector3(0, 0.55f, 0);
                    rb.velocity = Vector3.zero;
                    offGroundTimer = 0;
                    hoverUI.text = "";
                }
            }
            else
            {
                offGroundTimer += Time.deltaTime;
            }
        }
        else
        {
            offGroundTimer = 0;
            hoverUI.text = "";
        }
    }


    private void UpdateBrakeForceInput()
    {
        // Workaround to allow raising brakeForce over time with the new InputManager
        //  (Currently keyboard has no sensitivity/gravity)
        float brakeForceDeltaUpdate = brakeInput ? Time.deltaTime * 2 : -Time.deltaTime * 3;
        brakeForceInput = Mathf.Clamp(brakeForceInput + brakeForceDeltaUpdate, 0, 1);
    }

    private void Steer()
    {
        // TODO: Consider rotating around pivot
        var rotateMultiplier = Mathf.Clamp(forwardVelocity * 0.1f, -1, 1);
        rotateMultiplier *= Mathf.Lerp(1, 2, brakeForceInput);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * steeringAngle * rotationSpeed * rotateMultiplier * normalMultiplyer));
    }

    private void Accelerate()
    {
        // rb.AddForceAtPosition(Vector3.forward * verticalInput * acceleration * normalMultiplyer, Vector3.back, ForceMode.VelocityChange);
        var accelerationForce = verticalInput * acceleration * normalMultiplyer;
        if (boostInput)
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
        float brakeForce = 1 - (brakeForceInput * brake) / 100;
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