using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MyNetworkManager : NetworkManager
{
    public int curPlayer = 0;

    // Prefabs
    public GameObject leftPlayerPrefab;
    public GameObject rightPlayerPrefab;

    // Location Object
    public GameObject leftPlayerPositionObject;
    public GameObject rightPlayerPositionObject;

    public GameObject gamedata;

    //Called on client when connect
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log(string.Format("OnClientConnect called, id : {0}", curPlayer));
        // Create message to set the player
        
        IntegerMessage msg = new IntegerMessage(curPlayer);
        
        // Call Add player and pass the message
        ClientScene.AddPlayer(conn, 0, msg);
    }

    // Server
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        Debug.Log("OnServerAddPlayer called");
        // Read client message and receive index
        if (extraMessageReader != null)
        {
            var stream = extraMessageReader.ReadMessage<IntegerMessage>();
            curPlayer = stream.value;
        }
        //Select the prefab from the spawnable objects list
        var playerPrefab = spawnPrefabs[gamedata.GetComponent<gameDatas>().playerCount];
        Debug.Log(string.Format("OnServerAddPlayer : curPlayer = {0}", curPlayer));

        // Create player object with prefab
        GameObject player;
        if (gamedata.GetComponent<gameDatas>().playerCount == 0)
        {
            player = Instantiate(playerPrefab, leftPlayerPositionObject.transform.position, leftPlayerPositionObject.transform.rotation);
        }
        else if (gamedata.GetComponent<gameDatas>().playerCount == 1)
        {
            player = Instantiate(playerPrefab, rightPlayerPositionObject.transform.position, rightPlayerPositionObject.transform.rotation);
        }
        else
        {
            Debug.Log("Too large curPlayer");
            return;
        }
        gamedata.GetComponent<gameDatas>().playerCount++;
        // Add player object for connection
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        //player.transform.GetChild(0).gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(player.GetComponent<NetworkIdentity>().connectionToClient);
    }
}

