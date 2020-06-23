using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarUIController : MonoBehaviourPun
{
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI playersName;
    [SerializeField] private GameObject respawnMsg;

    private void Awake()
    {
        playersName.text = photonView.Owner.NickName;
    }

    public void EnableRespawnMsg()
    {
        respawnMsg.SetActive(true);
    }

    public void DisableRespawnMsg()
    {
        respawnMsg.SetActive(false);
    }

    [PunRPC]
    public void UpdateTimer(float fill)
    {
        timerImage.fillAmount = fill;
    }
}
