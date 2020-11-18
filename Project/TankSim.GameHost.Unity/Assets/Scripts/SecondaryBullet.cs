using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryBullet : MonoBehaviour
{

    private static int damage = 1;
    public static int speed { get; private set; } = 10;

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
        if (zombie != null)
        {
            zombie.takeDamage(damage);
        }


        Destroy(gameObject);
    }
}
