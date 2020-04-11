using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMaterial : MonoBehaviour
{
    [SerializeField] private Material material;
    void Start()
    {
        ChangeChildMaterial("Classic_16_Body");
        ChangeChildMaterial("Classic_16_Door_L");
        ChangeChildMaterial("Classic_16_Door_R");
        ChangeChildMaterial("Classic_16_Hood");
        ChangeChildMaterial("Classic_16_Roof");
        ChangeChildMaterial("Classic_16_Trunk");
        ChangeChildMaterial("Classic_16_Bumper_B_3");
        ChangeChildMaterial("Classic_16_Bumper_F_3");
    }

    private void OnValidate()
    {
        Start();
    }

    private void ChangeChildMaterial(string childName)
    {
        Transform child = gameObject.transform.Find(childName);
        var childMesh = child.GetComponent<MeshRenderer>();
        childMesh.material = material;
    }

}
