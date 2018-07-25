using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerController : NetworkBehaviour
{

    public enum State { standing = 0, running, jumping, swinging };

    public class Checker
    {
        private KeyCode keycode;
        private int oneKeyTime;
        private int oneKeyCount;

        // 생성자 : 체크할 키, 몇초동안 눌린 키를 하나로 볼 지
        public Checker(KeyCode keycode, int oneKeyTime)
        {
            this.keycode = keycode;
            this.oneKeyTime = oneKeyTime;
            oneKeyCount = 0;
        }

        public bool check()
        {
            if (oneKeyCount > 0)
            {
                // 한번 입력되었으면 oneKeyTime 동안 무시
                oneKeyCount = (oneKeyCount + 1) % oneKeyTime;
                return false;
            }

            // 아직 입력 안된 상태
            if (Input.GetKey(keycode))
            {
                oneKeyCount += 1;
                return true;
            }
            return false;
        }
    }

    
    public Rigidbody2D rb;
    public GameObject racket;
    public gameDatas gamedata;
    
    // initial info
    private Vector3 start_position;
    private Vector3 start_velocity;
    private bool initiated = false;
    //private bool round_started;


    // setting
    private float default_velocity = 2f;
    private float dash_speed = 50f;
    private float flash_dist = 1f;
    private float jump_speed = 200f;

    // action checkers
    private Checker fc;
    private Checker upper_sc;
    private Checker lower_sc;
    private Checker jc;

    // player info
    public int jumpChance;
    public State state;
    public bool isLeft;
    public bool isSingleComputer;

    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode upSwing;
    public KeyCode downSwing;


    public GameObject rightPlayerPrefab;
    public GameObject startRightPlayer;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        racket = transform.GetChild(0).gameObject;
        gamedata = GameObject.Find("Main").GetComponent<gameDatas>();
        if (isSingleComputer)
        {
            if (isLeft)
            {
                fc = new Checker(KeyCode.F, 8);
                left = KeyCode.A;
                right = KeyCode.D;
                up = KeyCode.W;
                jc = new Checker(up, 8);
                upSwing = KeyCode.S;
                downSwing = KeyCode.LeftShift;
                upper_sc = new Checker(upSwing, 16);
                lower_sc = new Checker(downSwing, 16);
            }
            else
            {
                fc = new Checker(KeyCode.F, 8);
                left = KeyCode.K;
                right = KeyCode.Semicolon;
                up = KeyCode.O;
                jc = new Checker(up, 8);
                upSwing = KeyCode.L;
                downSwing = KeyCode.RightShift;
                upper_sc = new Checker(upSwing, 16);
                lower_sc = new Checker(downSwing, 16);
            }
        }
        else
        {
            if (isLocalPlayer)
            {
                fc = new Checker(KeyCode.F, 8);
                left = KeyCode.A;
                right = KeyCode.D;
                up = KeyCode.W;
                jc = new Checker(up, 8);
                upSwing = KeyCode.S;
                downSwing = KeyCode.LeftShift;
                upper_sc = new Checker(upSwing, 16);
                lower_sc = new Checker(downSwing, 16);
            }
        }
        
        _Ready();
    }

    public void _Ready()
    {
        rb.velocity = new Vector2(0f, 0f);
        if (!initiated)
        {
            start_position = transform.position;
            initiated = true;
        }
        else
        { 
            transform.position = start_position;
        }

        rb.freezeRotation = true;

        //state = State.standing;
        state = State.jumping;
        jumpChance = 0;
    }
    public void _Start()
    {
        //round_started = true;
        //racket.GetComponent<RacketScript>().RotateClockwise();
    }

    bool isGrounded()
    {
        return Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, 0.1f);
    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("collided");
        /*ContactPoint2D contact = coll.contacts[0];
        Vector2 normal = contact.normal;
        if (normal.x == 0.0f && normal.y == 1.0f)*/
        if (coll.gameObject.CompareTag("Ground") && isGrounded())
        {
            // 바닥과 collision
            if (jumpChance == 0)
                jumpChance += 1;
            Debug.Log(string.Format("Jump chance ++ ! : {0}", jumpChance));
            if (state == State.jumping) state = State.standing;
        }
    }



    // Update is called once per frame
    void Update () {
        bool upper_swing = false;
        bool lower_swing = false;
        bool jump = false;
        if (!gamedata.round_started || !isLocalPlayer)
        {
            return;
        }
        Vector2 speed = new Vector2();
        Vector3 targetPos = new Vector3(0f, 0f, 0f);

        upper_swing = upper_sc.check();
        lower_swing = lower_sc.check();
        jump = jc.check();

        if (Input.GetKey(left))
        {
            if (isLocalPlayer)
            {
                speed.x -= default_velocity;
            }
        }

        if (Input.GetKey(right))
        {
            if (isLocalPlayer)
            {
                speed.x += default_velocity;
            }
        }

        //if (Input.GetKey(up))
        if(jump)
        {
            if (isLocalPlayer)
            {
                if (jumpChance > 0 && state != State.jumping)
                {
                    jumpChance -= 1;
                    speed.y += jump_speed;
                    state = State.jumping;
                }
            }
        }
     
        if (upper_swing || lower_swing)
        {
            if (upper_swing)
            {
                //if (isServer) // && isLeft
                if (isLocalPlayer)
                {
                    Debug.Log("playercontroller upper swing");
                    racket.GetComponent<RacketScript>().SwingUpper();
                }
            }                
            else
            {
                if (isLocalPlayer)
                {
                    Debug.Log("playercontroller lower swing");
                    racket.GetComponent<RacketScript>().SwingLower();
                }
            }
                
        } 
        if (state == State.standing)
            rb.velocity = speed;
        else
            rb.velocity = Vector3.Lerp(rb.velocity, speed, 2f * Time.deltaTime);
    }

    [Command]
    public void CmdHitBallTrans(float degWithZ, float power, int power_direction, bool isUpper)
    {
        GameObject.FindGameObjectWithTag("Ball").GetComponent<ShuttleCock>().HitBall(degWithZ, power, power_direction, isUpper);
    }

    public void sendHitInfo(Collider2D coll, float degWithZ, float power, int power_direction, bool isUpper)
    {
         
        if (isServer)
        {
            Debug.Log("sendHitInfo on server");
            coll.gameObject.GetComponent<ShuttleCock>().HitBall(degWithZ, power, power_direction, isUpper);
        }
        else
        {
            CmdHitBallTrans(degWithZ, power, power_direction, isUpper);
            //coll.gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
            //coll.gameObject.GetComponent<ShuttleCock>().CmdHitBall(degWithZ, power, power_direction, isUpper);
            //coll.gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient); 
            /*
            Debug.Log("sendHitInfo on client");
            NetworkMessageHandler.HitMessage _msg = new NetworkMessageHandler.HitMessage()
            {
                degWithZ = degWithZ,
                power = power,
                power_direction = power_direction,
                isUpper = isUpper
            };
            NetworkServer.SendToAll(NetworkMessageHandler.hit_msg, _msg);
            */
        }

    }
}

