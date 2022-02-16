using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    // Start is called before the first frame update
    void Start() {
        inputHorizontalAxis.AddListener(HorizontalMovement);
        inputVerticalAxis.AddListener(VerticalMovement);
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetAxis("Horizontal") != horInputValue) {
            horInputValue = Input.GetAxis("Horizontal");
            inputHorizontalAxis.Invoke(horInputValue);
        }
        if (Input.GetAxis("Vertical") != verInputValue) {
            verInputValue = Input.GetAxis("Vertical");
            inputVerticalAxis.Invoke(verInputValue);
        }
        if(horInputValue != 0 || verInputValue != 0)
            StartCoroutine("AverageMoevement");
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
    IEnumerator AverageMoevement () {
        yield return new WaitForFixedUpdate();
        Vector3 movementVector = new Vector3(horInputValue, 0, verInputValue).normalized;
        gameObject.transform.Translate(movementVector * speed * Time.deltaTime);
    }
    void StopMovement () {
        //evento pa indicar al server que el jugador dejó de moverse
    }
}
