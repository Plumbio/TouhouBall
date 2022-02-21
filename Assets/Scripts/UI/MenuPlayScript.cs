using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayScript : MonoBehaviour
{
    public void OnClickPlay() {
        SceneManager.LoadScene("ConnectToServer");
    }
}
