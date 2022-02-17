using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControllerScript : MonoBehaviour {

    float speed = 5f;
    Vector3 ballDirection;
    bool moving;

    void Start() {
        //test code
        ballDirection = new Vector3(1, 0, 0.6f).normalized;
        moving = true;
        //test code
    }

    void Update() {
        if (moving)
            StartCoroutine("Displace");
    }

    public void Hit (Vector3 direction) {
        ballDirection = direction;
    }

    IEnumerator Displace () {
        yield return new WaitForFixedUpdate();
        transform.Translate(ballDirection * speed * Time.deltaTime);
    }
}
