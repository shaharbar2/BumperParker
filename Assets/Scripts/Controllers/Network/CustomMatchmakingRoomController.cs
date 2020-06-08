using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomMatchmakingRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int multiPlayerSceneIndex;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private Transform playersContainer;
    [SerializeField] private GameObject playerListingPrefab;
    [SerializeField] private TextMeshProUGUI roomNameDisplay;

    public override void OnJoinedRoom()
    {
        roomPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomNameDisplay.text = PhotonNetwork.CurrentRoom.Name;

        startBtn.SetActive(PhotonNetwork.IsMasterClient);
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();

        // Activates start btn if the local player is the new master client
        startBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiPlayerSceneIndex);
        }
    }

    public void Leave()
    {
        // TODO: Consider clearing the player list here. ClearPlayerListings();
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        PhotonNetwork.LeaveRoom();

        // TODO: Check if it's still needed. The original issue by the original dev was not receiving updated rooms after leaving hosting room.
        PhotonNetwork.LeaveLobby();
        StartCoroutine(RejoinLobby());
    }

    IEnumerator RejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }

    private void UpdatePlayerList()
    {
        ClearPlayerListings();
        ListPlayers();
    }

    private void ClearPlayerListings()
    {
        foreach (Transform playerListing in playersContainer)
        {
            Destroy(playerListing.gameObject);
        }
    }

    private void ListPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListing = Instantiate(playerListingPrefab, playersContainer);
            TextMeshProUGUI playerName = playerListing.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerName.text = player.NickName;
        }
    }
}
