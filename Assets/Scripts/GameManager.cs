using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PhotonView camera;
    [SerializeField] private ParkingSpawnerController parkingSpawner;
    [SerializeField] private int goalScore = 5;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private float parkingSpawnInterval = 2;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerScorePrefab;
    [SerializeField] private float scoreDistance;
    [SerializeField] private TextMeshProUGUI gameStartCounter;
    [SerializeField] private Material carWonMaterial;

    private PhotonView photonView;
    private Dictionary<string, TextMeshProUGUI> playersScore;
    private bool isGameActive = false;
    private float parkingsMaxAmount = 2;
    private float currentParkingSpawnTimer = 0;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        StartCoroutine(GameStartTimer());
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || !isGameActive) return;

        int parkingCount = parkingSpawner.GetParkingCount();
        if (parkingCount == 0)
        {
            currentParkingSpawnTimer = Mathf.Max(currentParkingSpawnTimer, parkingSpawnInterval / 2);
        }
        if (parkingCount < parkingsMaxAmount)
        {
            if (currentParkingSpawnTimer >= parkingSpawnInterval)
            {
                parkingSpawner.SpawnParking();
                currentParkingSpawnTimer = 0;
            }
            else
            {
                currentParkingSpawnTimer += Time.deltaTime;
            }
        }
    }

    private IEnumerator GameStartTimer()
    {
        // TODO: Consider syncing timer between all clients
        // TODO: The for should be with i = timeTillStart
        for (int i = 3; i > 0; i--)
        {
            gameStartCounter.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

        gameStartCounter.text = "Bumpark!";
        StartGame();
        yield return new WaitForSeconds(1);
        gameStartCounter.text = "";
    }

    private void StartGame()
    {
        isGameActive = true;

        playersScore = PhotonNetwork.PlayerList
            .Select((player, index) => new { player, index })
            .ToDictionary(x => x.player.UserId, x => CreatePlayerScore(x.index, x.player));

        // maxAmount is the amount of players. Minimum 1.
        parkingsMaxAmount = Mathf.Max(playersScore.Count() - 1, 1);

        currentParkingSpawnTimer = parkingSpawnInterval;
    }

    private TextMeshProUGUI CreatePlayerScore(int index, Player player)
    {
        GameObject playerScore = Instantiate(playerScorePrefab, canvas.transform);
        playerScore.GetComponent<RectTransform>().anchoredPosition3D = Vector3.right * index * scoreDistance;
        var playerTextMesh = playerScore.GetComponent<TextMeshProUGUI>();
        playerTextMesh.color = GetColorByPlayer(player);
        return playerTextMesh;
    }

    public void CarWon(PhotonView car, PhotonView parking)
    {
        car.RPC("UpdateTimer", RpcTarget.AllBuffered, (float)0);
        parkingSpawner.RemoveParking(parking);

        // The reason the score is set like that, instead of "AddScore" and then "GetScore"
        //  is that GetScore doesn't retrieve the updated score after using AddScore
        int playersScore = car.Owner.GetScore();
        playersScore++;
        car.Owner.SetScore(playersScore);
        photonView.RPC("UpdateScoreText", RpcTarget.AllBuffered, car.Owner.UserId, playersScore);

        if (playersScore == goalScore)
        {
            gameObject.SetActive(false);
            winCanvas.SetActive(true);
            winCanvas.transform.Find("WinText").GetComponent<TextMeshProUGUI>().color = GetColorByPlayer(car.Owner);
        }
    }

    [PunRPC]
    public void ReloadGame(string scene)
    {
        PhotonNetwork.LoadLevel(scene);
    }

    [PunRPC]
    public void UpdateScoreText(string userId, int score)
    {
        TextMeshProUGUI currentPlayerScoreTextMesh = playersScore[userId];
        currentPlayerScoreTextMesh.text = score.ToString();
    }

    private Color GetColorByPlayer(Player player)
    {
        string usersColorName = (string)player.CustomProperties["CarMaterialColorName"];
        return Resources.Load<Material>($"CarMaterials/{usersColorName}").color;
    }

    // public void CarWon(CarController car, ParkingController parking)
    // {
    //     car.GetComponent<CarMaterial>().ChangeMaterial(carWonMaterial);
    //     car.UpdateTimer(0);
    //     Destroy(car);

    //     parking.ParkingWon();
    //     parking.GetComponent<ParkingColor>().UpdateColor(ParkingState.Won);
    //     Destroy(parking);

    //     camera.RemoveTarget(car.transform);
    //     camera.RemoveTarget(parking.transform);
    // }
}
