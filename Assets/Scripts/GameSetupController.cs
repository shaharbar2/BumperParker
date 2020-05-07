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

    private MultipleTargetCamera multipleTargetCamera;
    private List<CarController> players;

    void Start()
    {
        gameManager.SetActive(true);
        multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();


        GameObject joinedPlayer = PhotonNetwork.Instantiate("PhotonPrefabs/Carv2", Vector3.zero, Quaternion.identity);
        int playerIndex = Array.FindIndex(PhotonNetwork.PlayerList, p => p.IsLocal);
        joinedPlayer.transform.position = new Vector3(playerIndex * spawnDistance, 0.55f, 0);
        multipleTargetCamera.AddTarget(joinedPlayer.transform);
        int materialIndex = (playerIndex + 1 % carMaterials.Count) - 1;
        joinedPlayer.GetComponent<CarMaterial>().ChangeMaterial(carMaterials[materialIndex]);
    }

    // void Start()
    // {
    //     Time.timeScale = 0;
    //     multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();
    //     players = GameObject.FindObjectsOfType(typeof(CarController)).Select((g) => ((GameObject)g).GetComponent<CarController>()).ToList();
    //     CreatePlayer();
    // }

    // private void CreatePlayer()
    // {
    //     GameObject joinedPlayer = PhotonNetwork.Instantiate("PhotonPrefabs/Carv2", Vector3.zero, Quaternion.identity);
    //     int playerIndex = Array.FindIndex(PhotonNetwork.PlayerList, p => p.IsLocal);
    //     joinedPlayer.transform.position = new Vector3(playerIndex * spawnDistance, 0.55f, 0);
    //     multipleTargetCamera.AddTarget(joinedPlayer.transform);
    //     players.Add(joinedPlayer.GetComponent<CarController>());

    //     // This would allow infinite indexes. It'll repeat the colors when it reaches the final index.
    //     int materialIndex = (playerIndex + 1 % carMaterials.Count) - 1;
    //     joinedPlayer.GetComponent<CarMaterial>().ChangeMaterial(carMaterials[materialIndex]);
    // }

    // private void Update()
    // {
    //     if (players.Any((player) => player.ready))
    //     {
    //         StartGame();
    //     }
    // }

    // private void StartGame()
    // {
    //     Time.timeScale = 1;
    //     joinMessage.gameObject.SetActive(false);
    //     gameManager.SetActive(true);
    //     gameObject.SetActive(false);
    // }
}
