using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCircle : MonoBehaviour
{

    public int damage;

    private List<Zombie> _zombies = new List<Zombie>();

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DamageEnemies", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Zombie zombie = collision.gameObject.GetComponent<Zombie>();

        if (zombie != null)
        {
            _zombies.Add(zombie);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Zombie zombie = collision.gameObject.GetComponent<Zombie>();
        if (zombie != null)
        {
            _zombies.Remove(zombie);
        }
    }


    private void DamageEnemies()
    {
        foreach(Zombie zombie in _zombies)
        {
            if (zombie != null)
            {
                zombie.Health -= damage;
            }
        }
        Destroy(gameObject);
    }
}
