using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class ParkingSpawnerController : MonoBehaviour
{
    [SerializeField] private Transform parking;
    [SerializeField] private int maxParkings = 4;

    [SerializeField] private Vector3 size;
    [SerializeField] private float _minimumPlayerDistance;
    [SerializeField] private float _minimumParkingDistance;
    [SerializeField] private Color GizmosColor = new Color(1, 0, 0, 0.2f);

    private PhotonView multipleTargetCamera;
    private List<GameObject> currentPlayers;
    // TODO: Change parkings to object pooling
    private List<GameObject> currentParkings;
    private int spawnRetries = 15;

    void Start()
    {
        multipleTargetCamera = Camera.main.GetComponent<PhotonView>();
        currentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        currentParkings = GameObject.FindGameObjectsWithTag("Parking").ToList();
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
        if (PhotonNetwork.IsMasterClient)
        {
            int retriesCount = 0;
            float currentMinimumDistanceFromParking = _minimumParkingDistance;

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
                } while (IsPositionInvalid(pos, _minimumPlayerDistance, currentMinimumDistanceFromParking));

                GameObject newParking = PhotonNetwork.Instantiate("PhotonPrefabs/Parking", pos, Quaternion.Euler(Vector3.up * Random.Range(0, 359)));
                // TODO: Make sure only the master holds the parkings
                currentParkings.Add(newParking);

                int newParkingViewId = newParking.GetComponent<PhotonView>().ViewID;
                multipleTargetCamera.RPC("AddTarget", RpcTarget.AllBuffered, newParkingViewId);
            }
        }
    }

    public int GetParkingCount() => currentParkings.Count;
    public void RemoveParking(ParkingController parking)
    {
        parking.GetComponent<ParkingColor>().UpdateColor(ParkingState.Won);
        currentParkings.Remove(parking.gameObject);
        // Destroys parking before the parking.gameObject so it wouldn't be playable
        Destroy(parking);
        Destroy(parking.gameObject, 1.5f);
    }
}
