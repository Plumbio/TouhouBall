using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

[System.Serializable]
public class InputHorizontalAxis : UnityEvent<float> { }
[System.Serializable]
public class InputVerticalAxis : UnityEvent<float> { }

public class PjControllerScript : MonoBehaviour {

    public InputHorizontalAxis inputHorizontalAxis;
    public InputVerticalAxis inputVerticalAxis;

    float horInputValue;
    float verInputValue;

    float speed = 5f;
    bool isTouchingBall;
    bool canMove = true;
    bool hittingBall;
    bool connected;

    BallControllerScript ballScript;
    PhotonView ballPunView;
    int ballPunID;
    PhotonView punView;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start() {
        ballScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControllerScript>();
        ballPunView = ballScript.GetComponent<PhotonView>();
        ballPunID = ballPunView.ViewID;
        punView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        connected = PhotonNetwork.IsConnected;
        if (!punView.IsMine) {
            GetComponents<Collider>()[1].enabled = false;
            rb.isKinematic = true;
        }

        inputHorizontalAxis.AddListener(HorizontalMovement);
        inputVerticalAxis.AddListener(VerticalMovement);
    }

    // Update is called once per frame
    void Update() {
        if (connected && punView.IsMine) {
            if (canMove) {
                if(Input.GetAxis("Horizontal") != horInputValue) {
                    horInputValue = Input.GetAxis("Horizontal");
                    inputHorizontalAxis.Invoke(horInputValue);
                }
                if (Input.GetAxis("Vertical") != verInputValue) {
                    verInputValue = Input.GetAxis("Vertical");
                    inputVerticalAxis.Invoke(verInputValue);
                }
                if (horInputValue != 0 || verInputValue != 0)
                    AverageMoevement();
            }

            if (isTouchingBall && Input.GetMouseButtonDown(0))
                HitBall();
        }


        if (transform.position.y < -50)
            transform.position = Vector3.up;
    }

    void HorizontalMovement (float value) {
        if(value == 0 && verInputValue == 0) {
            StopMovement();
            return;
        }
        //gameObject.transform.Translate(Vector3.right * speed * Time.deltaTime);
        //StartCoroutine("AverageMoevement");
    }
    void VerticalMovement (float value) {
        if (value == 0 && horInputValue == 0) {
            StopMovement();
            return;
        }
        //gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        //StartCoroutine("AverageMoevement");
    }
    void AverageMoevement () {
        Vector3 movementVector = new Vector3(horInputValue, 0, verInputValue).normalized;
        gameObject.transform.Translate(movementVector * speed * Time.deltaTime);
    }
    void StopMovement () {
        //evento pa indicar al server que el jugador dejó de moverse
    }

    private void OnTriggerEnter (Collider col) {
        if (col.CompareTag("Ball")) {
            isTouchingBall = true;
        }
    }
    private void OnTriggerExit (Collider col) {
        if (col.CompareTag("Ball")) {
            isTouchingBall = false;
        }
    }

    void HitBall () {
        if (hittingBall || ballScript.beingHit)
            return;
        hittingBall = true;
        Vector3 hitDirection = Vector3.zero;
        Vector3 mouseWorldPos;

        Plane plane = new Plane(Vector3.up, Vector3.up);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance)) {
            mouseWorldPos = ray.GetPoint(distance);
            hitDirection = mouseWorldPos - ballScript.transform.position;
        }

        if(connected)
            ballScript.punView.RPC("Hit", RpcTarget.All, hitDirection, ballScript.transform.position, true, punView.ViewID);
        else ballScript.Hit(hitDirection, ballScript.transform.position, true, punView.ViewID);

        canMove = false;
        Invoke("MoveUnFreeze", ballScript.hitFreezeDuration);
    }
    void MoveUnFreeze () {
        canMove = true;
        hittingBall = false;
    }

    [PunRPC]
    public void BallCrush (Vector3 fullVector) {
        if (!punView.IsMine)
            return;

        rb.velocity = fullVector;
        ballPunView.RPC("Hit", RpcTarget.All, ballScript.ballDirection * -1, ballScript.transform.position, false, punView.ViewID);
        ballPunView.RPC("SetSpeed", RpcTarget.All, ballScript.speedBase);
    }
}
