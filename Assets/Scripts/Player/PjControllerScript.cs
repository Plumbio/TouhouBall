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

    BallControllerScript ballScript;
    PhotonView punView;

    // Start is called before the first frame update
    void Start() {
        ballScript = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallControllerScript>();
        punView = GetComponent<PhotonView>();

        inputHorizontalAxis.AddListener(HorizontalMovement);
        inputVerticalAxis.AddListener(VerticalMovement);
    }

    // Update is called once per frame
    void Update() {
        if (!punView.IsMine)
            return;

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

        if (isTouchingBall && Input.GetMouseButtonDown(0))
            HitBall();
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
        Vector3 hitDirection = Vector3.zero;
        Vector3 mouseWorldPos;

        Plane plane = new Plane(Vector3.up, Vector3.up);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance)) {
            mouseWorldPos = ray.GetPoint(distance);
            hitDirection = mouseWorldPos - transform.position;
        }

        ballScript.Hit(hitDirection);
    }
}
