using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    private Rigidbody rb;

    public float MaxSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = MaxSpeed * rb.velocity.normalized;
        }
    }
}
