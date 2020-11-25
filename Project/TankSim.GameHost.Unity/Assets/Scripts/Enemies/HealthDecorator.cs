using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDecorator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Zombie enemy = gameObject.GetComponent<Zombie>();
        enemy.AddHealth(1);
        enemy.AddPoints(100);
        float s = 0.02f;
        enemy.transform.localScale += new Vector3(s, s, s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
