using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallControllerScript : MonoBehaviour {

    [HideInInspector]
    public readonly float speedBase = 5f;
    [HideInInspector]
    public float speedCurrent = 5f;
    readonly float speedAcceleration = 3;
    [HideInInspector]
    public Vector3 ballDirection;
    bool moving;
    bool connected;
    [HideInInspector]
    public bool beingHit;
    [HideInInspector]
    public float hitFreezeDuration;

    //Compensación de Lag
    List<int> pingList = new List<int>();
    float pingTimer = 0;
    float pingTime = 3;

    float xLimits = 6.75f;
    float zLimits = 6.75f;

    [HideInInspector]
    public PhotonView punView;
    Collider ballCol;
    Collider pjHitCol;

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

        if (pingTimer > 0)
            pingTimer -= Time.deltaTime;
        else {
            pingTimer = pingTime;
            pingList.Add(PhotonNetwork.GetPing());
        }
    }

    [PunRPC]
    public void Hit (Vector3 direction, Vector3 position, bool addSpeed, int punID) {
        pjHitCol = null;

        if (addSpeed) {
            beingHit = true;
            speedCurrent += speedAcceleration;
            PhotonView pjPunView = PhotonView.Find(punID);
            pjHitCol = pjPunView.gameObject.GetComponents<Collider>()[0];
            if (!pjPunView.IsMine) {
                float lagCompensation = GetPingAvg() + 0.1f;
                if((speedCurrent * 0.05f) - lagCompensation >= 0.1f) {
                    hitFreezeDuration = (speedCurrent * 0.05f) - lagCompensation;
                    //print(lagCompensation + " ms - " + (speedCurrent * 0.05f) + " hitfreeze");
                }
                else hitFreezeDuration = speedCurrent * 0.05f;
            }
            else hitFreezeDuration = speedCurrent * 0.05f;
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
        if (col.CompareTag("Player") && !beingHit) {
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
    private void OnTriggerExit (Collider col) {
        if (beingHit && col == pjHitCol) {
            pjHitCol = null;
            beingHit = false;
        }
        else if (beingHit && pjHitCol == null)
            beingHit = false;
    }

    [PunRPC]
    public void SetSpeed (float speed) {
        speedCurrent = speed;
    }

    float GetPingAvg () {
        int pingAvg = 0;
        foreach (int ping in pingList)
            pingAvg += ping;
        pingAvg /= pingList.Count;

        return pingAvg * 0.001f;
    }
}
