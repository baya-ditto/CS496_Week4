using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class players : NetworkBehaviour {
    public GameObject leftPlayerPrefab;
    public GameObject rightPlayerPrefab;
    // Use this for initialization

    public GameObject startRightPlayer;
    public GameObject startLeftPlayer;

    void Start () {
        if (isServer)
            GameObject.Find("Main").GetComponent<gameDatas>().addPlayer();
        Destroy(gameObject);
    }

        
}
