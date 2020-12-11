using ArdNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TankSim;
using UnityEngine;

public class Tank : MonoBehaviour
{
    private const float _moveSpeed = 5;
    private const float _rotationSpeed = 80;
    private Rigidbody2D _rigidBody;
    private GameController _gameContoller;
    //tank helpers
    private Gun _gun;
    private Turret _turret;
    private GunTarget _gunTarget;


    private MovementDirection _currentMovement = MovementDirection.Stop;



    public static int MaxHealth { get; private set; } = 100;
    public int Health { get; private set; } = 10;

    private bool _canMove = true;

    public Explosion explosionPrefab;


    // Start is called before the first frame update
    async void Start()
    {
        Health = MaxHealth;
        _rigidBody = GetComponent<Rigidbody2D>();

        _turret = GetComponentInChildren<Turret>();
        _gun = _turret.GetComponentInChildren<Gun>();
        _gunTarget = _turret.GetComponentInChildren<GunTarget>();

        _gameContoller = FindObjectOfType<GameController>();
        if (_gameContoller == null)
        {
            Debug.LogWarning("GameController does not exist in scene. Switching to debug mode");
            GlobalDebugFlag.IsDebug = true;
        }
        else
        {
            await Task.Delay((int)Math.Ceiling(WaveHandler.RespawnTime));
            await SetTankCommands();
        }
    }

    // Update is called once per frame
    //TODO: should this be on FixedUpdate?
    void Update()
    {
        _rigidBody.velocity = Vector2.zero;
        if (GlobalDebugFlag.IsDebug)
        {
            DebugMode();
        }
        if (_canMove)
        {
            Movement();
        }


        HealthCheck();
    }


    private async Task SetTankCommands()
    {
        print("setting commands");
        await _gameContoller.AddTankFunctions(
                TankMovement,
                _turret.TurretRotation,
                _gunTarget.PrimaryFire,
                _gun.SecondaryFire,
                _gun.LoadGun,
                _gunTarget.ChangeAmmo);
    }


    private void HealthCheck()
    {
        //tank dead
        if (Health <= 0)
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
        _ = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        _gun.DisableGun();
        _gunTarget.DisableGun();

        //disable collider
        GetComponent<BoxCollider2D>().enabled = false;

        //disable Tank Image and Gun Image
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            renderer.enabled = false;
        }
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
            _gunTarget.PrimaryFire(null, TankSim.TankSystems.PrimaryWeaponFireState.Valid);
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
            _gunTarget.ChangeAmmo(null);
        }
    }


    public void DamageTank(int damage)
    {
        if (damage <= 0)
        {
            Debug.LogWarning("Trying to heal the tank instead of damaging it.");
            return;
        }

        Health -= damage;
    }

    public void TankMovement(IConnectedSystemEndpoint c, MovementDirection moveDir)
    {
        if (c is null)
        {
            throw new System.ArgumentNullException(nameof(c));
        }
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
