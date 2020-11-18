using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim.TankSystems;
using UnityEngine;

public class Gun : MonoBehaviour
{


    public SecondaryBullet secondaryBulletPrefab;


    private bool _canFire = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_canFire)
        {
            FireBullet();
            _canFire = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FireBullet()
    {
        SecondaryBullet newBullet = Instantiate(secondaryBulletPrefab, transform.position, Quaternion.identity);

        newBullet.GetComponent<Rigidbody2D>().velocity = transform.up * SecondaryBullet.Speed;
    }

    

    public void SecondaryFire(IConnectedSystemEndpoint c)
    {
        
        //_canFire = true;
    }

    

    public void LoadGun(IConnectedSystemEndpoint c)
    {

    }
}
