using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class Tank : MonoBehaviour
{
    private GameController gc;

    private bool debugMode = false;


    //tank helpers
    private Gun gun;
    private Turret turret;


    Rigidbody2D rb;

    private float moveSpeed = 5;
    private float rotationSpeed = 80;

    MovementDirection currentMovement;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        turret = GetComponentInChildren<Turret>();
        gun = turret.GetComponentInChildren<Gun>();
        

        gc = FindObjectOfType<GameController>();
        if (gc == null)
        {
            //Debug.LogError();
            Debug.LogWarning("Warning: GameController does not exist in scene. Switching to debug mode");
            debugMode = true;
        }
        else
        {
            gc.AddTankFunctions(TankMovement, turret.TurretRotation, gun.PrimaryFire, gun.SecondaryFire, gun.LoadGun, gun.ChangeAmmo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector2.zero;
        if (debugMode)
        {
            DebugMode();
            return;
        }
    }


    void DebugMode()
    {
        if (Input.GetKey(KeyCode.W))
        {
            TankMovement(null, MovementDirection.North);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            TankMovement(null, MovementDirection.South);
        }

        if (Input.GetKey(KeyCode.A))
        {
            TankMovement(null, MovementDirection.West);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            TankMovement(null, MovementDirection.East);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            turret.TurretRotation(null, MovementDirection.North);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            turret.TurretRotation(null, MovementDirection.South);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            turret.TurretRotation(null, MovementDirection.West);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            turret.TurretRotation(null, MovementDirection.East);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            gun.PrimaryFire(null, TankSim.TankSystems.PrimaryWeaponFireState.Valid);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gun.SecondaryFire(null);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gun.LoadGun(null);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            gun.ChangeAmmo(null);
        }
    }



    public void TankMovement(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {

        if (moveDir == MovementDirection.North)
        {
            rb.velocity = transform.up * moveSpeed;
        }
        else if (moveDir == MovementDirection.South)
        {
            rb.velocity = -transform.up * moveSpeed;
        }

        if (moveDir == MovementDirection.East)
        {
            //transform.rotation.eulerAngles.z += rotationSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
        }
        else if (moveDir == MovementDirection.West)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
        }
    }
}
