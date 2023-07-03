using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public async Task StartClient(string joinCode)
    {
        JoinAllocation joinAllocation;

        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay get join code request failed");
            throw;
        }

        Debug.Log($"client: {joinAllocation.ConnectionData[0]} {joinAllocation.ConnectionData[1]}");
        Debug.Log($"host: {joinAllocation.HostConnectionData[0]} {joinAllocation.HostConnectionData[1]}");
        Debug.Log($"client: {joinAllocation.AllocationId}");

        var relayServerData = new RelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}
