using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyManagerScript : MonoBehaviourPunCallbacks
{
    public InputField roomInputField;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;
    public RoomItemScript roomItemPrefab;
    List<RoomItemScript> roomItemsList = new List<RoomItemScript>();
    public Transform contentObject;
    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;
    public List<PlayerItemScript>  playerItemsList = new List<PlayerItemScript>();
    public PlayerItemScript playerItemPrefab;
    public Transform playerItemparent;
    public GameObject playButton;

    private void Start() {
        PhotonNetwork.JoinLobby();
    }
    private void Update() {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 2)
        {
            playButton.SetActive(true);
        }else{
            playButton.SetActive(false);
        }
    }

    public void OnClickCreate() {
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true });
            print("Creo la sala" + roomInputField.text);
        }
    }

    public override void OnJoinedRoom() 
    {
        print("entro a la sala");
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }

    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItemScript item in roomItemsList)
        {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();
        print("paso el clear");
        foreach (RoomInfo room in list)
        {
            print("room");
            print(room.Name);
            RoomItemScript newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }
    void UpdatePlayerList()
    {
        foreach (PlayerItemScript item in playerItemsList)
        {
            Destroy(item.gameObject);
        }

        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItemScript newPlayerItem = Instantiate(playerItemPrefab, playerItemparent);
            newPlayerItem.SetPlayerInfo(player.Value);
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            playerItemsList.Add(newPlayerItem);
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
