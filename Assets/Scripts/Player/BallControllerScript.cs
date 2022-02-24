﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallControllerScript : MonoBehaviour {

    [HideInInspector]
    public float speedBase = 5f;
    [HideInInspector]
    public float speedCurrent = 5f;
    float speedAcceleration = 1;
    [HideInInspector]
    public Vector3 ballDirection;
    bool moving;

    float xLimits = 6.75f;
    float zLimits = 6.75f;

    [HideInInspector]
    public PhotonView punView;

    void Start() {
        punView = GetComponent<PhotonView>();
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
            Hit((new Vector3(ballDirection.x * -1, 0, ballDirection.z)), transform.position, false);
        }
        else if (transform.position.z >= zLimits || transform.position.z <= -zLimits) {
            Hit((new Vector3(ballDirection.x, 0, ballDirection.z * -1)), transform.position, false);
        }
    }

    [PunRPC]
    public void Hit (Vector3 direction, Vector3 position, bool addSpeed) {
        if(addSpeed)
            speedCurrent += speedAcceleration;
        transform.position = new Vector3(position.x, 1, position.z);
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        ballDirection = newDirection.normalized;
        moving = true;
    }

    void Displace () {
        transform.Translate(ballDirection * speedCurrent * Time.deltaTime);
    }

    private void OnTriggerEnter (Collider col) {
        /*if (col.CompareTag("Wall")) {
            Vector3 ballPos = transform.position;
            Vector3 colPoint = col.ClosestPoint(ballPos);
            Vector3 newDirection = Vector3.zero;

            if (Mathf.Abs(ballPos.x - colPoint.x) > Mathf.Abs(ballPos.z - colPoint.z))
                newDirection = new Vector3(ballDirection.x * -1, 0, ballDirection.z);
            else newDirection = new Vector3(ballDirection.x, 0, ballDirection.z * -1);

            Hit(newDirection, transform.position);
        }*/
        if (col.CompareTag("Player")) {
            if(col == col.GetComponents<Collider>()[1]) {
                if (PhotonNetwork.IsConnected) {
                    if (col.GetComponent<PhotonView>().IsMine)
                        col.GetComponent<PhotonView>().RPC("BallCrush", RpcTarget.All, new Vector3(ballDirection.x, 0.5f, ballDirection.z) * (speedCurrent / 1.5f));
                }
                else {
                    col.attachedRigidbody.velocity = new Vector3(ballDirection.x, 0.5f, ballDirection.z) * (speedCurrent / 1.5f);
                    Hit(ballDirection * -1, transform.position, false);
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
