using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetPlayerSpawnerScript : MonoBehaviour
{

    public GameObject[] playerPrefabs;
    

    void Start() {
        GameObject playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        PhotonNetwork.Instantiate(playerToSpawn.name, transform.position + Vector3.up, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
