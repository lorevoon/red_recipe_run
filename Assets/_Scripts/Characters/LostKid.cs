using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostKid : MonoBehaviour
{
    private enum EDirection
    {
        East,
        West,
        South,
        North
    }
    
    // Configuration
    [Header("Movement")]
    [SerializeField] private float followSpeed = 1.5f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float detectRadius = 2.0f;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private float invincibilityTime = 1.5f;
    private bool isInvincible = false;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private EDirection prevDirection = EDirection.South;
    private static readonly int faceNorth = Animator.StringToHash("faceNorth");
    private static readonly int faceSouth = Animator.StringToHash("faceSouth");
    private static readonly int faceWest = Animator.StringToHash("faceWest");
    private static readonly int faceEast = Animator.StringToHash("faceEast");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    
    // State
    private bool isFollowing = false;
    private Transform playerTransform;
    private Vector3 prevPosition;
    private Rigidbody2D rb;
    
    // References
    private RecipeManager recipeManager;
    private GameOverManager gameOverManager;
    
    private bool isCutsceneActive = false;
    private GrandmasHouse grandmasHouseInRange = null;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        prevPosition = transform.position;
        
        recipeManager = RecipeManager.Instance;
        gameOverManager = GameOverManager.Instance;
        
        // Initialize health
        currentHealth = maxHealth;
        
        // Set up animations
        // if (animator != null)
        // {
        //     animator.CrossFade("kid_idle", 0, 0);
        // }
    }
    
    void Update()
    {
        if (isFollowing)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > followDistance)
            {
                rb.linearVelocity = direction * followSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Update animation based on movement
            UpdateAnimation();
        }
        else
        {
            // Check if player is within detect radius
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= detectRadius)
            {
                StartFollowing();
            }
        }
    }
    
    private void StartFollowing()
    {
        isFollowing = true;
        
        // Play found sound if available
        if (AudioManager.Instance != null)
        {
            AudioClip foundSound = Resources.Load<AudioClip>("Audio/UI/found");
            if (foundSound != null)
            {
                AudioManager.Instance.PlaySound(foundSound);
            }
        }
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;
        
        // Determine if moving
        bool isMoving = rb.linearVelocity != Vector2.zero;
        
        // Set animation based on movement
        if (isMoving)
        {
            animator.SetBool(IsWalking, true);
            // Direction animations
            Vector2 movement = rb.linearVelocity;
            
            // Determine if moving more horizontally or vertically
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                // Vertical movement
                if (movement.y > 0 && prevDirection != EDirection.North)
                {
                    prevDirection = EDirection.North;
                    animator.SetTrigger(faceNorth);
                }
                else if (movement.y < 0 && prevDirection != EDirection.South)
                {
                    prevDirection = EDirection.South;
                    animator.SetTrigger(faceSouth);
                }
            }
            else
            {
                // Horizontal movement
                if (movement.x > 0 && prevDirection != EDirection.East)
                {
                    prevDirection = EDirection.East;
                    animator.SetTrigger(faceEast);
                }
                else if (movement.x < 0 && prevDirection != EDirection.West)
                {
                    prevDirection = EDirection.West;
                    animator.SetTrigger(faceWest);
                }
            }
        }
        else
        {
            // Idle animation
            animator.SetBool(IsWalking, false);
        }
        
        prevPosition = transform.position;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for wolf attacks
        if (other.CompareTag("Wolf") && !isInvincible)
        {
            TakeDamage(1, other.transform.position);
        }
    }
    
    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        if (isInvincible) return;
        
        currentHealth -= damage;
        Debug.Log($"Kid took damage! Health: {currentHealth}/{maxHealth}");
        
        // Play hurt sound
        if (AudioManager.Instance != null)
        {
            AudioClip hurtSound = Resources.Load<AudioClip>("Audio/UI/hurt");
            if (hurtSound != null)
            {
                AudioManager.Instance.PlaySound(hurtSound);
            }
        }
        
        // Visual feedback
        StartCoroutine(FlashSprite());
        
        // Apply knockback
        if (rb != null)
        {
            Vector3 knockbackDirection = (transform.position - attackerPosition).normalized;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }
        
        // Start invincibility
        StartCoroutine(InvincibilityPeriod());
        
        // Check if kid died
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private IEnumerator FlashSprite()
    {
        if (spriteRenderer == null) yield break;
        
        Color normalColor = spriteRenderer.color;
        Color flashColor = Color.red;
        
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator InvincibilityPeriod()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
    
    private void Die()
    {
        Debug.Log("Kid died! Game over!");
        
        // Disable the kid
        rb.linearVelocity = Vector2.zero;
        isFollowing = false;
        
        // Play death animation
        if (animator != null)
        {
            // animator.CrossFade("idle_south", 0, 0); // We'll use idle as "death" pose
            animator.SetBool(IsWalking, false);
        }
        
        // Show game over screen with special message
        if (gameOverManager != null)
        {
            // Get player's current score
            int score = 0;
            if (UpgradeManager.Instance != null)
            {
                score = UpgradeManager.Instance.GetCoins();
            }
            
            gameOverManager.ShowGameOver(score, "The lost child has perished...");
        }
        
        // Disable the kid's collider and destroy after delay
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Make the sprite fade out
        StartCoroutine(FadeOut());
        
        // Destroy after animation
        Destroy(gameObject, 3f);
    }
    
    private IEnumerator FadeOut()
    {
        if (spriteRenderer == null) yield break;
        
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        
        float duration = 2f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
    }
} 