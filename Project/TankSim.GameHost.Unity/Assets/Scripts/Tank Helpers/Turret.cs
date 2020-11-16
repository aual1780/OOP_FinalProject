using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class Turret : MonoBehaviour
{

    private const float _rotationSpeed = 50;
    private GunTarget _gunTarget;

    // Start is called before the first frame update
    void Start()
    {
        _gunTarget = GetComponentInChildren<GunTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurretRotation(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {
        _gunTarget.ChangeAimDistance(moveDir);

        if (moveDir == MovementDirection.East)
        {
            transform.Rotate(new Vector3(0, 0, -1 * _rotationSpeed * Time.deltaTime));
        }
        else if (moveDir == MovementDirection.West)
        {
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
        }
    }
}
