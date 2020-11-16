using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    public Enemy enemyPreFab;
    float _timepassed;
    float _respawnTime = 2;
    int _points = 100;

    readonly int _zombiecost = 2;
    readonly int _healthcost = 1;
    readonly int _speedcost = 2;
    readonly int _damagecost = 3;


    // Start is called before the first frame update
    void Start()
    {
        Enemy e = Instantiate(enemyPreFab, new Vector3(5,5,0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        _timepassed += Time.deltaTime;
        if(_timepassed >= _respawnTime)
        {
            _timepassed -= _respawnTime;
            int temppoints = _points;
            int spend = Random.Range(2, temppoints+1);
            temppoints -= spend;
            spend -= _zombiecost; //add enemy selection here later
            var e = Instantiate(enemyPreFab, new Vector3(5, 5, 0), Quaternion.identity);
            GameObject obj = e.gameObject;
            while (spend > 0)
            {
                float prob = Random.Range(0.0f, 1.0f);
                if( prob <= 0.3f || spend == _healthcost)
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
            }
            
        }
    }
}
