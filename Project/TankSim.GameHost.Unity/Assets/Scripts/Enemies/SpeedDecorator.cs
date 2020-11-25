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
        float s = 0.02f;
        enemy.transform.localScale += new Vector3(s, s, s);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
