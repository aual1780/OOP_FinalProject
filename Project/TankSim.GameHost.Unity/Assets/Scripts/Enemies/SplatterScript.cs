using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatterScript : MonoBehaviour
{
    private float _timepassed = 0;
    private readonly float _killtime = 20;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _timepassed += Time.deltaTime;
        float alpha = 1.00f - (_timepassed / _killtime);
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, alpha);
        if (_timepassed >= _killtime)
            Destroy(gameObject);
    }
}
