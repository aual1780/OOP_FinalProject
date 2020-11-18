using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDecorator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Zombie enemy = gameObject.GetComponent<Zombie>();
        enemy.addDamage(1);
        enemy.Points += 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
