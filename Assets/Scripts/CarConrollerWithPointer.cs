using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarConrollerWithPointer : MonoBehaviour
{
    private Transform pointer;
    private Vector3 offset;
    private float verticalInput;

    public float movementSpeed;
    public float carRotationSpeed;
    public float pointerRotationSpeed;
    public float maxSteeringAngle;

    private void Start()
    {
        pointer = transform.Find("Pointer");
        offset = pointer.position - transform.position;
    }
    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("HorizontalMovement");

        UpdatePointerRotation(horizontalMovement);
        // TODO: Maybe needs to update the pointer angle
        // UpdatePointerAngle(mouseXAxis);
        // TODO: Looks better or not ? - if (verticalInput != 0)
        UpdateWheelPoses();
        if (verticalInput != 0)
        {
            Move();
            LookAtPointer();
        }
    }

    private void UpdatePointerRotation(float horizontalMovement)
    {
        // TODO - maybe set mouse static.

        // float angleToTarget = Vector2.Angle(transform.position, pointer.position);
        // Debug.Log(angleToTarget);
        // float steeringAngle = Mathf.Clamp(angleToTarget, -maxSteeringAngle, maxSteeringAngle);

        // Vector3 newOffset = Quaternion.AngleAxis(mouseXAxis * pointerRotationSpeed, Vector3.up) * offset;
        // if (Vector2.Angle(transform.position, pointer.position + newOffset) <= maxSteeringAngle)
        // {
        //     Debug.Log(Vector2.Angle(transform.position, pointer.position + newOffset));
        //     offset = newOffset;
        //     pointer.position = transform.position + offset;
        // }

        // TODO: Add Time.deltaTime
        // float rotationYInput = mouseXAxis * pointerRotationSpeed;
        // var xQuaternion = Quaternion.AngleAxis(rotationYInput, Vector3.up);
        // Vector3 tempOffset = Quaternion.AngleAxis(rotationYInput, Vector3.up) * offset;
        // if (Vector2.Angle(transform.position, tempOffset) < this.maxSteeringAngle)
        // {
        //     offset = tempOffset;
        //     pointer.position = offset;
        // }

        // TODO: change to update pointer location
        // pointer.position += pointer.forward * movementSpeed * verticalInput * Time.deltaTime;

        offset = Quaternion.AngleAxis(horizontalMovement * pointerRotationSpeed, Vector3.up) * offset;
        pointer.position = transform.position + offset;
    }

    private void LookAtPointer()
    {
        Vector3 lookDirection = pointer.position - transform.position;
        lookDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, carRotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        // TODO: implement reverse
        // transform.position = Vector3.Lerp(transform.position, pointer.position - offset, movementSpeed * Time.deltaTime);
        // transform.position = pointer.position - offset;

        var targetPos = pointer.position;
        targetPos.y = 0;
        transform.position = Vector3.LerpUnclamped(transform.position, targetPos, movementSpeed * verticalInput * Time.deltaTime);
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
        var rot = Quaternion.LookRotation(pointer.position - wheel.position);
        wheel.rotation = rot;
    }
}