using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingColor : MonoBehaviour
{
    [SerializeField] private Color emptyColor = Color.cyan;
    [SerializeField] private Color parkingColor = Color.green;
    [SerializeField] private Color competingColor = Color.red;
    [SerializeField] private Color wonColor = Color.white;

    [SerializeField] private float emissionIntensity = 3f;
    [SerializeField] private float colorChangeSpeed = 0.05f;

    private Dictionary<ParkingState, Color> stateColors;

    void Awake()
    {
        stateColors = new Dictionary<ParkingState, Color>(){
            {ParkingState.Empty, emptyColor},
            {ParkingState.Parking, parkingColor},
            {ParkingState.Competing, competingColor},
            {ParkingState.Won, wonColor}
        };
    }

    public void UpdateColor(ParkingState parkingState)
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
