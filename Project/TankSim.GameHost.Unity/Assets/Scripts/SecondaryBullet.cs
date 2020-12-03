using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryBullet : MonoBehaviour
{

    private const int _damage = 2;
    public static int Speed { get; private set; } = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        Zombie zombie = collision.gameObject.GetComponent<Zombie>();
        zombie?.TakeDamage(_damage);


        Destroy(gameObject);
    }
}
