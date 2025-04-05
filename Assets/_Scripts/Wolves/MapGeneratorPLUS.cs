using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public partial class MapGenerator
{
    public Node nodePrefab;
    public List<Node> nodeList;

    public Wolf wolf;

    private bool canDrawGizmos;

    public EGrid[,] grid;

    // wolf spawning
    public int amount = 2;

    public TimeController tc;
    private bool isNight = false;

    private bool notSpawned = true;

        // called at end of GenerateMap() in main MapGenerator file
    void CreateNodes()
    {
        grid  = _mapManager.BushTypeGrid;

        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for(int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x,y] == EGrid.Empty) // enum grid stuff
                {
                    Node node = Instantiate(nodePrefab, new Vector2(x+0.5f, y+0.5f), Quaternion.identity);
                    nodeList.Add(node);
                }
            }
        }
        CreateConnections();
    }

    void CreateConnections()
    {
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = i + 1; j < nodeList.Count; j++)
            {
                if (Vector2.Distance(nodeList[i].transform.position, nodeList[j].transform.position) <= 1.0f)
                {
                    ConnectNodes(nodeList[i], nodeList[j]);
                    ConnectNodes(nodeList[j], nodeList[i]);
                }
            }
        }
        // canDrawGizmos = true;
        
    }

    void ConnectNodes(Node from, Node to)
    {
        if(from == to) {return;}
        from.connections.Add(to);
    }

    void SpawnAI()
    {
        Node randNode = nodeList[Random.Range(0, nodeList.Count)];
        Wolf newWolf = Instantiate(wolf, randNode.transform.position, Quaternion.identity);
        newWolf.currentNode = randNode;
    }

    private void OnDrawGizmos()
    {
        if (canDrawGizmos)
        {
            Gizmos.color = Color.blue;
            for(int i = 0; i < nodeList.Count; i++)
            {
                for(int j = 0; j < nodeList[i].connections.Count; j++)
                {
                    Gizmos.DrawLine(nodeList[i].transform.position, nodeList[i].connections[j].transform.position);
                }
            }
        }
    }

        // wolves spawning and destroying
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
        for (int i = 0; i < amount; i++)
        {   
            SpawnAI();
        }
    }

    void destroyWolves()
    {
        foreach (GameObject wolf in GameObject.FindGameObjectsWithTag("Wolf"))
        {   
            Destroy(wolf);
        }
    }
}