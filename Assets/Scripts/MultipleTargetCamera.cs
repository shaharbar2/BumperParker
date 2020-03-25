using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;

    [SerializeField] private float smoothTime = .5f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minZoomY = 15;
    [SerializeField] private float maxZoomY = 35;
    [SerializeField] private float minZoomZ = -15;
    [SerializeField] private float maxZoomZ = -25;
    [SerializeField] private float zoomNormalizer = 100;

    private Camera cam;
    private Bounds bounds;
    private Vector3 moveVelocity;
    private Vector3 zoomVelocity;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0) return;
        UpdateBounds();

        Move();
        Zoom();
    }

    private void UpdateBounds()
    {
        // TODO: Consider optimizing it by using a single bounds object and update only if needed
        bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (Transform target in targets)
        {
            bounds.Encapsulate(target.position);
        }
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newCamPos = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newCamPos, ref moveVelocity, smoothTime);
    }

    private void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float newZoomY = Mathf.Lerp(minZoomY, maxZoomY, greatestDistance * zoomNormalizer);
        float newZoomZ = Mathf.Lerp(minZoomZ, maxZoomZ, greatestDistance * zoomNormalizer);
        // Debug.Log(newZoom);
        // cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
        // var tmp = new Vector3(transform.position.x, 0, transform.position.z);
        // var tmp = new Vector3(transform.position.x, 0, GetEdgeZ() + offset.z);
        var tmp = new Vector3(transform.position.x,
         newZoomY,
          GetEdgeZ() + newZoomZ);
        transform.position = tmp;
        // transform.position = tmp + Vector3.up * newZoom;
        // Debug.Log(transform.position);
        // Debug.Log(tmp + Vector3.up * newZoom);
        // Debug.Log("New Zoom: " + newZoom);
        Debug.Log(offset.z);
        Debug.Log(bounds.size.z);
        Debug.Log(offset.z + bounds.size.z);
    }

    private Vector3 GetCenterPoint() => bounds.center;

    private float GetEdgeZ()
    {
        return GetCenterPoint().z - bounds.size.z / 2;
    }

    private float GetGreatestDistance()
    {
        return Mathf.Max(bounds.size.x, bounds.size.z);
    }
}
