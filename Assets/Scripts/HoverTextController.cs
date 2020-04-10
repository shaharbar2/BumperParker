using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverTextController : MonoBehaviour
{
    public string text;
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(camera.transform.forward);
        TextMeshProUGUI textMesh = transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
    }
}
