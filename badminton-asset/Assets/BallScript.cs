using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour {

    public Rigidbody2D rb;
    public Vector3 start_position;
    private bool initiated = false;
	// Use this for initialization
	void Start () {
        if (!initiated)
        {
            rb = GetComponent<Rigidbody2D>();
            start_position = transform.position;
            initiated = true;
        }
        else
        {
            transform.position = start_position;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("called!");
            rb.velocity = new Vector2(0f, 0f);
            Start();
            return;
        }
    }
}
