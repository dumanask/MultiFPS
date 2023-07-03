using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ShootManager : NetworkBehaviour
{
    [SerializeField] GameObject missileProjectile;
    [SerializeField] Transform basePoint;

    [SerializeField] List<GameObject> spawnedMissiles = new List<GameObject>();

    [ServerRpc]
    public void ShootingServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject missile = Instantiate(missileProjectile, basePoint.position, basePoint.rotation);
        spawnedMissiles.Add(missile);
        missile.GetComponent<MoveProjectile>().parent = this;
        missile.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc()
    {
        GameObject toDestroy = spawnedMissiles[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedMissiles.Remove(toDestroy);
        Destroy(toDestroy);
    }

}