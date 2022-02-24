using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallControllerScript : MonoBehaviour {

    float speed = 5f;
    Vector3 ballDirection;
    bool moving;

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
    }

    [PunRPC]
    public void Hit (Vector3 direction, Vector3 position) {
        transform.position = new Vector3(position.x, 1, position.z);
        Vector3 newDirection = new Vector3(direction.x, 0, direction.z);
        ballDirection = newDirection.normalized;
        moving = true;
    }

    void Displace () {
        transform.Translate(ballDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter (Collider col) {
        if (col.CompareTag("Wall")) {
            Vector3 ballPos = transform.position;
            Vector3 colPoint = col.ClosestPoint(ballPos);
            Vector3 newDirection = Vector3.zero;

            if (Mathf.Abs(ballPos.x - colPoint.x) > Mathf.Abs(ballPos.z - colPoint.z))
                newDirection = new Vector3(ballDirection.x * -1, 0, ballDirection.z);
            else newDirection = new Vector3(ballDirection.x, 0, ballDirection.z * -1);

            Hit(newDirection, transform.position);
        }
    }
}
