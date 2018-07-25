using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class gameDatas : NetworkBehaviour {

    [SyncVar]
    public int scoreLeft = 0;

    [SyncVar]
    public int scoreRight = 0;

    public int flag = 0;
    public GameObject ball;
    public GameObject start_ball;
    private Vector3 start_ball_position;
    private Vector3 startLeftPosition;
    private Vector3 startRightPosition;

    // Start Player Position Object
    public GameObject startLeftPlayer;
    public GameObject startRightPlayer;

    // Start Player Prefab
    public GameObject leftplayer_prefab;
    public GameObject rightplayer_prefab;
    public GameObject leftracket_prefab;
    public GameObject rightracket_prefab;

    public GameObject startRightRacket;
    public GameObject startLeftRacket;

    private GameObject ball_clone;

    //public GameObject centerLine;
    
    private GameObject player_left; // runtime
    private GameObject player_right; // runtime
    private GameObject leftRacket; // runtime
    private GameObject rightRacket; // runtime

    [SyncVar]
    public bool round_started = false;
    [SyncVar]
    public bool round_restart = false;


    public bool isPrevLeft;

    [SyncVar]
    public int playerCount = 0;

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        Debug.Log("gameDatas Start()");
        start_ball_position = start_ball.transform.position;
        startLeftPosition = startLeftPlayer.transform.position;
        startRightPosition = startRightPlayer.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (scoreLeft != 0)
        {
            //Debug.Log("scoreLeft is " + scoreLeft);
        }
        if (Input.GetKey(KeyCode.Space) && !round_started)
        {
            if (isServer)
            {
                RpcResumeAll();
                ResumeAll();
            }
            /*
            else
            {
                ResumeAll();
                CmdResumeAll();
            }
            */
        }
        if ((isServer && Input.GetKey(KeyCode.R)) || round_restart)
        {
            if (isServer)
            {
                if (round_restart)
                    round_restart = false;
                RpcReadyAll();
                ReadyAll();
            }
            /*
            else
            {
                ReadyAll();
                CmdReadyAll();
            }
            */

            return;
        }
    }


    [Command]
    public void CmdReadyAll()
    {
        ReadyAll();
    }
    [Command]
    public void CmdResumeAll()
    {
        ResumeAll();
    }
    [ClientRpc]
    public void RpcReadyAll()
    {
        if (isServer)
        {
            Debug.Log("RpcReadyAll called on server;;");
            return;
        }
        ReadyAll();
    }
    [ClientRpc]
    public void RpcResumeAll()
    {
        if (isServer)
        {
            Debug.Log("RpcResumeAll called on server;;");
            return;
        }
        ResumeAll();
    }
    /*
    [Command]
    public void CmdBallClone()
    {
        var cmdBallClone = (GameObject)Instantiate(ball, start_ball_position, Quaternion.identity);

        NetworkServer.Spawn(cmdBallClone);
        //NetworkServer.SpawnWithClientAuthority(gameObject, cmdBallClone);

        //ball_clone = Instantiate(ball, start_ball_position, Quaternion.identity);
    } 
    */


    public void ReadyAll()
    {
        Debug.Log("readyAll");
        isPrevLeft = true;
        round_started = false;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            go.transform.GetComponent<PlayerController>()._Ready();
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Racket"))
        {
            Debug.Log("gameDatas, tag Racket name is : " + go.transform.name);
            go.transform.GetComponent<RacketScript>()._Ready();
        }

        
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ball"))
        {
            //go.transform.GetComponent<ShuttleCock>()._Ready();
            Destroy(go);
            Debug.Log("destroyed");
        }

        if (isServer)
        {
            ball_clone = (GameObject)Instantiate(ball, start_ball_position, Quaternion.identity);
            NetworkServer.Spawn(ball_clone);
        }
    }
    public void ResumeAll()
    {
        round_started = true;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Racket"))
        {
            RacketScript rs = go.transform.GetComponent<RacketScript>();
            rs._Start();
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            go.transform.GetComponent<PlayerController>()._Start();
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ball"))
        {
            go.transform.GetComponent<ShuttleCock>()._Start();
        }
        
    }
    public void changeTurn()
    {
        if (isPrevLeft)
        {
            GameObject.Find("RightPlayer(Clone)").GetComponent<PlayerController>().racket.GetComponent<RacketScript>().myturn = true;
        }
        else
        {
            GameObject.Find("LeftPlayer(Clone)").GetComponent<PlayerController>().racket.GetComponent<RacketScript>().myturn = true;
        }
        isPrevLeft = !isPrevLeft;
    }

    public void addPlayer()
    {
        
        if (playerCount >= 2)
        {
            Debug.Log("playerCount should not be over 2");
            return;
        }
        Debug.Log("addPlayer called");

        if (playerCount == 0)
        {
            player_left = Instantiate(leftplayer_prefab, startLeftPlayer.transform.position, startLeftPlayer.transform.rotation);
            //leftRacket = Instantiate(leftracket_prefab, startLeftRacket.transform.position, startLeftRacket.transform.rotation);
            NetworkServer.Spawn(player_left);
            //NetworkServer.Spawn(player_left.);
        }

        if (playerCount == 1)
        {
            player_right = Instantiate(rightplayer_prefab, startRightPlayer.transform.position, startRightPlayer.transform.rotation);
            //rightRacket = Instantiate(rightracket_prefab, startRightRacket.transform.position, startRightRacket.transform.rotation);
            NetworkServer.Spawn(player_right);
            //NetworkServer.Spawn(rightRacket);
        }
        playerCount++;
        
        /*
        if (isServer)
        {
            Debug.Log("isServer in addPlayer");
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.name.StartsWith("Left"))
                {
                    Debug.Log("LeftPlayer already exists");
                    return;
                }
            }
            GameObject leftPlayer = (GameObject)Instantiate(leftplayer_prefab, startLeftPlayer.transform.position, startLeftPlayer.transform.rotation);
        }
        else
        {
            Debug.Log("not isServer in addPlayer");
            GameObject rightPlayer = (GameObject)Instantiate(rightplayer_prefab, startRightPlayer.transform.position, startRightPlayer.transform.rotation);
        }
        */
    }

    public void exit()
    {
        //TODO
        if (isServer)
        {

        }
    }
}
