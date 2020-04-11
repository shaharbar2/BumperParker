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

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
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
