using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    public Zombie ZombiePreFab;
    float _timepassed;
    private int _points = 20;
    public static float RespawnTime { get; private set; } = 4.0f;

    readonly int _zombiecost = 4;
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
        if(_timepassed >= RespawnTime)
        {
            //int score = FindObjectOfType<GameHandler>().Score;

            _timepassed -= RespawnTime;

            _points += 2;
            int temppoints = _points;
            int spend = temppoints;
            temppoints -= spend;

            Vector2 spawnloc;
            do
            {
                spawnloc = new Vector2(_spawndistance, 0);
                int rot = Random.Range(0, 360);
                spawnloc = spawnloc.Rotate(rot); // pick a random spot at a spawndistance away from the tank and spawn enemies there
                spawnloc = spawnloc + new Vector2(_tank.transform.position.x, _tank.transform.position.y);
            } while(spawnloc.x >=25 || spawnloc.x <= -25 || spawnloc.y >= 25 || spawnloc.y <= -25);

            int meth = Random.Range(0, 100); //get num 0-99
            if(meth < 20) //make one unit as tough as possible
            {
                int speedcount = 0;
                spend -= _zombiecost; //add enemy selection here later
                var e = Instantiate(ZombiePreFab, spawnloc, Quaternion.identity);
                e.PassHandler(_handler);
                GameObject obj = e.gameObject;
                while (spend > 0)
                {
                    float prob = Random.Range(0.0f, 1.0f);
                    if (prob <= 0.3f || spend == _healthcost)
                    {
                        obj.AddComponent<HealthDecorator>();
                        spend -= _healthcost;
                    }
                    else if (prob <= 0.7f && speedcount < 16)
                    {
                        speedcount++;
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
            else if(meth < 80) // small group with decorators
            {
                int count = spend/10;
                GameObject[] obj = new GameObject[count];
                for(int i = 0; i < count; ++i)
                {
                    float r1 = Random.Range(0,0.5f);
                    float r2 = Random.Range(0, 0.5f);
                    Vector2 variation = new Vector2(r1, r2);
                    var e = Instantiate(ZombiePreFab, spawnloc + variation, Quaternion.identity);
                    e.PassHandler(_handler);
                    spend -= _zombiecost;
                    obj[i] = e.gameObject;
                }
                while (spend >= _lowestdeccost * count)
                {
                    int speedcount = 0;
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
                    else if (prob <= 0.7f && speedcount < 16)
                    {
                        speedcount++;
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
            else // as many as possible, all weak, without decorators
            {
                while (spend >= _zombiecost)
                {
                    float r1 = Random.Range(0, 0.5f);
                    float r2 = Random.Range(0, 0.5f);
                    Vector2 variation = new Vector2(r1, r2);
                    var e = Instantiate(ZombiePreFab, spawnloc + variation, Quaternion.identity);
                    e.PassHandler(_handler);
                    spend -= _zombiecost;
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
