using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim.TankSystems;
using UnityEngine;

public class Gun : MonoBehaviour
{


    public SecondaryBullet SecondaryBulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FireBullet()
    {
        SecondaryBullet newBullet = Instantiate(SecondaryBulletPrefab, transform.position, Quaternion.identity);

        newBullet.GetComponent<Rigidbody2D>().velocity = transform.up * SecondaryBullet.Speed;

    }

    

    public void SecondaryFire(IConnectedSystemEndpoint c)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(FireBullet);
    }

    

    public void LoadGun(IConnectedSystemEndpoint c)
    {

    }
}
