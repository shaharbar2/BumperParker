using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverUIController : MonoBehaviour
{
    public string text;
    public float fill;

    private Camera camera;
    private Vector3 offset;

    void Start()
    {
        camera = Camera.main;
        offset = transform.localPosition;
    }

    void Update()
    {
        transform.position = transform.parent.position + offset;
        transform.rotation = Quaternion.LookRotation(camera.transform.forward);
    }
}
