using UnityEngine;
using System.Collections.Generic; // Add this line to fix the error
using System.Collections;
using UnityEngine.Rendering.Universal;

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
    public float _baseSpeed = 3f; // base speed without inventory weight
    public float _maxAllowedSpeed = 3f;
    private float _dampingFactor = 0.9f;
    public float ToolSpeed = 3f;

    private Vector2 _lastInputDirection;
    private float _bounceCooldown = 0f;
    private const float _bounceSuppressTime = 0.2f;
    
    // Health UI references
    [SerializeField] private GameObject[] healthHearts; // Array of heart GameObjects
    private int _currentHealth = 3;
    private int _maxHealth = 3;
    
    // Player state
    private bool _isDead = false;

    private void Start()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody2D>();
        
        if (Tool == null)
            Tool = GameObject.FindWithTag("Tool");
            
        // Initialize health from UpgradeManager
        UpdateHealthFromUpgrades();
        
        // Initialize tool speed from UpgradeManager
        UpdateToolSpeedFromUpgrades();
        
        // Initialize base speed
        _baseSpeed = _maxSpeed;
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

        // Update properties from upgrades
        UpdateSpeedFromUpgrades();
        UpdateToolSpeedFromUpgrades();
        
        // Don't process input if player is dead
        if (_isDead) return;
        
        // Check for suicide button (B key) to test game over
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B key pressed - activating game over for testing");
            TestGameOver();
            return;
        }
        
        Move();
        HandlePickup();
    }
    
    public void UpdateSpeedFromUpgrades()
    {
        if (UpgradeManager.Instance != null)
        {
            // Update base speed from upgrade manager
            _baseSpeed = 3f * (1f + (UpgradeManager.Instance.GetSpeedLevel() * 0.25f));
            // Apply inventory weight effect to get final max speed
            _maxSpeed = _baseSpeed * Mathf.Pow(0.9f, _playerInventory.GetCurrentIngredients());
        }
    }
    
    private void UpdateHealthFromUpgrades()
    {
        if (UpgradeManager.Instance != null)
        {
            _maxHealth = UpgradeManager.Instance.GetCurrentHealth();
            _currentHealth = _maxHealth; // Set current health to max health
            
            // Update heart display
            UpdateHeartDisplay();
        }
    }
    
    private void UpdateToolSpeedFromUpgrades()
    {
        if (UpgradeManager.Instance != null)
        {
            // Update tool speed from upgrade manager
            ToolSpeed = UpgradeManager.Instance.GetCurrentToolSpeed();
        }
    }
    
    // Call this when upgrading health
    public void UpgradeHealth()
    {
        UpdateHealthFromUpgrades();
    }
    
    // Call this when player takes damage
    public void TakeDamage(int amount)
    {
        if (_isDead) return;
        
        _currentHealth -= amount;
        
        // Update heart display
        UpdateHeartDisplay();
        
        // Check if player is dead
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        _isDead = true;
        
        // Stop movement
        _rigidbody.linearVelocity = Vector2.zero;
        
        // Get player's current score (coins)
        int score = 0;
        if (UpgradeManager.Instance != null)
        {
            score = UpgradeManager.Instance.GetCoins();
        }
        
        // Show game over screen with player's score
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver(score);
        }
    }
    
    private void UpdateHeartDisplay()
    {
        // Make sure we have hearts to update
        if (healthHearts == null || healthHearts.Length == 0)
        {
            Debug.LogWarning("No heart objects assigned to PlayerController");
            return;
        }
        
        // Show/hide hearts based on current health and max health
        for (int i = 0; i < healthHearts.Length; i++)
        {
            // Show hearts up to current health
            if (i < _currentHealth)
            {
                healthHearts[i].SetActive(true);
            }
            // Hide hearts beyond current health
            else
            {
                healthHearts[i].SetActive(false);
            }
            
            // Hide hearts beyond max health
            if (i >= _maxHealth)
            {
                healthHearts[i].SetActive(false);
            }
        }
        
        Debug.Log($"Updated heart display: {_currentHealth}/{_maxHealth} hearts showing");
    }

    private void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Try to pick up an ingredient
            if (TimeController.Instance.IsNight) return;
            _playerInventory.TryPickUpIngredient();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // if (TimeController.Instance.IsNight) return;
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
    //     // if (collision.gameObject.CompareTag("Bush"))
    //     //     _bounceCooldown = _bounceSuppressTime;
    //         
    //     // Example: Take damage when colliding with an enemy
    //     if (collision.gameObject.CompareTag("Wolf"))
    //     {
    //         TakeDamage(1);
    //     }
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
    
    // For testing purposes - use this to simulate losing all health
    public void TestGameOver()
    {
        Debug.Log("TestGameOver called - simulating player death");
        
        // Set health to zero
        _currentHealth = 0;
        UpdateHeartDisplay();
        
        // Ensure the player is marked as dead
        _isDead = true;
        
        // Stop movement
        _rigidbody.linearVelocity = Vector2.zero;
        
        // Get current coins as score
        int score = 200; // Default test score
        if (UpgradeManager.Instance != null)
        {
            score = UpgradeManager.Instance.GetCoins();
        }
        
        // Create a new GameOverManager prefab if needed
        if (GameOverManager.Instance == null)
        {
            // Create game over manager
            GameObject gameOverObj = new GameObject("GameOverManager");
            gameOverObj.AddComponent<GameOverManager>();
            DontDestroyOnLoad(gameOverObj);
            
            // Give GameOverManager time to initialize
            StartCoroutine(ShowGameOverAfterDelay(score));
        }
        else
        {
            // Show game over screen immediately
            GameOverManager.Instance.ShowGameOver(score);
        }
    }
    
    private IEnumerator ShowGameOverAfterDelay(int score)
    {
        // Wait for GameOverManager to initialize
        yield return new WaitForSeconds(0.5f);
        
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver(score);
        }
        else
        {
            Debug.LogError("Failed to find GameOverManager after creation!");
        }
    }
}