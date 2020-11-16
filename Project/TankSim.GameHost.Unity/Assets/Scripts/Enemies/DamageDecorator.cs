using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDecorator : EnemyDecorator
{
    // Start is called before the first frame update
    void Start()
    {
        Enemy enemy = gameObject.GetComponent<Enemy>();
        enemy.Damage += 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
