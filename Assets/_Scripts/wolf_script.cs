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

    public int pauseAfterBite = 2;

    // other necessary variables
    private Vector2 v;
    private GameObject player;
    private SpriteRenderer sr;  // extra: flip sprite
    private float prevvx = 0f; // flip sprite

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // assign values to necessary wolf variables
        rb = GetComponent<Rigidbody2D>();
        v = rb.linearVelocity;

        player = GameObject.FindGameObjectWithTag("Player");
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        // wolf follows player and is close
        if (seesPlayer && distToPlayer <= viewRadius)
        {
            if (currentState != states.Bite)
            {
                currentState = states.Chase;
                Chase();
            }
        }
        else
        {
            currentState = states.Wander;
            Wander();
        }

        // flip sprite
        if (v.x != 0)
        {
            sr.flipX = v.x < prevvx;
        }
        prevvx = v.x;

        Debug.Log(currentState);
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
            StartCoroutine(Bite());
        }
    }

    IEnumerator Bite()
    {
        // replace this with bite logic
        Debug.Log("Bite player");

        yield return new WaitForSeconds(pauseAfterBite);

        // go back to chase
        currentState = states.Chase;
        Chase();
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
