using System;
using System.Collections;
using System.Collections.Generic;
using Shahar.Bar.ML;
using UnityEngine;
using UnityEngine.Events;

public class ParkingController : MonoBehaviour
{
    public float timeGoal = 5;
    public UnityEvent timeGoalReachedHandler;

    [SerializeField] private ParkingColor parkingColor;
    private GameManager gameManager;
    private List<CarController> carsInside;
    private CarController parkedCar;
    private float timer;
    private ParkingState parkingState;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        carsInside = new List<CarController>();
        parkingState = ParkingState.Empty;
        FindObjectOfType<CarAgent>().parkingLocation = transform;
    }

    private void Update()
    {
        //if 1 car inside start timer
        //if 2 cars pause the timer foreach car
        //on timer reach specified time goal, raise event
        // Debug.Log("Parked car: " + parkedCar.transform.position);
        // Debug.Log("timer: " + timer);

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

                gameManager.CarWon(parkedCar, this);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var carEntering = other.gameObject.GetComponent<CarController>();
        if (carEntering != null)
        {
            carsInside.Add(carEntering);
            ParkingStateChanged();
            carEntering.AddReward(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var carLeaving = other.gameObject.GetComponent<CarController>();
        if (carLeaving != null)
        {
            carLeaving.AddReward(-1);

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

    private void ResetCarTimer(CarController newParkedCar)
    {
        parkedCar = newParkedCar;
        timer = 0;
    }

    private void UpdateCarTimerFill(CarController car, float timer)
    {
        car.UpdateTimer(timer / timeGoal);
    }
}