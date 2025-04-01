using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    private GameObject manager;
    private EGrid[,] bushTypeGrid;
    public List<Vector2Int> emptyList;

    public int amount = 2;
    private List<Vector2Int> spawnPoints;

    public TimeController tc;
    private bool isNight = false;

    public GameObject wolf;
    private bool notSpawned = true;
    
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager");
        
        if (manager != null)
        {
            MapManager mapManager = manager.GetComponent<MapManager>();
            
            if (mapManager != null)
            {
                bushTypeGrid = mapManager.BushTypeGrid;
                Debug.Log("BushTypeGrid: " + bushTypeGrid);
            }
            // else
            // {
            //     Debug.LogError("MapManager component not found on the manager GameObject.");
            // }
        }
        // else
        // {
        //     Debug.LogError("Manager not found. Check the GameObject with the 'Manager' tag.");
        // }

        GetEmptyBlockList();
    }

    void GetEmptyBlockList()
    {
        emptyList = new List<Vector2Int>();

        int width = bushTypeGrid.GetLength(0);
        int height = bushTypeGrid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (bushTypeGrid[x, y] == EGrid.Empty)
                {
                    emptyList.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    void GetSpawnPoints()
    {
        spawnPoints = new List<Vector2Int>();

        for (int i = 0; i < amount; i++)
        {   
            Vector2Int spawnPoint = emptyList[Random.Range(0, emptyList.Count)];
            spawnPoints.Add(spawnPoint);
            Debug.Log(spawnPoint);
        }
    }
    
    void Update()
    {
        isNight = tc.GetComponent<TimeController>().IsNight;

        if (isNight && notSpawned)
        {
            spawnWolves();
            notSpawned = false;
        }
        else if (!isNight)
        {
            destroyWolves();
            notSpawned = true;
        }
    }

    void spawnWolves()
    {
        GetSpawnPoints();

        for (int i = 0; i < spawnPoints.Count; i++)
        {   
            Vector3 v = new Vector3(spawnPoints[i].x, spawnPoints[i].y, 0);
            Instantiate(wolf, v, Quaternion.identity);
        }
    }

    void destroyWolves()
    {
        GameObject[] wolves;
        wolves = GameObject.FindGameObjectsWithTag("Wolf");

        foreach (GameObject wolf in wolves)
        {   
            Destroy(wolf);
        }
    }

}