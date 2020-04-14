using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MultipleTargetCamera camera;
    [SerializeField] private float parkingSpawnInterval = 2;
    [SerializeField] private ParkingSpawnerController parkingSpawner;
    [SerializeField] private Material carWonMaterial;
    [SerializeField] private TextMeshProUGUI gameStartCounter;

    private bool isGameActive = false;
    private List<GameObject> players;
    private float parkingsMaxAmount = 2;
    private float currentParkingSpawnTimer = 5;

    void Start()
    {
        players = new List<GameObject>();
        // pim.onPlayerJoined += (player) => Debug.Log("Player joined " + player);
        // TODO: screen timer 3, 2, 1...

        // TODO: spawn 1 parking
        // afterwards spawn parking over time or when all parkings are taken
        StartCoroutine(GameStartTimer());
    }

    void Update()
    {
        // Debug.Log("Player Count: " + GetComponent<PlayerInputManager>().playerCount);

        if (isGameActive)
        {
            if (parkingSpawner.GetParkingCount() < parkingsMaxAmount)
            {
                // Debug.Log(currentParkingSpawnTimer);
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
        gameStartCounter.text = "3";
        yield return new WaitForSeconds(1);
        gameStartCounter.text = "2";
        yield return new WaitForSeconds(1);
        gameStartCounter.text = "1";
        yield return new WaitForSeconds(1);
        gameStartCounter.text = "Start!";
        isGameActive = true;
        yield return new WaitForSeconds(1);
        gameStartCounter.text = "";
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        Transform[] playersTransforms = players.Select(go => go.transform).ToArray();
        camera.AddTargets(playersTransforms);
        // parkingsMaxAmount = players.Count() - 1;
        // TODO: remove
        parkingsMaxAmount = players.Count() + 3;
    }

    public void CarWon(CarController car, ParkingController parking)
    {
        car.GetComponent<CarMaterial>().ChangeMaterial(carWonMaterial);
        car.UpdateTimer(0);
        Destroy(car);

        parking.ParkingWon();
        parking.GetComponent<ParkingColor>().UpdateColor(ParkingState.Won);
        Destroy(parking);

        camera.RemoveTarget(car.transform);
        camera.RemoveTarget(parking.transform);
    }
}
