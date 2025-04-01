using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    private GameObject _player;
    private SpriteRenderer _spriteRenderer;  // extra: flip sprite
    private Rigidbody2D _rigidBody;
    private AudioSource _audioSource;
    
    // actual wolf variables
    public int chaseSpeed = 5;
    public int wanderSpeed = 2;

    private EWolfStates _currentState = EWolfStates.Wander;

    private bool _seesPlayer = false;
    private float _distToPlayer;
    public float viewRadius = 5;

    public int pauseAfterBite = 2;

    // other necessary variables
    private Vector2 _velocity;
    private float _prevXVelocity = 0f; // flip sprite
    private Vector2 _wanderPoint;

    private GameObject lantern;
    private GameObject spawner;
    private List<Vector2Int> walkable;
    
    void Start()
    {
        // assign values to necessary wolf variables
        _rigidBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        lantern = GameObject.FindGameObjectWithTag("Lantern");

        spawner = GameObject.FindGameObjectWithTag("Spawner");
        walkable = spawner.GetComponent<WolfSpawner>().emptyList;
        
        _velocity = _rigidBody.linearVelocity;
        
        InvokeRepeating(nameof(newWanderPoint), 0, 5);
    }

    void Update()
    {
        float _distToPlayer = Vector2.Distance(transform.position, _player.transform.position);
        
        // wolf follows _player and is close
        if (_seesPlayer && _distToPlayer <= viewRadius)
        {
            if (_currentState != EWolfStates.Bite)
            {
                _currentState = EWolfStates.Chase;
                Chase();
            }
        }
        else if (lantern.GetComponent<LanternController>()._isLightOn == true)
        {
            _currentState = EWolfStates.Chase;
            Chase();
        }
        else
        {
            _currentState = EWolfStates.Wander;
            StartCoroutine(Wander());
        }

        // flip sprite
        if (_velocity.x != 0)
        {
            _spriteRenderer.flipX = _velocity.x < _prevXVelocity;
        }
        _prevXVelocity = _velocity.x;

        // Debug.Log(_currentState);

        // stop from moving out of window
        // Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        // if (screenPos.x <= 0 - 50)
        // {
        //     screenPos.x = 0 - 50;
        // }
        // else if (screenPos.y <= 0 - 50)
        // {
        //     screenPos.y = 0 - 50;
        // }
        // else if (screenPos.x >= Screen.width + 50)
        // {
        //     screenPos.x = Screen.width + 50;
        // }
        // else if (screenPos.y >= Screen.height + 50)
        // {
        //     screenPos.y = Screen.height + 50;
        // }
        // transform.position = Camera.main.ScreenToWorldPoint(screenPos);

    }

    private void FixedUpdate()
    {
        // create ray from wolf to _player
        RaycastHit2D ray = Physics2D.Raycast(transform.position, _player.transform.position - transform.position);
        
        // ray hits something
        if (ray.collider != null)
        {
            // update bool if sees _player
            _seesPlayer = ray.collider.CompareTag("Player");
            
            // for testing purpose (see green/red line depending on if wolf can see _player)
            if (_seesPlayer)
            {
                Debug.DrawRay(transform.position, _player.transform.position - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, _player.transform.position - transform.position, Color.red);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        string objTag = obj.gameObject.tag;

        // collides with _player --> change state to bite and invoke its function
        if (objTag == "Player")
        {
            _currentState = EWolfStates.Bite;
            StartCoroutine(Bite());
        }
        else
        {
            Debug.Log(objTag);
        }
    }

    IEnumerator Bite()
    {
        // replace this with bite logic
        Debug.Log("Bite player");

        yield return new WaitForSeconds(pauseAfterBite);

        // go back to chase
        _currentState = EWolfStates.Chase;
        Chase();
    }

    private void Chase()
    {
        _velocity = Vector2.MoveTowards(transform.position, _player.transform.position, chaseSpeed * Time.deltaTime);
        transform.position = _velocity;
    }

    IEnumerator Wander()
    {
        _velocity = Vector2.MoveTowards(transform.position, _wanderPoint, wanderSpeed * Time.deltaTime);
        transform.position = _velocity;

        yield return new WaitForSeconds(3);
    }

    private void newWanderPoint()
    {
        // _wanderPoint.x = transform.position.x + Random.Range(-10, 10);
        // _wanderPoint.y = transform.position.y + Random.Range(-10, 10);
        _wanderPoint = walkable[Random.Range(0, walkable.Count)];
    }
}
