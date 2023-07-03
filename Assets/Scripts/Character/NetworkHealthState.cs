using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHealthState : NetworkBehaviour
{
    
    public NetworkVariable<int> HealthPoint = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        HealthPoint.Value = 100;
    }

}
