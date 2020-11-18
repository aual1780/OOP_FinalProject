using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int Health = 3;
    public float Speed = 1F;
    public int Damage = 3;

    public int points = 100;

    public GameObject splatterprefab;

    private Rigidbody2D rb;
    private Tank tank;
    private float _lastcheck = 0;
    private float _hitcooldown = 1;

    private GameHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    protected void Update()
    {
        //Health--; //kill switch for testing
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
            handler.AddPoints(points);
            var s = Instantiate(splatterprefab,transform.position, transform.rotation);
            s.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
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
            
            if (Time.time - _lastcheck >= _hitcooldown)
            {
                _lastcheck = Time.time;
                tank.DamageTank(Damage);

            }
        }
        
    }
    public void passHandler(GameHandler h)
    {
        handler = h;
    }
}
