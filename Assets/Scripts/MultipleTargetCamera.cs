using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;

    // [SerializeField] private float smoothTime = .5f;
    [SerializeField] private float minZoomY = 15;
    [SerializeField] private float maxZoomY = 35;
    [SerializeField] private float minZoomZ = -15;
    [SerializeField] private float maxZoomZ = -25;
    [SerializeField] private float zoomNormalizer = 100;

    private Camera cam;
    private Bounds bounds;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0) return;
        UpdateBounds();

        Move();
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
        float greatestDistance = GetGreatestDistance();
        float newZoomY = Mathf.Lerp(minZoomY, maxZoomY, greatestDistance * zoomNormalizer);
        float newZoomZ = Mathf.Lerp(minZoomZ, maxZoomZ, greatestDistance * zoomNormalizer);
        var newCamPos = new Vector3(
            GetCenterPoint().x,
            newZoomY,
            GetEdgeZ() + newZoomZ);
        transform.position = newCamPos;
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
