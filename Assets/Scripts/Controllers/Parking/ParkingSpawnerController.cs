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
    private List<GameObject> players => GameObject.FindGameObjectsWithTag("Player").ToList();
    // TODO: Change parkings to object pooling

    // The parkings spawner data isnt saved when transferred from previous master, so I must look up all parkings everytime I search them.
    private List<GameObject> currentParkings => GameObject.FindGameObjectsWithTag("Parking").ToList();
    private int spawnRetries = 15;

    void Start()
    {
        multipleTargetCamera = Camera.main.GetComponent<PhotonView>();
        // TODO: add this again once data is transferred.
        // The parkings spawner data isnt saved when transferred from previous master, so I must look up all parkings everytime I search them.
        // currentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        // currentParkings = GameObject.FindGameObjectsWithTag("Parking").ToList();
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
            players.Any(p => Vector3.Distance(p.transform.position, pos) < minimumDistanceFromPlayer) ||
            currentParkings.Any(p => Vector3.Distance(p.transform.position, pos) < minimumDistanceFromParking);

    }

    public void SpawnParking()
    {
        if (!PhotonNetwork.IsMasterClient) return;

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

            // InstantiateSceneObject makes sure that parkings stays even when master leave (ownership is transferred)
            GameObject newParking = PhotonNetwork.InstantiateSceneObject("PhotonPrefabs/Parking", pos, Quaternion.Euler(Vector3.up * Random.Range(0, 359)));
            // TODO: add this again once data is transferred.
            // The parkings spawner data isnt saved when transferred from previous master, so I must look up all parkings everytime I search them.
            // currentParkings.Add(newParking);

            int newParkingViewId = newParking.GetComponent<PhotonView>().ViewID;
            multipleTargetCamera.RPC("AddTarget", RpcTarget.AllBuffered, newParkingViewId);
        }
    }

    public int GetParkingCount() => currentParkings.Count;
    public void RemoveParking(PhotonView parking)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // TODO: add this again once data is transferred.
        // The parkings spawner data isnt saved when transferred from previous master, so I must look up all parkings everytime I search them.
        // currentParkings.Remove(parking.gameObject);
        StartCoroutine(DestroyParking(parking));
        multipleTargetCamera.RPC("RemoveTarget", RpcTarget.AllBuffered, parking.ViewID);
    }

    public void RemoveAllParkings()
    {
        List<int> parkingViewIds = new List<int>();
        foreach (var parking in currentParkings)
        {
            PhotonView parkingPhotonView = parking.GetPhotonView();
            parkingViewIds.Add(parking.GetPhotonView().ViewID);
            StartCoroutine(DestroyParking(parkingPhotonView));
        }
        multipleTargetCamera.RPC("RemoveMultipleTargets", RpcTarget.AllBuffered, parkingViewIds.ToArray());
    }

    private IEnumerator DestroyParking(PhotonView parking)
    {
        // Destroys parking before the parking.gameObject so it wouldn't be playable
        parking.RPC("Destroy", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.Destroy(parking);
    }
}
