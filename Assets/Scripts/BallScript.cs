using System.Collections;
using System.Collections.Generic;
using System.Transactions.Configuration;
using UnityEngine;

public class BallScript : MonoBehaviour
{

    private Rigidbody rb;
    private Vector3 initialPosition;

    public float MaxSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = MaxSpeed * rb.velocity.normalized;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "table") transform.localPosition = initialPosition;
    }
}
