using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private PhotonView multipleTargetCamera;
    [SerializeField] private int goalScore = 5;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private float parkingSpawnInterval = 2;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerScorePrefab;
    [SerializeField] private float scoreDistance;
    [SerializeField] private TextMeshProUGUI gameStartCounter;
    [SerializeField] private Material carWonMaterial;

    private ParkingSpawnerController parkingSpawner;

    private Dictionary<string, TextMeshProUGUI> playersScore;
    private bool isGameActive = false;
    private float parkingsMaxAmount = 2;
    private float currentParkingSpawnTimer = 0;

    void Start()
    {
        StartCoroutine(GameStartTimer());
        if (PhotonNetwork.IsMasterClient)
        {
            EnableRestartMenu();
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || !isGameActive) return;
        if (parkingSpawner == null)
        {
            // TODO: Instantiating the gamemanager as a scene object, and transfering it's ownership would allow having the parking spawner initalized within already.
            //  It would also allow having 1 parkingSpawnTimer, 
            //      and keeping the same maxAmount for everyone - On CarLeave add the following
            //      parkingsMaxAmount = Mathf.Max(parkingsMaxAmount - 1, 1);
            //      
            //  to do so, the gamestartertimer logic should be transferred somewhere else
            parkingsMaxAmount = Mathf.Max(playersScore.Count() - 1, 1);
            parkingSpawner = GameObject.FindObjectOfType<ParkingSpawnerController>();
        }

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            EnableRestartMenu();
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

        if (PhotonNetwork.IsMasterClient)
        {
            // TODO: should be removed once gamemanager data is syncronized between all clients.
            currentParkingSpawnTimer = parkingSpawnInterval;
        }
    }

    private TextMeshProUGUI CreatePlayerScore(int index, Player player)
    {
        GameObject playerScore = Instantiate(playerScorePrefab, canvas.transform);
        UpdateScorePosition(playerScore, index);
        var playerTextMesh = playerScore.GetComponent<TextMeshProUGUI>();
        playerTextMesh.GetComponent<ColorTextByPlayer>().UpdateColorByPlayer(player);
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
            parkingSpawner.RemoveAllParkings();
            photonView.RPC("SetGameOver", RpcTarget.AllBuffered, car.Owner);
        }
    }

    public void Leave()
    {
        Player player = PhotonNetwork.LocalPlayer;
        GameObject playersCar = GameObject.Find(player.UserId);
        var playersCarPhotonView = playersCar.GetComponent<PhotonView>();
        photonView.RPC("PlayerLeft", RpcTarget.AllBuffered, player);
        multipleTargetCamera.RPC("RemoveTarget", RpcTarget.AllBuffered, playersCarPhotonView.ViewID);
    }

    [PunRPC]
    public void SetGameOver(Player winner)
    {
        isGameActive = false;
        winCanvas.SetActive(true);
        var winTextMesh = winCanvas.transform.Find("WinText").GetComponent<TextMeshProUGUI>();
        winTextMesh.GetComponent<ColorTextByPlayer>().UpdateColorByPlayer(winner);
        winTextMesh.text = $"{winner.NickName} Won !";
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

    [PunRPC]
    public void PlayerLeft(Player player)
    {
        StartCoroutine(PlayerLeftScoreCourotine(player.UserId));
    }

    private void EnableRestartMenu()
    {
        winCanvas.transform.Find("WaitForHostText").gameObject.SetActive(false);
        winCanvas.transform.Find("WinMenu").gameObject.SetActive(true);
    }

    private IEnumerator PlayerLeftScoreCourotine(string userId)
    {
        TextMeshProUGUI currentPlayerScoreTextMesh = playersScore[userId];
        var oldFontSize = currentPlayerScoreTextMesh.fontSize;

        currentPlayerScoreTextMesh.text = "PlayerLeft";
        currentPlayerScoreTextMesh.fontSize = 14;
        playersScore.Remove(userId);
        // TODO: Remove it from here, this is temporarly here until figured how to syncornize data OnOwnershipTransfered
        //     Should syncronize parkingsMax, parkingspawner players and parkings, and currentParkingSpawnTimer
        //      :should be removed once gamemanager data is syncronized between all clients.
        parkingsMaxAmount = Mathf.Max(playersScore.Count() - 1, 1);
        yield return new WaitForSeconds(3);

        Destroy(currentPlayerScoreTextMesh);
        int index = 0;
        foreach (KeyValuePair<string, TextMeshProUGUI> score in playersScore)
        {
            UpdateScorePosition(score.Value.gameObject, index);
            index++;
        }
    }

    private void UpdateScorePosition(GameObject score, int index)
    {
        score.GetComponent<RectTransform>().anchoredPosition3D = Vector3.right * index * scoreDistance;
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
