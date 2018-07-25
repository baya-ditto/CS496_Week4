using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class shuttleSpawn : NetworkBehaviour
{

    public GameObject shuttlePrefab;
    public int numberOfEnemies;
    public GameObject start_shuttle;
    private Vector3 start_shuttle_position;

    public override void OnStartServer()
    {
        Debug.Log("OnStart Server when is executed?");

        start_shuttle_position = start_shuttle.transform.position;                

        for (int i = 0; i < numberOfEnemies; i++)
        {

            Debug.Log("OnStart Server for loop");
            var enemy = (GameObject)Instantiate(shuttlePrefab, start_shuttle_position, Quaternion.identity);

            Debug.Log("OnStart Server enemy name is "+enemy.name);

            NetworkServer.Spawn(enemy);
        }
    }
}
