using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{

    public GameObject wallPrefab;
    public GameObject cobblePrefab;

    private const int _totalCobble = 50;
    private const int _totalWalls = 10;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 worldSize = new Vector2(transform.localScale.x - 5, transform.localScale.y - 5);

        //cobble
        for (int i = 0; i < _totalCobble; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-worldSize.x / 2, worldSize.x / 2), Random.Range(-worldSize.y / 2, worldSize.y / 2), 0);
            Instantiate(cobblePrefab, pos, Quaternion.identity);
        }

        //wall
        for (int i = 0; i < _totalWalls; ++i)
        {
            //Vector3 pos = new Vector3(Random.Range(-worldSize.x / 2, worldSize.x / 2), Random.Range(-worldSize.y / 2, worldSize.y / 2), 0);
            Instantiate(wallPrefab, RandomWallPosition(worldSize), Quaternion.identity);
        }
    }

    private Vector3 RandomWallPosition(Vector2 worldSize)
    {
        while (true)
        {
            int x = Random.Range(-(int)worldSize.x / 2, (int)worldSize.x / 2);
            int y = Random.Range(-(int)worldSize.y / 2, (int)worldSize.y / 2);
            if (x % 2 == 0 && y % 2 == 0 &&
                (x < -2 || x > 2) && (y < -2 || y > 2))
            {
                return new Vector3(x, y, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
