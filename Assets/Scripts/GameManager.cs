using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MultipleTargetCamera camera;
    [SerializeField] private ParkingSpawnerController parkingSpawner;
    [SerializeField] private int goalScore = 5;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private float parkingSpawnInterval = 2;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject playerScorePrefab;
    [SerializeField] private float scoreDistance;
    [SerializeField] private TextMeshProUGUI gameStartCounter;
    [SerializeField] private Material carWonMaterial;

    Dictionary<GameObject, TextMeshProUGUI> playersScore;
    private bool isGameActive = false;
    private float parkingsMaxAmount = 2;
    private float currentParkingSpawnTimer = 0;

    void Start()
    {
        //StartCoroutine(GameStartTimer());
        StartGame();
    }

    void Update()
    {
        if (isGameActive)
        {
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
    }

    private IEnumerator GameStartTimer()
    {
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

        playersScore = GameObject.FindGameObjectsWithTag("Player")
            .Select((player, index) => new { player, index })
            .ToDictionary(x => x.player, x => CreatePlayerScore(x.index, x.player));

        // maxAmount is the amount of players. Minimum 1.
        parkingsMaxAmount = Mathf.Max(playersScore.Count() - 1, 1);

        currentParkingSpawnTimer = parkingSpawnInterval;
    }

    private TextMeshProUGUI CreatePlayerScore(int index, GameObject player)
    {
        GameObject playerScore = Instantiate(playerScorePrefab, canvas.transform);
        playerScore.GetComponent<RectTransform>().anchoredPosition3D = Vector3.right * index * scoreDistance;
        var playerTextMesh = playerScore.GetComponent<TextMeshProUGUI>();
        playerTextMesh.color = player.GetComponent<CarMaterial>().GetColor();
        return playerTextMesh;
    }

    public void CarWon(CarController car, ParkingController parking)
    {
        car.UpdateTimer(0);
        car.AddReward(100);
        
        //camera.RemoveTarget(parking.transform);
        parkingSpawner.RemoveParking(parking);

        TextMeshProUGUI currentPlayerScoreTextMesh = playersScore[car.gameObject];
        int currentPlayerScore = int.Parse(currentPlayerScoreTextMesh.text) + 1;
        currentPlayerScoreTextMesh.text = currentPlayerScore.ToString();

        if (currentPlayerScore == goalScore)
        {
            Time.timeScale = 0;
            winCanvas.SetActive(true);
            winCanvas.transform.Find("WinText").GetComponent<TextMeshProUGUI>().color = currentPlayerScoreTextMesh.color;
        }
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
