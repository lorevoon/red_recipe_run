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
            playerHealth.TakeDamage(1);
        }

        path.Clear();
        yield return new WaitForSeconds(pause);

        // go back to chase
        currentState = EWolfStates.Chase;
    }

    public void CreatePath()
    {
        if (path != null)
        {
            if (path.Count > 0)
            {
                int x = 0;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
                {
                    currentNode = path[x];
                    path.RemoveAt(x);
                }
            }
            else
            {
                Node[] nodes = FindObjectsOfType<Node>();
                while (path.Count == 0)
                {
                    path = AStarManager.Instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
                }
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();
            while (path == null)
            {
                path = AStarManager.Instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
            }
        }
    }

    // draw path of wolf
    private void OnDrawGizmos()
    {
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