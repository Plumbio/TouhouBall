using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetPlayerSpawnerScript : MonoBehaviour
{

    public GameObject[] playerPrefab;

    void Start() {
        if (PhotonNetwork.IsConnected) {
            GameObject playerToSpawn = playerPrefab[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
            PhotonNetwork.Instantiate(playerToSpawn.name, transform.position + Vector3.up, Quaternion.identity);
        }
        else Instantiate(playerPrefab[0], transform.position + Vector3.up, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
