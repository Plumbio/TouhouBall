using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public InputField usernameInput;
    public Text buttonText;

    private void Start() {
        FindObjectOfType<AudioManagerScript>().Stop("MainTheme");
    }
    public void OnClickConnect() 
    {
        if (usernameInput.text.Length >= 1)
        {
            print("se conecto al lobby");
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
