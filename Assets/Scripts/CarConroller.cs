using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConroller : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;

    public WheelCollider flWheel, frWheel, rlWheel, rrWheel;
    public Transform flWheelT, frWheelT, rlWheelT, rrWheelT;
    public float maxSteerAngle = 30;
    public float motorForce = 50;

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        Brake();
        UpdateWheelPoses();
    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        steeringAngle = maxSteerAngle * horizontalInput;
        flWheel.steerAngle = steeringAngle;
        frWheel.steerAngle = steeringAngle;
    }

    private void Accelerate()
    {
        flWheel.motorTorque = verticalInput * motorForce;
        frWheel.motorTorque = verticalInput * motorForce;
        rlWheel.motorTorque = verticalInput * motorForce;
        rrWheel.motorTorque = verticalInput * motorForce;
    }

    private void Brake()
    {
        // GetComponent<Rigidbody>().velocity *= 0.9f;
    }

    private void UpdateWheelPoses()
    {
        UpdateWheelPose(flWheelT, flWheel);
        UpdateWheelPose(frWheelT, frWheel);
        UpdateWheelPose(rlWheelT, rlWheel);
        UpdateWheelPose(rrWheelT, rrWheel);
    }

    private void UpdateWheelPose(Transform transform, WheelCollider collider)
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;

        collider.GetWorldPose(out pos, out quat);
        transform.position = pos;
        transform.rotation = quat;
    }
}