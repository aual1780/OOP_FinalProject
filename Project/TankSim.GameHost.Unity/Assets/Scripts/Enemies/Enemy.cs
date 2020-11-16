using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{

    public int Health = 10;
    public float Speed = 0.003F;
    public int Damage = 3;

    private Rigidbody2D rb;
    private Tank tank;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tank = FindObjectOfType<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tank == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 target = tank.transform.position;

        LookAt2D(transform, target);
        setVelocity(transform, target, rb);
        //transform.position = Vector3.MoveTowards(transform.position,tank.transform.position,Speed); //rigidbody.velocity
        
    }

    public static void setVelocity(Transform transform, Vector2 target, Rigidbody2D rb)
    {
        Vector2 current = transform.position;
        var direction = target - current;

        float mag = Mathf.Sqrt((direction.x * direction.x) + (direction.y * direction.y));
        rb.velocity = direction / mag;
    }

    //borrowed from https://forum.unity.com/threads/2d-look-at-object-disappears.390105/
    public static void LookAt2D(Transform transform, Vector2 target)
    {
        Vector2 current = transform.position;
        var direction = target - current;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
