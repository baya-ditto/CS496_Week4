using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeCountDown : MonoBehaviour {

    public Text TimeCount;    
    public Text TimerCount;
    public float TimeCost;
    public float Timer;    
    private bool isGameStart;
    public GameObject player;
    public PlayerController playerControllerScript;

    private string record;

    public Animator animator;

    public bool finished;

    // Use this for initialization
    void Start () {

        Timer = 0;
        isGameStart = false;
        playerControllerScript = player.GetComponent<PlayerController>();

        animator = player.GetComponent<Animator>();

        finished = false;
        
    }
	
	// Update is called once per frame
	void Update () {

        if(!isGameStart)
        {
            if (TimeCost <= 1.1)
            {
                TimeCount.text = "GO!";
                //TimeCount.text = "";
                isGameStart = true;
                playerControllerScript.isGameStart = true;
                animator.SetBool("isRun", true);

                Invoke("SetDeactiveText", 1);
                
                return;
            }
            TimeCost -= Time.deltaTime;
            TimeCount.text = "LEFT : " + Mathf.Floor(TimeCost);
            return;
        }
        else
        {            
            if(!finished)
            {
                Timer += Time.deltaTime;
                record = GetN2(Timer);
                TimerCount.text = "Time : " + record;                
            }
            else
            {
                TimerCount.text = record;
            }
            

        }

                       		
	}

    private void SetDeactiveText()
    {
        TimeCount.text = "";
        
    }

    private string GetN2(float A)
    {
        string result = string.Empty;

        if (A == (int)A)
            result = A.ToString();
        else
            result = A.ToString("N2");

        return result;
    }

}
