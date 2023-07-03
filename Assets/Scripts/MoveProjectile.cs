using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class MoveProjectile : NetworkBehaviour
{
    public ShootManager parent;
    [SerializeField] float shootForce;
    //[SerializeField] int damage = 10;
    Rigidbody rb;

    /*
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    */
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GetComponent<Rigidbody>().velocity = this.transform.forward * shootForce;
    }
    /*
    private void Update()
    {
        rb.velocity = rb.transform.forward * shootForce;
        //rb.MovePosition(rb.position + rb.transform.forward * shootForce * Time.fixedDeltaTime);
    }*/

}
