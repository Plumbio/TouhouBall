using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetPlayerSpawnerScript : MonoBehaviour
{

    public GameObject playerPrefab;

    void Start() {
        if(PhotonNetwork.IsConnected)
            PhotonNetwork.Instantiate(playerPrefab.name, transform.position + Vector3.up, Quaternion.identity);
        else GameObject.Instantiate(playerPrefab, transform.position + Vector3.up, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
