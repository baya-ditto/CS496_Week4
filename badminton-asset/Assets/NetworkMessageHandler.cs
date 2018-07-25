using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkMessageHandler : NetworkBehaviour
{
    public const short hit_msg = 1337;

    public class HitMessage : MessageBase
    {
        public float degWithZ;
        public float power;
        public int power_direction;
        public bool isUpper;
    }
}


