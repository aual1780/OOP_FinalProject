using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class Turret : MonoBehaviour
{

    private const float _rotationSpeed = 70;
    private GunTarget _gunTarget;

    private MovementDirection _currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        _gunTarget = GetComponentInChildren<GunTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((_currentDirection & MovementDirection.East) == MovementDirection.East)
        {
            transform.Rotate(new Vector3(0, 0, -1 * _rotationSpeed * Time.deltaTime));
        }
        else if ((_currentDirection & MovementDirection.West) == MovementDirection.West)
        {
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
        }
    }

    public void TurretRotation(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {
        _currentDirection = moveDir;
        _gunTarget.ChangeAimDistance(moveDir);

        
    }
}
