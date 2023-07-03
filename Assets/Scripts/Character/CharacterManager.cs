using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;



    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
    }

    protected virtual void Update()
    {
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, characterNetworkManager.networkPosition.Value, ref characterNetworkManager.networkPositionVelocity, characterNetworkManager.networkPositionSmoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void LateUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) { return; }
        if (other.GetComponent<MoveProjectile>() && GetComponent<NetworkObject>().OwnerClientId != other.GetComponent<NetworkObject>().OwnerClientId)
        {
            GetComponent<NetworkHealthState>().HealthPoint.Value -= 10;
            if (GetComponent<NetworkHealthState>().HealthPoint.Value <= 0)
            {
                GetComponent<NetworkHealthState>().HealthPoint.Value = 100;
                DeathClientRpc();
            }
        }
    }

    [ClientRpc]
    private void DeathClientRpc()
    {
        if (IsLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }

}
