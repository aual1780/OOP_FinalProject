using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class GunTarget : MonoBehaviour
{


    private float movementSpeed = 20;


    private float maxDistance = 10;

    private float minDistance = 1;

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
            transform.localPosition += transform.parent.transform.up * movementSpeed * Time.deltaTime;
            if (transform.localPosition.magnitude > maxDistance)
            {
                transform.localPosition = transform.parent.transform.up * maxDistance;
            }
        }
        else if (moveDir == MovementDirection.South)
        {
            transform.localPosition -= transform.parent.transform.up * movementSpeed * Time.deltaTime;
            if (transform.localPosition.magnitude < minDistance)
            {
                transform.localPosition = transform.parent.transform.up * minDistance;
            }
        }
    }
}
