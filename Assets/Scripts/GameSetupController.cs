using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
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
        // TODO: make sure win menu appears for everyone

        gameManager.SetActive(true);
        // multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();
        multipleTargetCamera = Camera.main.GetComponent<PhotonView>();
        int playerIndex = Array.FindIndex(PhotonNetwork.PlayerList, p => p.IsLocal);
        var playersPos = new Vector3(playerIndex * spawnDistance, 0.55f, 0);
        GameObject joinedPlayer = PhotonNetwork.Instantiate("PhotonPrefabs/Carv2", playersPos, Quaternion.identity);
        joinedPlayer.name = PhotonNetwork.LocalPlayer.UserId;

        var joinedPlayerPhotonView = joinedPlayer.GetComponent<PhotonView>();
        joinedPlayerPhotonView.Owner.SetScore(0);
        multipleTargetCamera.RPC("AddTarget", RpcTarget.AllBuffered, joinedPlayerPhotonView.ViewID);

        int materialIndex = (playerIndex + 1 % carMaterials.Count) - 1;
        string carMaterialColorName = carMaterials[materialIndex].name;
        joinedPlayerPhotonView.RPC("ChangeMaterial", RpcTarget.AllBuffered, carMaterialColorName);
        Hashtable playerCustomProperties = new Hashtable();
        playerCustomProperties.Add("CarMaterialColorName", carMaterialColorName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject("PhotonPrefabs/ParkingSpawner", playersPos, Quaternion.identity);
        }
    }

}
