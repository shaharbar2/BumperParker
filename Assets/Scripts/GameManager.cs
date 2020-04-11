using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MultipleTargetCamera camera;
    [SerializeField] private ParkingSpawnerController parkingSpawner;
    [SerializeField] private Material carWonMaterial;

    void Start()
    {
        GameObject[] playersGameObjects = GameObject.FindGameObjectsWithTag("Player");
        Transform[] playersTransforms = playersGameObjects.Select(go => go.transform).ToArray();
        camera.AddTargets(playersTransforms);


        // TODO: spawn 1 parking
        // afterwards spawn parking over time or when all parkings are taken
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
