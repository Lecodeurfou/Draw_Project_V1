using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class personamove : MonoBehaviour {
  
    public float speed;

    private Rigidbody rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        speed = 5;
    }

    void FixedUpdate ()
    {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");

        Vector3 movement = new Vector3 (moveHorizontal*speed, 0.0f, moveVertical+speed);

    }
}