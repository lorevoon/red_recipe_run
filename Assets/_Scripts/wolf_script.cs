using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum states
{
    Wander,
    Chase,
    Bite
}

public class wolf_script : MonoBehaviour
{
    // actual wolf variables
    public int speed = 10;
    private Rigidbody2D rb;
    private states currentState = states.Wander;
    private bool seesPlayer = false;
    private float distToPlayer;
    public float viewRadius = 5;

    // other necessary variables
    private Vector2 v;
    private GameObject player;
    // private SpriteRenderer sr;  // extra: flip sprite

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // assign values to necessary wolf variables
        rb = GetComponent<Rigidbody2D>();
        v = rb.linearVelocity;

        player = GameObject.FindGameObjectWithTag("Player");
        // sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        // wolf follows player and is close
        if (seesPlayer && distToPlayer <= viewRadius)
        {
            currentState = states.Chase;
            Chase();
        }
        else
        {
            currentState = states.Wander;
            Wander();
        }

        // Debug.Log(currentState);
        // Debug.Log(distToPlayer);

        // flip sprite
        // if (v.x != 0)
        // {
        //     sr.flipX = v.x < 0f;
        // }
    }

    private void FixedUpdate()
    {
        // create ray from wolf to player
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
        
        // ray hits something
        if (ray.collider != null)
        {
            // update bool if sees player
            seesPlayer = ray.collider.CompareTag("Player");
            
            // for testing purpose (see green/red line depending on if wolf can see player)
            if (seesPlayer)
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        string objTag = obj.gameObject.tag;

        // collides with player --> change state to bite and invoke its function
        if (objTag == "Player")
        {
            currentState = states.Bite;
            Bite();
        }
    }

    private void Bite()
    {
        Debug.Log("Bite player");
    }

    private void Chase()
    {
        v = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        transform.position = v;
    }

    private void Wander()
    {

    }
}
