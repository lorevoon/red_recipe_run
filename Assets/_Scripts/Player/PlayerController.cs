using UnityEngine;
using System.Collections.Generic; // Add this line to fix the error

public class PlayerController : Singleton<PlayerController>
{
    private PlayerInventory _playerInventory;
    private AudioSource _audioSource;
    private Rigidbody2D _rigidbody;
    
    public GameObject Tool;
    public GameObject Basket;
    public Transform left_hand_pos;
    public Transform right_hand_pos;
    public Transform head;
    public int LastFacingDirection { get; private set; } = 1;
    
    // private float _playerSpeed = 10; // speed player moves
    private bool _isMovementEnabled = true;
    public float _moveForce = 10f;
    public float _maxSpeed = 3f; // speed for player with empty basket

    public float _maxAllowedSpeed = 3f;
    private float _dampingFactor = 0.9f;
    public float ToolSpeed = 3;

    private Vector2 _lastInputDirection;
    private float _bounceCooldown = 0f;
    private const float _bounceSuppressTime = 0.2f;

    private void Start()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        if (Tool == null)
            Tool = GameObject.FindWithTag("Tool");
    }
  
    private void Update()
    {
        if (!_isMovementEnabled) return;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Update last facing direction
        if (moveX != 0)
            LastFacingDirection = moveX > 0 ? 1 : -1;

        if (moveY != 0)
            LastFacingDirection = moveY > 0 ? 0 : 2;

        Move();
        HandlePickup();
    }

    private void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Try to pick up an ingredient
            _playerInventory.TryPickUpIngredient();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _playerInventory.DropIngredient();
        }
    }

    private void Move()
    {
        _bounceCooldown -= Time.deltaTime;
        Vector2 movement = Vector2.zero;

        if (_bounceCooldown <= 0f)
        {
            if (Input.GetKey("down")) movement.y -= 1;
            if (Input.GetKey("up")) movement.y += 1;
            if (Input.GetKey("right")) movement.x += 1;
            if (Input.GetKey("left")) movement.x -= 1;
        }
        
        if (movement.sqrMagnitude > 1)
            movement = movement.normalized;
        
        _rigidbody.AddForce(movement * _moveForce, ForceMode2D.Impulse);
        if (_rigidbody.linearVelocity.magnitude > _maxSpeed)
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _maxSpeed;
        _lastInputDirection = movement;
        
        if (movement != Vector2.zero) return; // if player stops
        
        _rigidbody.linearVelocity *= _dampingFactor;
        if (_rigidbody.linearVelocity.magnitude < 0.1f)
            _rigidbody.linearVelocity = Vector2.zero;
    }
    
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Bush"))
    //         ResetBounce();
    // }

    public void ResetBounce()
    {
        _bounceCooldown = _bounceSuppressTime;
    }
    
    public bool IsInLastInputDirection(Vector3 blockPosition)
    {
        if (_lastInputDirection == Vector2.zero)
            return false;

        Vector2 playerToBlock = (blockPosition - transform.position).normalized;
        float dotProduct = Vector2.Dot(playerToBlock, _lastInputDirection);

        return dotProduct > 0.7f;
    }
    
    public void EnableMovement(bool isEnable = true)
    {
        _isMovementEnabled = isEnable;
    }
}