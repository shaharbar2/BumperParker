using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParkingController : MonoBehaviour
{
    public float timeGoal = 5;
    public UnityEvent timeGoalReachedHandler;

    [SerializeField] private Color emptyColor = Color.cyan;
    [SerializeField] private Color parkingColor = Color.green;
    [SerializeField] private Color competingColor = Color.red;
    [SerializeField] private float emissionIntensity = 3f;
    [SerializeField] private float colorChangeSpeed = 0.05f;

    private List<CarController> carsInside;
    private CarController parkedCar;
    private float timer;
    private ParkingState parkingState;
    private Dictionary<ParkingState, Color> stateColors;

    private void Start()
    {
        carsInside = new List<CarController>();
        stateColors = new Dictionary<ParkingState, Color>(){
            {ParkingState.Empty, emptyColor},
            {ParkingState.Parking, parkingColor},
            {ParkingState.Competing, competingColor}
        };
        UpdateColor();
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var carLeaving = other.gameObject.GetComponent<CarController>();
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
        UpdateColor();
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
        car.transform.Find("HoverUI").GetComponent<HoverUIController>().fill = timer / timeGoal;
    }

    private void UpdateColor()
    {
        Color color = stateColors[parkingState];
        StartCoroutine(ChangeColorOverTime(color));
    }

    IEnumerator ChangeColorOverTime(Color color)
    {
        foreach (Transform cube in transform)
        {
            cube.GetComponent<Renderer>().material.color = color;
            cube.GetComponent<Renderer>().material.SetColor("_EmissionColor", color * emissionIntensity);
            yield return new WaitForSeconds(colorChangeSpeed);
        }
    }
}