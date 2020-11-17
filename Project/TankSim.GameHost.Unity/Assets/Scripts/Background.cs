using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{

    public GameObject wallPrefab;
    public GameObject cobblePrefab;

    public int totalCobble = 50;
    public int totalWalls = 10;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 worldSize = new Vector2(transform.localScale.x - 5, transform.localScale.y - 5);

        //cobble
        for (int i = 0; i < totalCobble; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-worldSize.x / 2, worldSize.x / 2), Random.Range(-worldSize.y / 2, worldSize.y / 2), 0);
            Instantiate(cobblePrefab, pos, Quaternion.identity);
        }

        //wall
        for (int i = 0; i < totalWalls; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-worldSize.x / 2, worldSize.x / 2), Random.Range(-worldSize.y / 2, worldSize.y / 2), 0);
            Instantiate(wallPrefab, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
