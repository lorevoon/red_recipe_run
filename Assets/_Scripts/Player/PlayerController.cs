using System;
using UnityEngine;
using System.Collections.Generic; // Add this line to fix the error

public class PlayerController : Singleton<PlayerController>
{
    private PlayerInventory _playerInventory;
    private AudioSource _audioSource;
    private Rigidbody2D _rigidbody;
    
    public GameObject Tool;
    public GameObject Basket;
    
    // private float _playerSpeed = 10; // speed player moves
    private float _moveForce = 10f;
    private float _maxSpeed = 3f;
    private float _dampingFactor = 0.9f;
    public float ToolSpeed = 3;

    private Vector2 _lastInputDirection;
    private float _bounceCooldown = 0f;
    private const float _bounceSuppressTime = 0.2f;

    void Start()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        if (Tool == null)
            Tool = GameObject.FindWithTag("Tool");
    }
  
    void Update() {
        Move();
        HandlePickup();
    }

    void HandlePickup()
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bush"))
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
}