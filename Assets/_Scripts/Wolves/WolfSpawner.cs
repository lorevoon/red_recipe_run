using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    private GameObject manager;
    private EGrid[,] bushTypeGrid;
    public List<Vector2> emptyList;

    public int amount = 2;
    private List<Vector2> spawnPoints;

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

    }

    void GetEmptyBlockList()
    {
        emptyList = new List<Vector2>();

        int width = bushTypeGrid.GetLength(0);
        int height = bushTypeGrid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (bushTypeGrid[x, y] == EGrid.Empty)
                {
                    emptyList.Add(new Vector2(x, y));
                }
            }
        }
    }

    void GetSpawnPoints()
    {
        spawnPoints = new List<Vector2>();

        for (int i = 0; i < amount; i++)
        {   
            Vector2 spawnPoint = emptyList[Random.Range(0, emptyList.Count)];
            spawnPoints.Add(spawnPoint);
            Debug.Log(spawnPoint);
        }
    }
    
    void Update()
    {   
        GetEmptyBlockList();
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
            wolf.GetComponent<Wolf>().currentPos = spawnPoints[i];
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

    void OnDrawGizmos()
    {
        // Set Gizmo color
        Gizmos.color = Color.green;

        // Draw each walkable block
        foreach (Vector2 block in emptyList)
        {
            Vector3 position = new Vector3(block.x, block.y, 0);
            Gizmos.DrawCube(position, Vector3.one * 1f); // Adjust size as needed
        }
    }

}