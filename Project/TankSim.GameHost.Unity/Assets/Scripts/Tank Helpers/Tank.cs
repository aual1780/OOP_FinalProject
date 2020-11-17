using ArdNet;
using System.Collections;
using System.Collections.Generic;
using TankSim;
using UnityEngine;

public class Tank : MonoBehaviour
{
    private const float _moveSpeed = 5;
    private const float _rotationSpeed = 80;
    private Rigidbody2D _rigidBody;
    private GameController _gameContoller;
    private bool _debugMode = false;
    //tank helpers
    private Gun _gun;
    private Turret _turret;


    // Start is called before the first frame update
    async void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _turret = GetComponentInChildren<Turret>();
        _gun = _turret.GetComponentInChildren<Gun>();


        _gameContoller = FindObjectOfType<GameController>();
        if (_gameContoller == null)
        {
            Debug.LogError("GameController does not exist in scene. Switching to debug mode");
            _debugMode = true;
        }
        else
        {
            await _gameContoller.AddTankFunctions(
                TankMovement,
                _turret.TurretRotation,
                _gun.PrimaryFire,
                _gun.SecondaryFire,
                _gun.LoadGun,
                _gun.ChangeAmmo);
        }
    }

    // Update is called once per frame
    //TODO: should this be on FixedUpdate?
    void Update()
    {
        _rigidBody.velocity = Vector2.zero;
        if (_debugMode)
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
            _turret.TurretRotation(null, MovementDirection.North);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _turret.TurretRotation(null, MovementDirection.South);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _turret.TurretRotation(null, MovementDirection.West);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _turret.TurretRotation(null, MovementDirection.East);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _gun.PrimaryFire(null, TankSim.TankSystems.PrimaryWeaponFireState.Valid);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _gun.SecondaryFire(null);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _gun.LoadGun(null);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _gun.ChangeAmmo(null);
        }
    }



    public void TankMovement(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {

        if (moveDir == MovementDirection.North)
        {
            _rigidBody.velocity = transform.up * _moveSpeed;
        }
        else if (moveDir == MovementDirection.South)
        {
            _rigidBody.velocity = -transform.up * _moveSpeed;
        }

        if (moveDir == MovementDirection.East)
        {
            //transform.rotation.eulerAngles.z += rotationSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, -_rotationSpeed * Time.deltaTime));
        }
        else if (moveDir == MovementDirection.West)
        {
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
        }
    }
}
