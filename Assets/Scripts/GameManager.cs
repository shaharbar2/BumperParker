using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MultipleTargetCamera camera;
    [SerializeField] private ParkingSpawnerController parkingSpawner;
    [SerializeField] private Material carWonMaterial;

    [SerializeField] private float parkingSpawnInterval = 2;

    private float parkingsMaxAmount = 2;
    private float currentParkingSpawnTimer = 0;

    void Start()
    {
        GameObject[] playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        Transform[] playersTransforms = playersGameObjects.Select(go => go.transform).ToArray();
        camera.AddTargets(playersTransforms);

        parkingsMaxAmount = playersGameObjects.Count() + 5;

        // TODO: screen timer 3, 2, 1...

        // TODO: spawn 1 parking
        // afterwards spawn parking over time or when all parkings are taken
        parkingSpawner.SpawnParking();
    }

    void Update()
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
