using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    private int _health = 3;
    private float _speed = 1F;
    private int _damage = 3;

    public int Points { get; private set; } = 100;

    public GameObject SplatterPrefab;

    private Rigidbody2D _rb;
    private Tank _tank;
    private float _lastcheck = 0;
    private float _hitcooldown = 1;

    private GameHandler _handler;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    protected void Update()
    {
        //Health--; //kill switch for testing
        if (_tank == null)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        Vector2 target = _tank.transform.position;

        LookAt2D(transform, target);
        SetVelocity(transform, target, _rb, _speed);
        if (_health <= 0)
        {
            //add points, send info to gamecontroller
            _handler.AddPoints(Points);
            var s = Instantiate(SplatterPrefab, transform.position, transform.rotation);
            s.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        Points += points;
    }

    public static void SetVelocity(Transform transform, Vector2 target, Rigidbody2D rb, float Speed)
    {
        Vector2 current = transform.position;
        var direction = target - current;

        float mag = Mathf.Sqrt((direction.x * direction.x) + (direction.y * direction.y));
        rb.velocity = direction / mag * Speed;
    }

    //borrowed from https://forum.unity.com/threads/2d-look-at-object-disappears.390105/
    public static void LookAt2D(Transform transform, Vector2 target)
    {
        Vector2 current = transform.position;
        var direction = target - current;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tank>() is null)
        {
            return;
        }

        if (Time.time - _lastcheck >= _hitcooldown)
        {
            _lastcheck = Time.time;
            _tank.DamageTank(_damage);
        }

    }
    public void PassHandler(GameHandler h)
    {
        _handler = h;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
    }

    public void AddHealth(int health)
    {
        _health += health;
    }

    public void AddSpeed(float speed)
    {
        _speed += speed;
    }

    public void AddDamage(int damage)
    {
        _damage += damage;
    }



}
