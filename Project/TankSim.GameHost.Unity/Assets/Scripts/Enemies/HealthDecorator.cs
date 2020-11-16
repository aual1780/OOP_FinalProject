using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDecorator : EnemyDecorator
{
    // Start is called before the first frame update
    void Start()
    {
        Enemy enemy = gameObject.GetComponent<Enemy>();
        enemy.Health += 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
