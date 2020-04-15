using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverUIController : MonoBehaviour
{
    public string text;
    public float fill;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Image imageScript;
    private Camera camera;
    private Vector3 offset;

    void Start()
    {
        camera = Camera.main;
        offset = transform.parent.position + transform.localPosition;
    }

    void Update()
    {
        transform.position = transform.parent.position + offset;
        transform.rotation = Quaternion.LookRotation(camera.transform.forward);
        if (textMesh != null)
        {
            textMesh.text = text;
        }
        if (imageScript != null)
        {
            imageScript.fillAmount = fill;
        }
    }
}
