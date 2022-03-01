using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayScript : MonoBehaviour
{
    private void Start() {
        FindObjectOfType<AudioManagerScript>().Play("MainTheme");
    }
    public void OnClickPlay() {
        SceneManager.LoadScene("ConnectToServer");
    }
}
