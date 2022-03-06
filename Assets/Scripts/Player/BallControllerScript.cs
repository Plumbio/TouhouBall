using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallControllerScript : MonoBehaviour {

    [HideInInspector]
    public float speedBase = 5f;
    [HideInInspector]
    public float speedCurrent = 5f;
    float speedAcceleration = 2;
    [HideInInspector]
    public Vector3 ballDirection;
    bool moving;
    bool connected;
    [HideInInspector]
    public float hitFreezeDuration;

    float xLimits = 6.75f;
    float zLimits = 6.75f;

    [HideInInspector]
    public PhotonView punView;
    Collider ballCol;

    void Start() {
        punView = GetComponent<PhotonView>();
        ballCol = GetComponent<Collider>();
        connected = PhotonNetwork.IsConnected;
        /*test code
        ballDirection = new Vector3(1, 0, 0.6f).normalized;
        moving = true;
        test code*/
    }

    void Update() {
        if (moving)
            Displace();

        if (transform.position.x > xLimits)
            transform.position = new Vector3(xLimits, 1, transform.position.z);
        else if (transform.position.x < -xLimits)
            transform.position = new Vector3(-xLimits, 1, transform.position.z);

        if (transform.position.z > zLimits)
            transform.position = new Vector3(transform.position.x, 1, zLimits);
        else if (transform.position.z < -zLimits)
            transform.position = new Vector3(transform.position.x, 1, -zLimits);

        if (transform.position.x >= xLimits || transform.position.x <= -xLimits) {
            Hit((new Vector3(ballDirection.x * -1, 0, ballDirection.z)), transform.position, false, 0);
        }
        else if (transform.position.z >= zLimits || transform.position.z <= -zLimits) {
            Hit((new Vector3(ballDirection.x, 0, ballDirection.z * -1)), transform.position, false, 0);
        }
    }

    [PunRPC]
    public void Hit (Vector3 direction, Vector3 position, bool addSpeed, int punID) {
        if (addSpeed) {
            speedCurrent += speedAcceleration;
            if (!PhotonView.Find(punID).IsMine) {
                float lagCompensation = PhotonNetwork.GetPing() * 0.002f;
                hitFreezeDuration = (speedCurrent * 0.1f) - lagCompensation;
            }
            else hitFreezeDuration = speedCurrent * 0.1f;
        }

        transform.position = new Vector3(position.x, 1, position.z);
        //ballCol.enabled = false;
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        ballDirection = newDirection.normalized;
        moving = false;
        if (!addSpeed)
            HitUnFreeze();
        else Invoke("HitUnFreeze", hitFreezeDuration);
    }
    void HitUnFreeze () {
        moving = true;
        //ballCol.enabled = false;
    }

    void Displace () {
        transform.Translate(ballDirection * speedCurrent * Time.deltaTime);
    }

    private void OnTriggerEnter (Collider col) {
        if (!moving)
            return;
        if (col.CompareTag("Player")) {
            if(col == col.GetComponents<Collider>()[1]) {
                if (connected) {
                    PhotonView colledPun = col.GetComponent<PhotonView>();
                    if (colledPun.IsMine) {
                        Photon.Realtime.Player colledPlayer = colledPun.Controller;
                        colledPun.RPC("BallCrush", colledPlayer, new Vector3(ballDirection.x, 0.5f, ballDirection.z) * (speedCurrent / 1.5f));
                    }
                }
                else {
                    col.attachedRigidbody.velocity = new Vector3(ballDirection.x, 0.5f, ballDirection.z) * (speedCurrent / 1.5f);
                    Hit(ballDirection * -1, transform.position, false, 0);
                    SetSpeed(speedBase);
                }
            }
        }
    }

    [PunRPC]
    public void SetSpeed (float speed) {
        speedCurrent = speed;
    }
}
