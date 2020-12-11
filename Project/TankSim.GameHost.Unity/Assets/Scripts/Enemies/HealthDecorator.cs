using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDecorator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Zombie enemy = gameObject.GetComponent<Zombie>();
        enemy.AddHealth(2);
        enemy.AddPoints(75);
        float s = 0.015f;
        enemy.Grow(s);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
