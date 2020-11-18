using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDecorator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Zombie enemy = gameObject.GetComponent<Zombie>();
        enemy.Health += 1;
        enemy.points += 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
