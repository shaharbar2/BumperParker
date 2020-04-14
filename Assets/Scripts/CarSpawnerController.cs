using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private TextMeshProUGUI joinMessage;
    [SerializeField] private Material blueCar;
    [SerializeField] private Transform car;
    [SerializeField] private float spawnDistance = 4;

    private MultipleTargetCamera multipleTargetCamera;
    private PlayerInputManager pim;
    private List<CarController> players;

    void Start()
    {
        Time.timeScale = 0;
        multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();
        pim = GetComponent<PlayerInputManager>();
        players = new List<CarController>();

        pim.onPlayerJoined += (joinedPlayer) =>
         {
             joinedPlayer.transform.position = new Vector3(joinedPlayer.playerIndex * spawnDistance, 0.55f, 0);
             multipleTargetCamera.AddTarget(joinedPlayer.transform);
             players.Add(joinedPlayer.GetComponent<CarController>());
         };
    }

    void Update()
    {
        if (players.Any((player) => player.ready))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        Time.timeScale = 1;
        pim.DisableJoining();
        joinMessage.gameObject.SetActive(false);
        gameManager.SetActive(true);
        gameObject.SetActive(false);
    }
}
