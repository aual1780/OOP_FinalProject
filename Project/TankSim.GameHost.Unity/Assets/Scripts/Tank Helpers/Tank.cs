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


    private MovementDirection _currentMovement = MovementDirection.Stop;


    public static int maxHealth { get; private set; } = 100;
    public int health { get; private set; } = 10;

    private bool _canMove = true;


    // Start is called before the first frame update
    async void Start()
    {
        health = maxHealth;
        _rigidBody = GetComponent<Rigidbody2D>();

        _turret = GetComponentInChildren<Turret>();
        _gun = _turret.GetComponentInChildren<Gun>();


        _gameContoller = FindObjectOfType<GameController>();
        if (_gameContoller == null)
        {
            Debug.LogWarning("GameController does not exist in scene. Switching to debug mode");
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
        }
        if (_canMove)
        {
            Movement();
        }
        

        HealthCheck();
    }


    private void HealthCheck()
    {
        //tank dead
        if (health <= 0)
        {
            if (_canMove)
            {
                Explode();
            }
            
            _canMove = false;
            
        }
    }

    private void Explode()
    {
        //TODO: add explosion
    }

    void DebugMode()
    {

        //tank movement
        MovementDirection moveDir = MovementDirection.Stop;

        if (Input.GetKey(KeyCode.W))
        {
            moveDir |= MovementDirection.North;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDir |= MovementDirection.South;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDir |= MovementDirection.West;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDir |= MovementDirection.East;
            
        }
        TankMovement(null, moveDir);

        //aim movement
        MovementDirection aimDir = MovementDirection.Stop;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            aimDir |= MovementDirection.North;
            
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            aimDir |= MovementDirection.South;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            aimDir |= MovementDirection.West;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            aimDir |= MovementDirection.East;
        }
        _turret.TurretRotation(null, aimDir);



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


    public void DamageTank(int damage)
    {
        if (damage <= 0)
        {
            Debug.LogWarning("Trying to heal the tank instead of damaging it.");
            return;
        }

        health -= damage;
    }

    public void TankMovement(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {
        //print($"dir: {moveDir}");
        _currentMovement = moveDir;
        
    }



    private void Movement()
    {
        if ((_currentMovement & MovementDirection.North) == MovementDirection.North)
        {
            _rigidBody.velocity = transform.up * _moveSpeed;
        }
        else if ((_currentMovement & MovementDirection.South) == MovementDirection.South)
        {
            _rigidBody.velocity = -transform.up * _moveSpeed;
        }
        else if ((_currentMovement & MovementDirection.Stop) == MovementDirection.Stop)
        {
            _rigidBody.velocity = Vector2.zero;
        }

        if ((_currentMovement & MovementDirection.East) == MovementDirection.East)
        {
            //transform.rotation.eulerAngles.z += rotationSpeed * Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, -_rotationSpeed * Time.deltaTime));
        }
        else if ((_currentMovement & MovementDirection.West) == MovementDirection.West)
        {
            transform.Rotate(new Vector3(0, 0, _rotationSpeed * Time.deltaTime));
        }
    }
}
