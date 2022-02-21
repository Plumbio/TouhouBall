using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemScript : MonoBehaviour
{
    public Text roomName;
    public LobbyManagerScript lobbyManager;
    private void Start() {
        lobbyManager = FindObjectOfType<LobbyManagerScript>();
    }

    public void SetRoomName(string _roomName) 
    {
        roomName.text = _roomName;
    }
    public void OnClickItem()
    {
        lobbyManager.JoinRoom(roomName.text);
    }
}
