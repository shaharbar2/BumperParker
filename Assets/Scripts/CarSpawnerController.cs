using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSpawnerController : MonoBehaviour
{
    [SerializeField] private Material blueCar;
    [SerializeField] private PlayerInputManager pim;

    [SerializeField] private Transform car;
    [SerializeField] private InputActionAsset carController;

    private MultipleTargetCamera multipleTargetCamera;

    void Start()
    {
        multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();
        // var newCar = Instantiate(car, Vector3.zero, Quaternion.identity);
        // newCar.GetComponent<PlayerInput>().actions = carController;
        // multipleTargetCamera.AddTarget(newCar);
        // var newCar1 = Instantiate(car, Vector3.right * 3, Quaternion.identity);
        // newCar1.GetComponent<PlayerInput>().actions = carController;
        // multipleTargetCamera.AddTarget(newCar1);
        var player = pim.JoinPlayer();
        multipleTargetCamera.AddTarget(player.transform);
        var player1 = pim.JoinPlayer();
        player1.GetComponent<CarMaterial>().ChangeMaterial(blueCar);
        multipleTargetCamera.AddTarget(player1.transform);

        //pim.JoinPlayer();

        // var newCar2 = Instantiate(car, Vector3.right * 6, Quaternion.identity);
        // multipleTargetCamera.AddTarget(newCar2);
        // currentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        // currentParkings = GameObject.FindGameObjectsWithTag("Parking").ToList();
    }
}
