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
    private states currentState;

    // other necessary variables
    private Vector2 v;
    // private SpriteRenderer sr;  // extra: flip sprite
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // assign values to necessary wolf variables
        rb = GetComponent<Rigidbody2D>();
        v = rb.linearVelocity;
        currentState = states.Chase;

        // sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // wolf follows player
        if (currentState == states.Chase)
        {
            Invoke(nameof(Chase), 0);
        }
        // v.x = 0;
        // v.y = 0;
        // transform.position = v;

        // flip sprite
        // if (v.x != 0)
        // {
        //     sr.flipX = v.x < 0f;
        // }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        string objTag = obj.gameObject.tag;

        // collides with player --> change state to bite and invoke its function
        if (objTag == "Player")
        {
            currentState = states.Bite;
            Invoke(nameof(Bite), 0);
            currentState = states.Wander;
        }
    }

    private void Bite()
    {
        Debug.Log("Bite player function");
    }

    private void Chase()
    {
        v = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        transform.position = v;
    }
}
