using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class Turret : MonoBehaviour
{

    private float rotationSpeed = 50;
    private GunTarget gunTarget;

    // Start is called before the first frame update
    void Start()
    {
        gunTarget = GetComponentInChildren<GunTarget>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurretRotation(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {
        gunTarget.ChangeAimDistance(moveDir);

        if (moveDir == MovementDirection.East)
        {
            transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
        }
        else if (moveDir == MovementDirection.West)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }
    }
}
