using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private TextMeshProUGUI joinMessage;
    [SerializeField] private List<Material> carMaterials;
    [SerializeField] private float spawnDistance = 4;

    // private MultipleTargetCamera multipleTargetCamera;
    private PhotonView multipleTargetCamera;
    private List<CarController> players;

    void Start()
    {
        gameManager.SetActive(true);
        // multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();
        multipleTargetCamera = Camera.main.GetComponent<PhotonView>();

        int playerIndex = Array.FindIndex(PhotonNetwork.PlayerList, p => p.IsLocal);
        var playersPos = new Vector3(playerIndex * spawnDistance, 0.55f, 0);
        GameObject joinedPlayer = PhotonNetwork.Instantiate("PhotonPrefabs/Carv2", playersPos, Quaternion.identity);

        var joinedPlayerPhotonView = joinedPlayer.GetComponent<PhotonView>();

        multipleTargetCamera.RPC("AddTarget", RpcTarget.AllBuffered, joinedPlayerPhotonView.ViewID);

        int materialIndex = (playerIndex + 1 % carMaterials.Count) - 1;
        joinedPlayerPhotonView.RPC("ChangeMaterial", RpcTarget.AllBuffered, carMaterials[materialIndex].name);
    }

}
