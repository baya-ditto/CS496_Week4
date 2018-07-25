using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterLineScript : MonoBehaviour {

    //public gameDatas gamedata;
	// Use this for initialization
	void Start () {
        //gamedata = ;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Ball"))
        {
            Debug.Log("centerline triggered, change turn");
            GameObject.Find("Main").GetComponent<gameDatas>().changeTurn();
        }
    }
}
