using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim.TankSystems;
using UnityEngine;

public class Gun : MonoBehaviour
{


    public SecondaryBullet secondaryBulletPrefab;


    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void SecondaryFire(IConnectedSystemEndpoint c)
    {
        SecondaryBullet newBullet = Instantiate(secondaryBulletPrefab, transform.position, Quaternion.identity);

        newBullet.GetComponent<Rigidbody2D>().velocity = transform.up * SecondaryBullet.speed;
    }

    

    public void LoadGun(IConnectedSystemEndpoint c)
    {

    }
}
