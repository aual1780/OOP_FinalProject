﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCircle : MonoBehaviour
{

    private int _damage;

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

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Zombie zombie = collision.gameObject.GetComponent<Zombie>();
        _zombies?.Add(zombie);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Zombie zombie = collision.gameObject.GetComponent<Zombie>();
        if (zombie is null)
        {
            return;
        }
        _zombies.Remove(zombie);
    }


    private void DamageEnemies()
    {
        foreach(Zombie zombie in _zombies)
        {
            zombie?.TakeDamage(_damage);
        }
        Destroy(gameObject);
    }
}
