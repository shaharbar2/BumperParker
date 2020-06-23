using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class ColorTextByPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    public void UpdateColorByPlayer(Player player)
    {
        _textMesh.color = GetColorByPlayer(player);
    }

    private Color GetColorByPlayer(Player player)
    {
        string usersColorName = (string)player.CustomProperties["CarMaterialColorName"];
        return Resources.Load<Material>($"CarMaterials/{usersColorName}").color;
    }
}
