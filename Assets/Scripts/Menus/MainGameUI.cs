using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MainGameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void ExitGame()
    {
        gameManager.Leave();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
