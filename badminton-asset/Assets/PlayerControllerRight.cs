using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;   

public class PlayerControllerRight : NetworkBehaviour
{

    public enum State { standing = 0, running, jumping, swinging };
    public class DashChecker
    {
        private KeyCode keycode;
        private int oneKeyTime;
        private int oneKeyCount;
        private int waitTime;// 키가 연속으로 눌렸다고 인식하는 최대 시간.
        private int timer;
        private bool dash_ready;

        // 생성자 : 체크할 키, 몇초동안 눌린 키를 하나로 볼 지, 대시키 연속으로 누를 때 허용할 최대 시간 간격
        public DashChecker(KeyCode keycode, int oneKeyTime, int waitTime)
        {
            this.keycode = keycode;
            this.oneKeyTime = oneKeyTime;
            oneKeyCount = 0;
            this.waitTime = waitTime;
            timer = 0;
            dash_ready = false;
        }

        public bool check()
        {
            // 키가 한번 인식되었으면 oneKeyTime 동안 무시.
            if (oneKeyCount > 0)
            {
                oneKeyCount = (oneKeyCount + 1) % oneKeyTime;
                return false;
            }

            // 첫번째 대시키가 눌렸으면 매프레임마다 timer 갱신.
            if (dash_ready)
            {
                timer += 1;
                if (timer > waitTime)
                {
                    dash_ready = false;
                    timer = 0;
                }
            }

            if (Input.GetKey(keycode))
            {
                oneKeyCount += 1;
                if (dash_ready)
                {
                    // 이전 대시키에 이어서 연속으로 눌림. -> 대시
                    dash_ready = false;
                    timer = 0;
                    return true;
                }
                else
                {
                    dash_ready = true;
                    return false;
                }
            }
            return false;
        }
    }

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
    private bool round_started;


    // setting
    private float default_velocity = 2f;
    private float dash_speed = 50f;
    private float flash_dist = 1f;
    private float jump_speed = 200f;

    // action checkers
    private DashChecker dc;
    private Checker fc;
    private Checker upper_sc;
    private Checker lower_sc;

    // player info
    private int jumpChance;
    public State state;
    public bool isLeft;

    private KeyCode left;
    private KeyCode right;
    private KeyCode up;
    private KeyCode upSwing;
    private KeyCode downSwing;


    public GameObject rightPlayerPrefab;
    public GameObject startRightPlayer;

    // Use this for initialization
    void Start () {

       if(isServer)
        {
            Debug.Log("player Controll I'm server");
        }
        else
        {
            //var rightPlayer = (GameObject)Instantiate(rightPlayerPrefab, startRightPlayer.transform.position, startRightPlayer.transform.rotation);
            //NetworkServer.Spawn(rightPlayer);

            Debug.Log("player Controll I'm not server");
           // Destroy(this);
            //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        rb = GetComponent<Rigidbody2D>();
        racket = transform.GetChild(0).gameObject;
        gamedata = GameObject.Find("Main").GetComponent<gameDatas>();
        if (isLeft)
        {
            dc = new DashChecker(KeyCode.Z, 8, 16);
            fc = new Checker(KeyCode.F, 8);
            left = KeyCode.A;
            right = KeyCode.D;
            up = KeyCode.W;
            upSwing = KeyCode.S;
            downSwing = KeyCode.LeftShift;
            upper_sc = new Checker(upSwing, 16);
            lower_sc = new Checker(downSwing, 16);
        }
        else
        {
            dc = new DashChecker(KeyCode.Z, 8, 16);
            fc = new Checker(KeyCode.F, 8);
            left = KeyCode.K;
            right = KeyCode.Semicolon;
            up = KeyCode.O;
            upSwing = KeyCode.L;
            downSwing = KeyCode.RightShift;
            upper_sc = new Checker(upSwing, 16);
            lower_sc = new Checker(downSwing, 16);
        }
        
        
        _Ready();
    }

    public void _Ready()
    {
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

        state = State.standing;
        jumpChance = 0;
        //state = State.jumping;
        //UpdateAnimationStates();
        round_started = false;
    }
    public void _Start()
    {
        round_started = true;
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

        if(!isLocalPlayer)
        {
            return;
        }

        bool upper_swing = false;
        bool lower_swing = false;
        if (!round_started)
        {
            lower_swing = lower_sc.check();
            //upper_swing = upper_sc.check();
            //if (upper_swing)
            if (lower_swing)
            {
                GameObject.Find("Main").GetComponent<gameDatas>().ResumeAll();
            }
            return;
        }
        Vector2 speed = new Vector2();
        Vector3 targetPos = new Vector3(0f, 0f, 0f);
        bool dash = false;
        //bool flash = false;
        bool jump = false;
        

        if (Input.GetKey(KeyCode.R))
        {
            rb.velocity = new Vector2(0f, 0f);
            //_Ready(); // gameDatas.readyall 을 불러야 함.
            gamedata.ReadyAll();
            return;
        }

        dash = dc.check();
        //flash = fc.check();
        upper_swing = upper_sc.check();
        lower_swing = lower_sc.check();

        if (Input.GetKey(left))
        {
            /*
            if (flash)
            {
                targetPos.x -= flash_dist;
            }
            */

            if (dash)
            {
                speed.x -= dash_speed;
            }
            else
            {
                speed.x -= default_velocity;
            }
        }

        if (Input.GetKey(right))
        {
            /*
            if (flash)
            {
                targetPos.x += flash_dist;
            }
            */

            if (dash)
            {
                speed.x += dash_speed;
            }
            else
            {
                speed.x += default_velocity;
            }
        }

        if (Input.GetKey(up))
        {
            if (jumpChance > 0)
            {
                jumpChance -= 1;
                speed.y += jump_speed;
                state = State.jumping;
                jump = true;
            }
        }
     
        if (upper_swing || lower_swing)
        {
            
            if (upper_swing)
            {
                //CmdSwingUpper();
                Debug.Log("playercontroller upper swing");
                racket.GetComponent<RacketScript>().SwingUpper();
            }                
            else
            {
                //CmdSwingLower();
                Debug.Log("playercontroller down swing");
                racket.GetComponent<RacketScript>().SwingLower();
            }
                
        }
                    
        if (state == State.standing)
            rb.velocity = speed;
        else
            rb.velocity = Vector3.Lerp(rb.velocity, speed, 2f * Time.deltaTime);

        if (state == State.standing && !(rb.velocity == new Vector2(0.0f, 0.0f)))
        {
            //state = State.running;
        }
        //UpdateAnimationStates();
    }

    void UpdateAnimationStates()
    {
        Debug.Log(string.Format("state : {0}", (int) state));
        //GetComponent<Animator>().SetInteger("state", (int) state);
    }

    [Command]
    void CmdSwingUpper()
    {
        Debug.Log("Swing upper");
        racket.GetComponent<RacketScript>().SwingUpper();
    }

    [Command]
    void CmdSwingLower()
    {
        Debug.Log("Swing lower");
        racket.GetComponent<RacketScript>().SwingLower();
    }


}

