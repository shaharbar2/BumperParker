using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    [SerializeField] private PhotonView gameManager;

    public void ReloadGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Apparently if you try to reload the same current scene it only works on the master
            gameManager.RPC("ReloadGame", RpcTarget.All, SceneManager.GetActiveScene().name);
        }
    }
}
