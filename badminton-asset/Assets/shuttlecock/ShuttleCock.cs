using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.SceneManagement;

public class ShuttleCock : NetworkBehaviour {

    private Rigidbody2D rb2D;
    private Vector3 original_position;
    private Quaternion original_rotation;
    private float original_gravityScale;

    //private ParticleSystem psChild;
    private Transform psChild;
    //private int rotationOffset = 45;
    private bool gameOver = false;

    public gameDatas gamedata;

    // ball physics setting
    private float upper_z_margin = 2.2f;
    private float lower_z_margin = 1.5f;

    // Use this for initialization
    void Start () {
        Debug.Log("ball start");
        rb2D = GetComponent<Rigidbody2D>();
        original_gravityScale = rb2D.gravityScale;
        psChild = transform.GetChild(0);

        original_position = transform.position;
        original_rotation = transform.rotation;
        gamedata = GameObject.Find("Main").GetComponent<gameDatas>();
        _Ready();
	}

    public void _Ready()
    {
        transform.position = original_position;
        transform.rotation = original_rotation;
        rb2D.velocity = new Vector2(0f, 0f);
        rb2D.gravityScale = 0.0f;
        Debug.Log("ball ready");
    }

    public void _Start()
    {
        Debug.Log("ball _start");
        rb2D.gravityScale = original_gravityScale;
    }

    float torque = 1.0f;
    private float thrust = 200.0f;
    float rotationAngle;
    float rotationOffset = 0f;

    // Update is called once per frame
    void Update () {
        if (!isServer)
            return;
        if(rb2D.velocity.x != 0 && rb2D.velocity.y != 0)
        {
            //Debug.Log("Velocity vector" + rb2D.velocity.x + " / " + rb2D.velocity.y);
            rotationAngle = Mathf.Rad2Deg * Mathf.Atan( (float) rb2D.velocity.y / (float) rb2D.velocity.x);
            //Debug.Log("RotationAngle : " + rotationAngle);
        }

        //transform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle + rotationOffset);
        //psChild.transform.rotation = Quaternion.Euler(0, 0, rotationAngle + rotationOffset -45);
               
    }

    [Command]
    public void CmdHitBall(float degWithZ, float power, int power_direction, bool isUpper)
    {
        HitBall(degWithZ, power, power_direction, isUpper);
    }

    public void HitBall(float degWithZ, float power, int power_direction, bool isUpper)
    {
        Debug.Log("CmdHitBall called");
        rb2D.velocity = new Vector2(rb2D.velocity.x / 2.0f, 0);
        if (isUpper)
        {
            Vector2 v = new Vector2(Mathf.Rad2Deg * Mathf.Cos(degWithZ), Mathf.Rad2Deg * Mathf.Sin(degWithZ) * upper_z_margin) * power * power_direction;
            rb2D.AddForce(v);
            rb2D.rotation = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
        else
        {
            Vector2 v = new Vector2(Mathf.Rad2Deg * Mathf.Cos(degWithZ), Mathf.Rad2Deg * Mathf.Sin(degWithZ) * lower_z_margin) * power * power_direction;
            rb2D.AddForce(v);
            rb2D.rotation = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.CompareTag("Ground") && isServer)
        {
            StartCoroutine(RoundEnded());
        }
    }


    IEnumerator RoundEnded()
    {
        gamedata.GetComponent<gameDatas>().round_started = false;
        //gamedata.scoreLeft++;
        yield return new WaitForSeconds(1);
        gamedata.GetComponent<gameDatas>().round_restart = true;
    }

}
