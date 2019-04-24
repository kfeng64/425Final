using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    float moveV, moveH;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveV = Input.GetAxis("Vertical");
        moveH = Input.GetAxis("Horizontal");

        Vector3 newPosition = new Vector3(moveH, 0.0f, moveV);
        transform.LookAt(newPosition + transform.position);
        transform.Translate(newPosition * speed * Time.deltaTime, Space.World);
    }
}
