using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingController : MonoBehaviour
{
    private List<CarController> carsInside;

    [SerializeField] private Color empty = Color.cyan;
    [SerializeField] private Color parking = Color.green;
    [SerializeField] private Color competing = Color.red;
    [SerializeField] private float emissionIntensity = 3f;
    [SerializeField] private float colorChangeSpeed = 0.05f;

    private void Start()
    {
        carsInside = new List<CarController>();
        UpdateColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        var carEntering = other.gameObject.GetComponent<CarController>();
        if (carEntering != null)
        {
            carsInside.Add(carEntering);
            UpdateColor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var carLeaving = other.gameObject.GetComponent<CarController>();
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
        // PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        // volume.profile.GetSetting<Bloom>().color.value = color * 3;
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