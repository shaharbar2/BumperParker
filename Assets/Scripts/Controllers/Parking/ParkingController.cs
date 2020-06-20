using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ParkingController : MonoBehaviour
{
    public float timeGoal = 5;
    public UnityEvent timeGoalReachedHandler;

    private PhotonView photonView;
    [SerializeField] private ParkingColor parkingColor;
    private GameManager gameManager;
    private List<PhotonView> carsInside;
    private PhotonView parkedCar;
    private float timer;
    private ParkingState parkingState;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        carsInside = new List<PhotonView>();
        parkingState = ParkingState.Empty;
    }

    private void Update()
    {
        //if 1 car inside start timer
        //if 2 cars pause the timer foreach car
        //on timer reach specified time goal, raise event
        // Debug.Log("Parked car: " + parkedCar.transform.position);
        // Debug.Log("timer: " + timer);
        if (PhotonNetwork.IsMasterClient)
        {
            if (parkingState == ParkingState.Parking)
            {
                timer += Time.deltaTime;
                UpdateCarTimerFill(parkedCar, timer);

                if (timer >= timeGoal)
                {
                    timeGoalReachedHandler.Invoke();
                    // TODO: Lock parking;
                    // maybe save the name of the car player;
                    // maybe change the parking to the color of the winner
                    // Disable the player's movement (because he won) or collision.
                    // Destroy(gameObject);

                    gameManager.CarWon(parkedCar, photonView);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var carEntering = other.gameObject.GetComponent<PhotonView>();
        if (carEntering != null)
        {
            carsInside.Add(carEntering);
            ParkingStateChanged();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var carLeaving = other.gameObject.GetComponent<PhotonView>();
        if (carLeaving != null)
        {
            carsInside.Remove(carLeaving);
            UpdateCarTimerFill(carLeaving, 0);
            ParkingStateChanged();
        }
    }

    private void ParkingStateChanged()
    {
        UpdateParkingState();
        parkingColor.UpdateColor(parkingState);
        UpdateCarTimer();
    }

    private void UpdateParkingState()
    {
        switch (carsInside.Count)
        {
            case 0:
                parkingState = ParkingState.Empty;
                break;
            case 1:
                parkingState = ParkingState.Parking;
                break;
            default:
                parkingState = ParkingState.Competing;
                break;
        }
    }

    private void UpdateCarTimer()
    {
        // if 1 parking save the parking car
        // if more save only the first one parked there
        switch (parkingState)
        {
            case ParkingState.Empty:
                ResetCarTimer(null);
                break;
            case ParkingState.Parking:
                if (parkedCar != carsInside[0])
                {
                    ResetCarTimer(carsInside[0]);
                }
                break;
            case ParkingState.Competing:
                Debug.Log("Competing");
                break;
        }
    }

    private void ResetCarTimer(PhotonView newParkedCar)
    {
        parkedCar = newParkedCar;
        timer = 0;
    }

    private void UpdateCarTimerFill(PhotonView car, float timer)
    {
        car.RPC("UpdateTimer", RpcTarget.AllBuffered, (float)timer / timeGoal);
    }

    [PunRPC]
    public void Destroy()
    {
        Destroy(this);
    }
}