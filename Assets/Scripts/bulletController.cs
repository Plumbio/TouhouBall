using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletController : MonoBehaviour
{
    Vector3 direction;
    float speed;

    void Start()
    {
        SetDirection(Vector3.forward);
        SetSpeed(1f);
    }
    void Update()
    {
        Move();
    }
    
    public Vector3 GetDirection() 
    {
        return direction;
    }
    public void SetDirection(Vector3 _direction)
    {
        direction = _direction;
    }
    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    public float GetSpeed()
    {
        return speed;
    }

    public void Move()
    {
        transform.Translate(direction * Time.deltaTime * speed);
    }
    public void DestroyBullet()
    {
        gameObject.SetActive(false);
    }
}
