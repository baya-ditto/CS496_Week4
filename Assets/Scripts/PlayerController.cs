using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int speed;
    public Animator animator;

    public GameObject goalposition;
    public GameObject goalcamera;
    public GameObject cameratransform;

    private Vector3 playerPosition;
    private Quaternion playerRotation;

    private bool isGoal;
    public bool isGameStart;
    public int dynamicRun;
    private Rigidbody rb;

    public GameObject timeManager;
    public TimeCountDown timeCountDown;

    private bool cooltimeRemain;

    // Use this for initialization
    void Start () {

        animator = GetComponent<Animator>();

        cooltimeRemain = false;

        timeCountDown = timeManager.GetComponent<TimeCountDown>();

        dynamicRun = 0;
        isGoal = false;
        isGameStart = false;
        rb = GetComponent<Rigidbody>();
        speed = 1;
        //rb.velocity = Vector3.forward * speed;
        playerPosition = gameObject.transform.position;
        playerRotation = gameObject.transform.rotation;

    }
	
	// Update is called once per frame
	void Update () {

        if(!isGameStart)
        {
            return;
        }

        if(isGoal)
        {
            return;
        }

        //transform.Translate(Vector3.forward * speed * Time.deltaTime);        
     
        if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("get key W");
            transform.Translate(Vector3.forward * speed *40* Time.deltaTime);

            //rigidbody.velocity = 우리가 받는 int값;
            //TODO : velocity가 pudate마다 줄어들게 만들자

        }

        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("RRRR");
            transform.position = playerPosition;
            transform.rotation = playerRotation;
            
        }

    }

    private void FixedUpdate()
    {
        if(!isGameStart)
        {
            return;
        }

        if (isGoal)
        {
            return;
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision tag is "+collision.gameObject.tag);

        if(collision.gameObject.CompareTag("Goal"))
        {

            isGoal = true;
            timeCountDown.finished = true;
            //animator.SetBool("isSpeed12", false);
            //animator.SetBool("isSpeed34", false);

            animator.SetBool("isDone", true);
            //animator.SetBool("isRun", false);


            Invoke("goToCeremony", 1f);

            animator.SetBool("isCeremony", true);
            //Invoke("doDance", 2f);

            //Invoke("resetTimeScale", 1f);
            // TODO : do somthing
        }
       
    }

    void resetTimeScale()
    {
        Debug.Log("reset timescale");
        Time.timeScale = 1f;
    }

    void goToCeremony()
    {
        transform.position = goalposition.transform.position;
        transform.rotation = goalposition.transform.rotation;

        cameratransform.transform.position = goalcamera.transform.position;
        cameratransform.transform.rotation = goalcamera.transform.rotation;
    }

    private void doDance()
    {
        animator.SetBool("isCeremony", true);
    }

    public void SetVelocity(int gap)
    {
        if(cooltimeRemain)
        {
            return;
        }

        if(false)
        {
            animator.SetBool("isSpeed12", false);
            animator.SetBool("isSpeed34", false);
        }
        else if(gap == 0 || gap == 1)
        {
            animator.SetBool("isSpeed12", true);
            animator.SetBool("isSpeed34", false);

        }
        else if(gap == 3 || gap == 4 || gap == 2 )
        {
            animator.SetBool("isSpeed34", true);
            animator.SetBool("isSpeed12", false);

            cooltimeRemain = true;
            Invoke("resetCooltime", 2f);
        }

        Debug.Log("setVelocity : " + gap);

        speed = gap * 5+ 5;

        //transform.Translate(Vector3.forward * gap*2 * Time.deltaTime);
        //rb.velocity = Vector3.forward * gap*2;
        //speed = gap;
    }

    private void resetCooltime()
    {
        cooltimeRemain = false;
    }


}
