using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void ExitGame()
    {
        GameObject playersCar = GameObject.Find(PhotonNetwork.LocalPlayer.UserId);
        var playersCarPhotonView = playersCar.GetComponent<PhotonView>();
        gameManager.CarLeave(playersCarPhotonView);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
