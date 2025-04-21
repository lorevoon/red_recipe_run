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
    public AudioClip howl, close, bite;
    private bool playedClose, playedBite;

    private Animator anim;

    private SpriteRenderer spriteRenderer;
    private Vector3 prevPos;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lantern = GameObject.FindGameObjectWithTag("Lantern");
        playerHealth = player.GetComponent<PlayerHealth>();

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = howl;
        audioSource.Play();
        playedClose = false;
        playedBite = false;

        anim = GetComponent<Animator>();

        // for flip
        spriteRenderer = GetComponent<SpriteRenderer>();
        prevPos = transform.position;

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
        }
        
        if (previousState != currentState && path != null)
        {
            path.Clear();
        }

        // sfx if close and not biting
        noiseIfClose();

        // animation and flipping
        spriteUpdate();
        prevPos = transform.position;
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
            StartCoroutine(Bite());
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
        anim.CrossFade("wolf_attack", 0, 0);

        if (!playedBite)
        {
            audioSource.clip = bite;
            audioSource.Play();
            playedBite = true;
        }

        if (playerHealth != null){
            playerHealth.TakeDamage(1, transform.position);
        }

        if (path != null)
        {
            path.Clear();
        }

        yield return new WaitForSeconds(pause);

        // go back to chase
        currentState = EWolfStates.Chase;
        playedBite = false;
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

                if (randomNode == null)
                {
                    Debug.LogWarning("Randomly selected node was null.");
                    attempts++;
                    continue;
                }

                path = AStarManager.Instance.GeneratePath(currentNode, randomNode);

                if (path == null || path.Count == 0)
                {
                    Debug.LogWarning($"Path attempt {attempts + 1}: Failed from {currentNode.name} to {randomNode.name}");
                }

                attempts++;
            }


            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Failed to generate path after several attempts.");
                path = new List<Node> { currentNode };
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

    private void noiseIfClose()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < 5.0f && currentState != EWolfStates.Bite)
        {
            if (!playedClose)
            {
                audioSource.clip = close;
                audioSource.Play();
                playedClose = true;
            }
        }
        else
        {
            playedClose = false;
        }
    }

    private void spriteUpdate()
    {
        if (currentState != EWolfStates.Bite)
        {
            if (transform.position == prevPos)
            {
                anim.CrossFade("wolf_idle", 0, 0);
            }
            else
            {
                anim.CrossFade("wolf_run", 0, 0);
            }
        }

        // for flipping
        spriteRenderer.flipX = transform.position.x > prevPos.x;
        
    }
}
