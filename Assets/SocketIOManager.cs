using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SocketIOManager : MonoBehaviour {

    public GameObject go;
    public GameObject player;
    public PlayerController playerControllerScript;

    SocketIO.SocketIOComponent socket;

    //TimegapClass myclassObject = new TimegapClass();

    public void Start()
    {


        playerControllerScript = player.GetComponent<PlayerController>();


        socket = go.GetComponent<SocketIO.SocketIOComponent>();

        var myclassObject2 = new TimegapClass();

        socket.On("test", TestBoop);
        socket.On("TimeGap", TestGap);        
        socket.On("start", gameStart);

        Time.timeScale = 0.0f;

    }

    public void TestBoop(SocketIO.SocketIOEvent e)
    {
        Debug.Log(string.Format("[name: {0}, data: {1}]", e.name, e.data));
    }

    public void TestGap(SocketIO.SocketIOEvent e)
    {        
        Debug.Log(string.Format("[tag: {0}, TimeGApdata: {1}]", e.name, e.data));

        String a;
        String b;        
        b = e.data.ToString();
        

        int c;
        c = int.Parse(b.Substring(7, 1));

        playerControllerScript.SetVelocity(c);


        //speed = int.Parse(e.data);

    }

    public void gameStart(SocketIO.SocketIOEvent e)
    {
        Time.timeScale = 1f;

    }

    [Serializable]
    public class TimegapClass
    {
        public int speed;
    }


}
