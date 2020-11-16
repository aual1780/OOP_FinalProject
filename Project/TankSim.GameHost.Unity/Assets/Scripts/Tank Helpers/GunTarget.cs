using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class GunTarget : MonoBehaviour
{
    private const float _movementSpeed = 20;
    private const float _maxDistance = 10;
    private const float _minDistance = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeAimDistance(MovementDirection moveDir)
    {
        if (moveDir == MovementDirection.North)
        {
            transform.position += transform.up * _movementSpeed * Time.deltaTime;
            if (transform.position.magnitude > _maxDistance)
            {
                transform.position = transform.up * _maxDistance;
            }
        }
        else if (moveDir == MovementDirection.South)
        {
            transform.position -= transform.up * _movementSpeed * Time.deltaTime;
            if (transform.position.magnitude < _minDistance)
            {
                transform.position = transform.up * _minDistance;
            }
        }
    }
}
