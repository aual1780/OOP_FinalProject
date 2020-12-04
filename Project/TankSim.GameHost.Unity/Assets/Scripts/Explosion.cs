using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        
        //destroy explosion object when particle duration is over
        Destroy(gameObject, ps.main.duration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
