using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class GunTarget : MonoBehaviour
{
    private const float _movementSpeed = 20;
    private const float _maxDistance = 10;
    private const float _minDistance = 1;

    private MovementDirection _currentDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((_currentDirection & MovementDirection.North) == MovementDirection.North)
        {
            transform.localPosition += Vector3.up * _movementSpeed * Time.deltaTime;
            if (transform.localPosition.magnitude > _maxDistance)
            {
                transform.localPosition = Vector3.up * _maxDistance;
            }
        }
        else if ((_currentDirection & MovementDirection.South) == MovementDirection.South)
        {
            transform.localPosition -= Vector3.up * _movementSpeed * Time.deltaTime;
            if (transform.localPosition.magnitude < _minDistance)
            {
                transform.localPosition = Vector3.up * _minDistance;
            }
        }
    }


    public void ChangeAimDistance(MovementDirection moveDir)
    {
        _currentDirection = moveDir;
        
    }
}
