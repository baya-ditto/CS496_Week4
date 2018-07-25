using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkServerHitListener : NetworkMessageHandler
{
    //private gameDatas gamedata;
    private ShuttleCock sc = null;
    private void Start()
    {
        if (isServer)
        {
            //gamedata = GameObject.Find("Main").GetComponent<gameDatas>();
            RegisterNetworkMessages();
        }
        else
        {
            Debug.Log("register nothing");
            NetworkServer.RegisterHandler(hit_msg, nothing);
        }
    }
    private void nothing(NetworkMessage _msg)
    {
        Debug.Log("nothing called");
        return;
    }
    private void RegisterNetworkMessages()
    {
        Debug.Log("Message enrolling");
        NetworkServer.RegisterHandler(hit_msg, OnReceiveHitMessage);
    }

    private void OnReceiveHitMessage(NetworkMessage _message)
    {
        if (sc == null)
            sc = GameObject.FindGameObjectWithTag("Ball").GetComponent<ShuttleCock>();
        var _msg = _message.ReadMessage<HitMessage>();
        //NetworkServer.SendToAll(hit_msg, _msg);
        Debug.Log("hitball called by message");
        sc.HitBall(_msg.degWithZ, _msg.power, _msg.power_direction, _msg.isUpper);
    }
}
