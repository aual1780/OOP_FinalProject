using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int Health = 10;
    public float Speed = 1F;
    public int Damage = 3;

    private Rigidbody2D rb;
    private Tank tank;
    private float _timepassed = 0;
    private float _hitcooldown = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    protected void Update()
    {
        if (tank == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 target = tank.transform.position;

        LookAt2D(transform, target);
        setVelocity(transform, target, rb, Speed);
        if(Health <= 0)
        {
            //add points, send info to gamecontroller
            Destroy(gameObject);
        }
    }

    public static void setVelocity(Transform transform, Vector2 target, Rigidbody2D rb, float Speed)
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
        if (collision.gameObject.GetComponent<Tank>() != null)
        {
            _timepassed += Time.deltaTime;
            if (_timepassed >= _hitcooldown)
            {
                _timepassed = 0;
                tank.DamageTank(Damage);

            }
        }
        
    }
}
