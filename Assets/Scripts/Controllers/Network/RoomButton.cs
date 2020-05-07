using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomButton : MonoBehaviour
{
    // TODO: check if using ScriptableObject here can be helpful

    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomSizeText;

    private string roomName;
    private int roomSize;
    private int playerCount;

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void SetRoom(string roomNameInput, int roomSizeInput, int playerCountInput)
    {
        roomName = roomNameInput;
        roomSize = roomSizeInput;
        playerCount = playerCountInput;

        roomNameText.text = roomName;
        roomSizeText.text = $"{playerCountInput}/{roomSize}";
    }
}
