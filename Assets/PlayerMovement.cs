using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
       
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody2D.velocity = new Vector3(1, 0, 0) * speed;
            //gameObject.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidbody2D.velocity = new Vector3(-1, 0, 0) * speed;
           // gameObject.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidbody2D.velocity = new Vector3(0, 1, 0) * speed;
         //   gameObject.transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rigidbody2D.velocity = new Vector3(0, -1, 0) * speed;
           // gameObject.transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * speed);
        }
    }
}
