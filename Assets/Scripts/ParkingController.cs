using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingController : MonoBehaviour
{
    private List<CarControllerv2> carsInside;

    [SerializeField] private Color empty;
    [SerializeField] private Color parking;
    [SerializeField] private Color competing;

    private void Start()
    {
        carsInside = new List<CarControllerv2>();
        UpdateColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        var carEntering = other.gameObject.GetComponent<CarControllerv2>();
        if (carEntering != null)
        {
            carsInside.Add(carEntering);
            UpdateColor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var carLeaving = other.gameObject.GetComponent<CarControllerv2>();
        if (carLeaving != null)
        {
            carsInside.Remove(carLeaving);
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        switch (carsInside.Count)
        {
            case 0:
                ChangeColor(empty);
                break;
            case 1:
                ChangeColor(parking);
                break;
            default:
                ChangeColor(competing);
                break;
        }
    }

    private void ChangeColor(Color color)
    {
        foreach (Transform cube in transform)
        {
            cube.GetComponent<Renderer>().material.color = color;
        }
    }
}