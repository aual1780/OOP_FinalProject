using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    public Zombie enemyPreFab;
    float _timepassed;
    float _respawnTime = 2;
    int _points = 100;

    readonly int _zombiecost = 2;
    readonly int _healthcost = 1;
    readonly int _speedcost = 2;
    readonly int _damagecost = 3;
    int _lowestdeccost;


    // Start is called before the first frame update
    void Start()
    {
        Zombie e = Instantiate(enemyPreFab, new Vector3(5,5,0), Quaternion.identity);
       if(_healthcost <= _speedcost && _healthcost <= _damagecost)
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
            int spend = Random.Range(10, temppoints+1);
            temppoints -= spend;

            Vector3 spawnloc = new Vector3(5,5,0);

            int meth = Random.Range(1, 4); //get num 1-3
            if(meth == 1) //make one unit as tough as possible
            {
                print("method 1");
                spend -= _zombiecost; //add enemy selection here later
                var e = Instantiate(enemyPreFab, spawnloc, Quaternion.identity);
                GameObject obj = e.gameObject;
                obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                while (spend > 0)
                {
                    float prob = Random.Range(0.0f, 1.0f);
                    if (prob <= 0.3f || spend == _healthcost)
                    {
                        HealthDecorator dec = obj.AddComponent<HealthDecorator>();
                        spend -= _healthcost;
                    }
                    else if (prob <= 0.7f)
                    {
                        SpeedDecorator dec = obj.AddComponent<SpeedDecorator>();
                        spend -= _speedcost;
                    }
                    else if (prob <= 1.0f)
                    {
                        DamageDecorator dec = obj.AddComponent<DamageDecorator>();
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
                int count = Random.Range(3, spend / 2 + 1);
                GameObject[] obj = new GameObject[count];
                for(int i = 0; i < count; ++i)
                {
                    var e = Instantiate(enemyPreFab, spawnloc, Quaternion.identity);
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
                                HealthDecorator dec = obj[i].AddComponent<HealthDecorator>();
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
                                SpeedDecorator dec = obj[i].AddComponent<SpeedDecorator>();
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
                                DamageDecorator dec = obj[i].AddComponent<DamageDecorator>();
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
            else if(meth == 3) // as many as possible, all weak
            {
                print("method 3");
                while (spend >= _zombiecost)
                {
                    var e = Instantiate(enemyPreFab, spawnloc, Quaternion.identity);
                    spend -= _zombiecost;
                    e.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0);
                }
            }
        }
    }
}
