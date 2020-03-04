using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConroller1 : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;
    public float steeringSpeed = 30;
    public float speed = 50;

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        steeringAngle = horizontalInput * steeringSpeed * Time.deltaTime;
        GetComponent<Rigidbody>().AddRelativeTorque(0, steeringAngle, 0, ForceMode.VelocityChange);
    }

    private void Accelerate()
    {
        var appliedForce = verticalInput * speed * Time.deltaTime;
        GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, appliedForce, ForceMode.VelocityChange);
        Debug.Log(appliedForce);

    }

    private void UpdateWheelPoses()
    {
        // UpdateWheelPose(flWheelT, flWheel);
        // UpdateWheelPose(frWheelT, frWheel);
        // UpdateWheelPose(rlWheelT, rlWheel);
        // UpdateWheelPose(rrWheelT, rrWheel);
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