using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkingSpawnerController : MonoBehaviour
{
    [SerializeField] private Transform parking;
    [SerializeField] private int maxParkings = 4;

    [SerializeField] private Vector3 size;
    [SerializeField] private float minimumDistanceFromPlayer;
    [SerializeField] private float minimumDistanceFromParking;
    [SerializeField] private Color GizmosColor = new Color(1, 0, 0, 0.2f);

    private List<GameObject> currentPlayers;
    // TODO: Change parkings to object pooling
    private List<GameObject> currentParkings;

    void Start()
    {
        currentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        currentParkings = GameObject.FindGameObjectsWithTag("Parking").ToList();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Players: " + currentPlayers.Count);
        Debug.Log("Parkings: " + currentParkings.Count);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnParking();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawCube(transform.localPosition, size);
    }



    private Vector3 GetRandomPosition()
    {
        return transform.position +
                        new Vector3(
                            Random.Range(-size.x / 2, size.x / 2),
                            0,
                            Random.Range(-size.z / 2, size.z / 2));
    }
    private bool IsPositionInvalid(Vector3 pos)
    {
        return
            currentPlayers.Any(p => Vector3.Distance(p.transform.position, pos) < minimumDistanceFromPlayer) ||
            currentParkings.Any(p => Vector3.Distance(p.transform.position, pos) < minimumDistanceFromParking);

    }
    public void SpawnParking()
    {
        // TODO: change it to work with object pooling
        if (currentParkings.Count < 4)
        {
            Vector3 pos;
            do
            {
                pos = GetRandomPosition();
            } while (IsPositionInvalid(pos));

            var newParking = Instantiate(parking, pos, Quaternion.Euler(Vector3.up * Random.Range(0, 359)));
            currentParkings.Add(newParking.gameObject);
        }
    }

}
