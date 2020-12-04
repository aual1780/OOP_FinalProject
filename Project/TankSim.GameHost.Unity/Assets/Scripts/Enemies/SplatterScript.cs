using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatterScript : MonoBehaviour
{
    private float _timepassed = 0;
    private float _killtime = 20;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _timepassed += Time.deltaTime;
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1-_timepassed/_killtime);
        if (_timepassed >= _killtime)
            Destroy(gameObject);
    }
}
