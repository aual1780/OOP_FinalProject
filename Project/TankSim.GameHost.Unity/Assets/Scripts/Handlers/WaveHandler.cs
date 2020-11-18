using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    public Zombie ZombiePreFab;
    float _timepassed;
    float _respawnTime = 2;
    int _points = 20;

    readonly int _zombiecost = 3;
    readonly int _healthcost = 1;
    readonly int _speedcost = 2;
    readonly int _damagecost = 3;
    private int _lowestdeccost;

    private int _spawndistance = 20;

    private Tank _tank;
    private GameHandler _handler;


    // Start is called before the first frame update
    void Start()
    {
        _handler = FindObjectOfType<GameHandler>();
        _tank = FindObjectOfType<Tank>();
        //Zombie e = Instantiate(enemyPreFab, new Vector3(5,5,0), Quaternion.identity);
        if (_healthcost <= _speedcost && _healthcost <= _damagecost)
        {
            _lowestdeccost = _healthcost;
        }
       else if (_speedcost <= _healthcost && _speedcost <= _damagecost)
        {
            _lowestdeccost = _speedcost;
        }
       else if (_damagecost <= _speedcost && _damagecost <= _healthcost)
        {
            _lowestdeccost = _damagecost;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timepassed += Time.deltaTime;
        if(_timepassed >= _respawnTime)
        {
            _timepassed -= _respawnTime;
            int temppoints = _points;
            int spend = temppoints;//Random.Range(10, temppoints+1);
            temppoints -= spend;

            Vector2 spawnloc;
            do
            {
                spawnloc = new Vector2(_spawndistance, 0);
                int rot = Random.Range(0, 360);
                //print("rot: " + rot);
                spawnloc = spawnloc.Rotate(rot); // pick a random spot at a spawndistance away from the tank and spawn enemies there
                spawnloc = spawnloc + new Vector2(_tank.transform.position.x, _tank.transform.position.y);
                //print("x: " + spawnloc.x);
                //print("y: " + spawnloc.y);
            } while(spawnloc.x >=25 || spawnloc.x <= -25 || spawnloc.y >= 25 || spawnloc.y <= -25);

            int meth = Random.Range(1, 4); //get num 1-3
            if(meth == 1) //make one unit as tough as possible
            {
                print("method 1");
                spend -= _zombiecost; //add enemy selection here later
                var e = Instantiate(ZombiePreFab, spawnloc, Quaternion.identity);
                e.PassHandler(_handler);
                GameObject obj = e.gameObject;
                obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                while (spend > 0)
                {
                    float prob = Random.Range(0.0f, 1.0f);
                    if (prob <= 0.3f || spend == _healthcost)
                    {
                        obj.AddComponent<HealthDecorator>();
                        spend -= _healthcost;
                    }
                    else if (prob <= 0.7f)
                    {
                        obj.AddComponent<SpeedDecorator>();
                        spend -= _speedcost;
                    }
                    else if (prob <= 1.0f)
                    {
                        obj.AddComponent<DamageDecorator>();
                        spend -= _damagecost;
                    }
                    if(spend <= _lowestdeccost)
                    {
                        break;
                    }
                }
            }
            else if(meth == 2) // small group with decorators
            {
                print("method 2");
                int count = Random.Range(3, spend / 3 + 1);
                GameObject[] obj = new GameObject[count];
                for(int i = 0; i < count; ++i)
                {
                    var e = Instantiate(ZombiePreFab, spawnloc, Quaternion.identity);
                    e.PassHandler(_handler);
                    spend -= _zombiecost;
                    obj[i] = e.gameObject;
                    obj[i].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
                }
                while (spend >= _lowestdeccost * count)
                {
                    float prob = Random.Range(0.0f, 1.0f);
                    if (prob <= 0.3f)
                    {
                        if(spend >= _healthcost * count)
                        {
                            for(int i = 0; i < count; ++i)
                            {
                                obj[i].AddComponent<HealthDecorator>();
                                spend -= _healthcost;
                            }
                        }
                        
                    }
                    else if (prob <= 0.7f)
                    {
                        if (spend >= _speedcost * count)
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                obj[i].AddComponent<SpeedDecorator>();
                                spend -= _speedcost;
                            }
                        }
                    }
                    else if (prob <= 1.0f)
                    {
                        if (spend >= _damagecost * count)
                        {
                            for (int i = 0; i < count; ++i)
                            {
                                obj[i].AddComponent<DamageDecorator>();
                                spend -= _damagecost;
                            }
                        }
                    }
                    if (spend < _lowestdeccost * count)
                    {
                        break;
                    }
                }
            }
            else if(meth == 3) // as many as possible, all weak, without decorators
            {
                print("method 3");
                while (spend >= _zombiecost)
                {
                    var e = Instantiate(ZombiePreFab, spawnloc, Quaternion.identity);
                    e.PassHandler(_handler);
                    spend -= _zombiecost;
                    e.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                }
            }
        }
    }
}

//borrowed from https://answers.unity.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}
