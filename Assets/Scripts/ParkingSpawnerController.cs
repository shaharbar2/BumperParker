using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkingSpawnerController : MonoBehaviour
{
    [SerializeField] private Transform parking;
    [SerializeField] private int maxParkings = 4;

    [SerializeField] private Vector3 size;
    [SerializeField] private float _minimumDistanceFromPlayer;
    [SerializeField] private float _minimumDistanceFromParking;
    [SerializeField] private Color GizmosColor = new Color(1, 0, 0, 0.2f);

    private MultipleTargetCamera multipleTargetCamera;
    private List<GameObject> currentPlayers;
    // TODO: Change parkings to object pooling
    private List<GameObject> currentParkings;
    private int spawnRetries = 15;

    void Start()
    {
        multipleTargetCamera = Camera.main.GetComponent<MultipleTargetCamera>();
        currentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        currentParkings = GameObject.FindGameObjectsWithTag("Parking").ToList();
    }

    // Update is called once per frame
    void Update()
    {
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

    private bool IsPositionInvalid(Vector3 pos, float minimumDistanceFromPlayer, float minimumDistanceFromParking)
    {
        return
            currentPlayers.Any(p => Vector3.Distance(p.transform.position, pos) < minimumDistanceFromPlayer) ||
            currentParkings.Any(p => Vector3.Distance(p.transform.position, pos) < minimumDistanceFromParking);

    }

    public void SpawnParking()
    {
        int retriesCount = 0;
        float currentMinimumDistanceFromParking = _minimumDistanceFromParking;

        // TODO: change it to work with object pooling
        if (currentParkings.Count < maxParkings)
        {
            Vector3 pos;
            do
            {
                // lowers the distance allowed from parking if retries has reached the limit
                if (retriesCount == spawnRetries && currentMinimumDistanceFromParking > 0)
                {
                    currentMinimumDistanceFromParking--; // Changing to /= 2 might be nicer
                    retriesCount = 0;
                }
                pos = GetRandomPosition();
                retriesCount++;
            } while (IsPositionInvalid(pos, _minimumDistanceFromPlayer, currentMinimumDistanceFromParking));

            var newParking = Instantiate(parking, pos, Quaternion.Euler(Vector3.up * Random.Range(0, 359)));
            currentParkings.Add(newParking.gameObject);


            multipleTargetCamera.AddTarget(newParking);
        }
    }

    public void RemoveParking(GameObject parking)
    {
        currentParkings.Remove(parking);
    }

}
