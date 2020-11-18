using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedDecorator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Zombie enemy = gameObject.GetComponent<Zombie>();
        enemy.AddSpeed(0.5F);
        enemy.AddPoints(50);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
