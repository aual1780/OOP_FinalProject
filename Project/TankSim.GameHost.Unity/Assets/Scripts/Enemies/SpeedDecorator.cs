using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDecorator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Zombie enemy = gameObject.GetComponent<Zombie>();
        enemy.AddSpeed(0.2F);
        enemy.AddPoints(25);
        float s = 0.005f;
        enemy.Grow(s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
