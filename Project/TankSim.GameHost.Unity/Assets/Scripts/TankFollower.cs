using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFollower : MonoBehaviour
{

    private Tank _tank;
    // Start is called before the first frame update
    void Start()
    {
        _tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_tank != null)
        {
            transform.position = _tank.transform.position + new Vector3(0, 0, -10);
        }
    }
}
