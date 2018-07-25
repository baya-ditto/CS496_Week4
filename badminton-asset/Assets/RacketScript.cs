using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RacketScript : NetworkBehaviour {

    public Rigidbody2D rb;

    // Rotation setting
    public bool isRotating;
    public bool isUpper;
    public float lowerAngle;
    public int lowerDirection;
    public float upperAngle;
    public int upperDirection;

    // Swing setting
    private int swing_frame = 12;
    private int count = 0;
    private float power = 10.0f;
    private int power_direction;
    private float upper_z_margin = 2.2f;
    private float lower_z_margin = 1.5f;

    public bool myturn;

    [SyncVar]
    public bool isFirstPlayer;

    private Quaternion initial_rotation;
    //private Vector3 offset;
    //public Transform player_transform;

	// Use this for initialization
	void Start () {
        //isFirstPlayer = myturn;       

        if(isFirstPlayer)
        {
            transform.rotation = Quaternion.Euler(0, 0, 58);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -58);
        }

        

        initial_rotation = transform.rotation;
        
        isRotating = false;
        myturn = isFirstPlayer;
        if (transform.GetComponentInParent<PlayerController>().isLeft)
            power_direction = 1;
        else
            power_direction = -1;
        _Ready();
	}

    public void _Ready()
    {
        isRotating = false;
        transform.rotation = initial_rotation;
        myturn = isFirstPlayer;
    }

    public void _Start()
    {

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Ball") && myturn && isRotating)
        {
            if (isLocalPlayer)
                Debug.Log("Racket, isLocalPlayer");
            else
                Debug.Log("Racket, is not Local Player");
            myturn = false;
            float degWithZ = transform.rotation.z;
            if (isUpper)
            {
                Debug.Log("trigger with ball and racket");
                transform.GetComponentInParent<PlayerController>().sendHitInfo(coll, degWithZ, power, power_direction, true);
            }
            else
            {
                Debug.Log("trigger with ball and racket");
                transform.GetComponentInParent<PlayerController>().sendHitInfo(coll, degWithZ, power, power_direction, false);
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //transform.position = player_transform.position + offset;
        //Debug.Log(string.Format("Update called : {0} {1} {2}", transform.position.x, transform.position.y, transform.position.z));
        /*
        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("called!");
            _Ready();
            return;
        }
        */

        if (isRotating)
        {
            if (isUpper)
            {
                if (count < swing_frame)
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, upperDirection * (upperAngle / swing_frame)));
                    count += 1;
                }
                else
                {
                    isRotating = false;
                    transform.rotation = initial_rotation;
                    count = 0;
                }
            }
            else
            {
                if (count < swing_frame)
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, lowerDirection * (lowerAngle / swing_frame)));
                    count += 1;
                }
                else
                {
                    isRotating = false;
                    transform.rotation = initial_rotation;
                    count = 0;
                }
            }
        }
    }

    public void SwingLower()
    {
        transform.rotation = initial_rotation;
        count = 0;
        isUpper = false;
        isRotating = true;
    }
    public void SwingUpper()
    {
        transform.rotation = initial_rotation;
        count = 0;
        isUpper = true;
        isRotating = true;     
    }
}
