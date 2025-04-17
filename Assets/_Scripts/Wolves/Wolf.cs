using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class Wolf : MonoBehaviour
{
    // pathfinding
    public Node currentNode;
    public List<Node> path = new List<Node>();

    // wolf
    public int speed = 1;
    private EWolfStates currentState = EWolfStates.Wander;
    private EWolfStates previousState;
    public int pause = 3;
    
    // player
    private GameObject player;
    private PlayerHealth playerHealth;
    private bool seesPlayer = false;
    public float viewRadius = 3.0f;
    private GameObject lantern;

    // audio
    private AudioSource audioSource;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lantern = GameObject.FindGameObjectWithTag("Lantern");
        playerHealth = player.GetComponent<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();

        Node[] nodes = FindObjectsOfType<Node>();
        currentNode = FindNearestNode(nodes, transform.position);
    }

    private void Update()
    {
        previousState = currentState;

        seesPlayer = Vector2.Distance(transform.position, player.transform.position) < viewRadius;

        // sees player (and is close) or lantern on...
        if (seesPlayer || lantern.GetComponent<LanternController>()._isLightOn)
        {
            // if not already biting player --> chase
            if (currentState != EWolfStates.Bite)
            {
                currentState = EWolfStates.Chase;
            }    
        }
        else
        {
            currentState = EWolfStates.Wander;
        }
        // bite state dealt with in collide function (OnTriggerEnter2D)

        switch(currentState)
        {
            case EWolfStates.Wander:
                Wander();
                CreatePath();
                break;
            case EWolfStates.Chase:
                Chase();
                CreatePath();
                break;
            case EWolfStates.Bite:
                StartCoroutine(Bite());
                break;
        }
        
        if (previousState != currentState)
        {
            path.Clear();
        }

    }

    private Node FindNearestNode(Node[] nodes, Vector2 position)
    {
        Node nearest = null;
        float minDist = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float dist = Vector2.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = node;
            }
        }

        return nearest;
    }




    private void OnTriggerEnter2D(Collider2D obj)
    {
        // collides with player --> change state to bite and invoke its function
        if (obj.gameObject.CompareTag("Player"))
        {
            currentState = EWolfStates.Bite;
        }
    }

    void Wander()
    {
        speed = 1;
    }

    void Chase()
    {

        if (AStarManager.Instance == null)
        {
            Debug.LogError("AStarManager.Instance is null in Chase!");
            return;
        }

        if (path == null) return;
        speed = 2;

        if (path.Count == 0)
        {
            path = AStarManager.Instance.GeneratePath(currentNode, AStarManager.Instance.FindNearestNode(player.transform.position));
        }
    }

    IEnumerator Bite()
    {
        //Debug.Log("Bite player");
        audioSource.Play(); 

         if (playerHealth != null){
            playerHealth.TakeDamage(1, transform.position);
        }

        path.Clear();
        yield return new WaitForSeconds(pause);

        // go back to chase
        currentState = EWolfStates.Chase;
    }

    public void CreatePath()
    {
        if (currentNode == null)
        {
            Debug.LogError("currentNode is null in CreatePath()!");
            return;
        }

        if (AStarManager.Instance == null)
        {
            Debug.LogError("AStarManager.Instance is null in CreatePath()!");
            return;
        }

        if (path != null && path.Count > 0)
        {
            if (path[0] == null)
            {
                Debug.LogWarning("First node in path is null.");
                return;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(path[0].transform.position.x, path[0].transform.position.y, -2),
                speed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, path[0].transform.position) < 0.1f)
            {
                currentNode = path[0];
                path.RemoveAt(0);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();
            if (nodes.Length == 0)
            {
                Debug.LogError("No nodes found in scene.");
                return;
            }

            int attempts = 0;
            int maxAttempts = 10;

            while ((path == null || path.Count == 0) && attempts < maxAttempts)
            {
                Node randomNode = nodes[Random.Range(0, nodes.Length)];
                path = AStarManager.Instance.GeneratePath(currentNode, randomNode);
                attempts++;
            }

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Failed to generate path after several attempts.");
                return;
            }
        }
    }


    // draw path of wolf
    private void OnDrawGizmos()
    {
        if (path == null) return;
        if(path.Count > 0)
        {
            Gizmos.color = Color.blue;
            for(int i = 1; i < path.Count; i++)
            {
                Gizmos.DrawLine(path[i].transform.position, path[i - 1].transform.position);
            }
        }
    }
}