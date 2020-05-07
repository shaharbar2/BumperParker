using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomMatchmakingLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject lobbyConnectBtn;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Transform roomsContainer;
    [SerializeField] private GameObject roomListingPrefab;

    private string roomName;
    private int roomSize;
    private List<RoomInfo> availableRooms;

    private const string NickNameKey = "NickName";

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Whenever Master Client calls PhotonNetwork.LoadScene all clients load to the same scene as the Master Client
        lobbyConnectBtn.SetActive(true);
        availableRooms = new List<RoomInfo>();

        PhotonNetwork.NickName = PlayerPrefs.GetString(NickNameKey, RandomPlayerName());
        playerNameInput.text = PhotonNetwork.NickName;
    }

    public void PlayerNameUpdate(string nameInput)
    {
        string name = nameInput != "" ? nameInput : RandomPlayerName();
        PhotonNetwork.NickName = name;
        PlayerPrefs.SetString(NickNameKey, name);
    }

    public void JoinLobby()
    {
        mainPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (availableRooms != null)
        {
            ClearRoomListings();
        }
        ListRooms(roomList);
    }

    public void OnRoomNameChanged(string nameInput)
    {
        roomName = nameInput;
    }

    public void OnRoomSizeChanged(string sizeInput)
    {
        roomSize = int.Parse(sizeInput);
    }

    public void CreateRoom()
    {
        Debug.Log("Creating room");
        var roomOps = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)roomSize
        };
        PhotonNetwork.CreateRoom(roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // TODO: show error msg for the player
        Debug.Log($"Room Creation Failed.\nReturn Code: {returnCode}\nMessage: {message}.");
    }

    public void MatchmakingCancel()
    {
        mainPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        PhotonNetwork.LeaveLobby();
    }

    private string RandomPlayerName() => "Player " + Random.Range(0, 1000);

    private void ClearRoomListings()
    {
        foreach (Transform roomListing in roomsContainer)
        {
            Destroy(roomListing);
        }
    }

    private void ListRooms(List<RoomInfo> updatedRooms)
    {
        foreach (RoomInfo room in updatedRooms)
        {
            availableRooms.Add(room);
            ListRoom(room);
        }
    }

    private void ListRoom(RoomInfo room)
    {
        if (room.IsOpen && room.IsVisible && room.PlayerCount > 0)
        {
            GameObject roomListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton roomButton = roomListing.GetComponent<RoomButton>();
            roomButton.SetRoom(room.Name, room.MaxPlayers, room.PlayerCount);
        }
    }
}
