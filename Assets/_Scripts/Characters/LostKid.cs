using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostKid : MonoBehaviour
{
    // Configuration
    [Header("Movement")]
    [SerializeField] private float followSpeed = 1.5f;
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float detectRadius = 2.0f;
    [SerializeField] private float pathUpdateFrequency = 1f;
    
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private float invincibilityTime = 1.5f;
    private bool isInvincible = false;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // State
    private bool isFollowing = false;
    private Transform playerTransform;
    private Vector3 prevPosition;
    private Rigidbody2D rb;
    
    // Pathfinding
    private Node currentNode;
    private List<Node> path = new List<Node>();
    private float pathUpdateTimer = 0f;
    
    // References
    private RecipeManager recipeManager;
    private GameOverManager gameOverManager;
    private AStarManager pathfindingManager;
    
    private Node lastPlayerNode;
    
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
        pathfindingManager = AStarManager.Instance;
        
        // Initialize health
        currentHealth = maxHealth;
        
        // Initialize pathfinding
        Node[] nodes = FindObjectsOfType<Node>();
        currentNode = FindNearestNode(nodes, transform.position);
        
        // Set up animations
        if (animator != null)
        {
            animator.CrossFade("kid_idle", 0, 0);
        }
    }
    
    void Update()
    {
        if (isFollowing)
        {
            // Always prioritize direct path to player when possible
            if (IsDirectPathClear(transform.position, playerTransform.position))
            {
                // Clear path and follow directly
                path = null;
                FollowDirectly();
            }
            else
            {
                // Update path to player periodically when obstacles are in the way
                pathUpdateTimer -= Time.deltaTime;
                if (pathUpdateTimer <= 0 || path == null || path.Count == 0)
                {
                    UpdatePathToPlayer();
                    pathUpdateTimer = pathUpdateFrequency;
                }
                
                // If we have a path, follow it
                if (path != null && path.Count > 0)
                {
                    FollowPath();
                }
                else
                {
                    // Can't find path, try to move toward player while avoiding obstacles
                    TryToApproachPlayer();
                }
            }
        }
        else
        {
            CheckForPlayer();
        }
        
        UpdateAnimation();
        prevPosition = transform.position;
    }
    
    private void FollowDirectly()
    {
        if (playerTransform == null) return;
        
        // Calculate direct direction to player
        Vector3 direction = playerTransform.position - transform.position;
        float distance = direction.magnitude;
        
        // Only move if we're too far from player
        if (distance > followDistance)
        {
            direction.Normalize();
            rb.linearVelocity = direction * followSpeed;
            
            // Draw debug line to show direct movement
            Debug.DrawLine(transform.position, playerTransform.position, Color.yellow, 0.1f);
        }
        else
        {
            // Stop moving when close enough
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private void TryToApproachPlayer()
    {
        if (playerTransform == null) return;
        
        // Find the nearest node to try to at least get closer to the player
        Node[] nodes = FindObjectsOfType<Node>();
        Node nearestToKid = FindNearestNode(nodes, transform.position);
        
        if (nearestToKid != null)
        {
            // Move toward the nearest node to try to find a path
            Vector3 nodePos = nearestToKid.transform.position;
            if (IsDirectPathClear(transform.position, nodePos))
            {
                Vector3 direction = (nodePos - transform.position).normalized;
                rb.linearVelocity = direction * followSpeed * 0.7f; // Move more cautiously
                
                Debug.DrawLine(transform.position, nodePos, Color.magenta, 0.1f);
                currentNode = nearestToKid; // Update current node for better pathfinding
            }
            else
            {
                // We're stuck, stop moving
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            // No nodes found, stop moving
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private void FollowPath()
    {
        if (path == null || path.Count == 0) return;
        
        // Get the next waypoint position
        Vector3 targetPosition = new Vector3(
            path[0].transform.position.x,
            path[0].transform.position.y, 
            transform.position.z
        );
        
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        // Check if we've reached the current waypoint
        if (distance < 0.1f)
        {
            // Update current node
            currentNode = path[0];
            path.RemoveAt(0);
            
            // If we reached the end of the path, clear it
            if (path.Count == 0)
            {
                path = null;
                
                // Stop moving when we reach the end of the path
                rb.linearVelocity = Vector2.zero;
                return;
            }
        }
        
        // Move toward the next node - use MovePosition for more reliable physics-based movement
        rb.linearVelocity = direction * followSpeed;
        
        // Draw debug line to show target
        Debug.DrawLine(transform.position, targetPosition, Color.green, 0.1f);
    }
    
    private void UpdatePathToPlayer()
    {
        if (pathfindingManager == null) return;
        
        // Make sure we have a current node
        if (currentNode == null)
        {
            Node[] nodes = FindObjectsOfType<Node>();
            currentNode = FindNearestNode(nodes, transform.position);
            if (currentNode == null)
            {
                Debug.LogWarning("Lost Kid: No nodes found in scene!");
                return;
            }
        }
        
        // Find the node nearest to the player
        Node playerNode = pathfindingManager.FindNearestNode(playerTransform.position);
        if (playerNode == null)
        {
            Debug.LogWarning("Lost Kid: No node found near player!");
            return;
        }
        
        // Check if we need to regenerate the path
        bool shouldUpdatePath = true;
        
        // Don't update if already at player's node
        if (currentNode == playerNode)
        {
            shouldUpdatePath = false;
            path = new List<Node>(); // Clear path since we're already at destination node
        }
        
        // Don't update if player is moving and we already have a path
        if (path != null && path.Count > 0 && playerNode == lastPlayerNode)
        {
            shouldUpdatePath = false;
        }
        
        // Generate a new path if needed
        if (shouldUpdatePath)
        {
            List<Node> newPath = pathfindingManager.GeneratePath(currentNode, playerNode);
            
            // Only update path if we found a valid one
            if (newPath != null && newPath.Count > 0)
            {
                path = newPath;
                lastPlayerNode = playerNode;
                Debug.Log($"New path found with {path.Count} nodes");
            }
            else
            {
                Debug.LogWarning("No valid path found to player");
                path = null;
            }
        }
    }
    
    private bool IsDirectPathClear(Vector3 start, Vector3 end)
    {
        // Cast a ray to check if there's a clear line of sight
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        
        // Create a layermask for obstacles - adjust these layer numbers to match your project
        int bushLayer = LayerMask.NameToLayer("Bush");
        int wallLayer = LayerMask.NameToLayer("Wall");
        int obstaclesLayer = LayerMask.NameToLayer("Obstacles");
        
        // If layers don't exist, use a default approach
        int obstaclesMask = 0;
        if (bushLayer != -1) obstaclesMask |= (1 << bushLayer);
        if (wallLayer != -1) obstaclesMask |= (1 << wallLayer);
        if (obstaclesLayer != -1) obstaclesMask |= (1 << obstaclesLayer);
        
        // If no specific layers were found, check everything except player and ingredients
        if (obstaclesMask == 0)
        {
            // Use everything except player and certain tags
            obstaclesMask = Physics2D.AllLayers;
            int playerLayer = LayerMask.NameToLayer("Player");
            int ingredientLayer = LayerMask.NameToLayer("Ingredient");
            if (playerLayer != -1) obstaclesMask &= ~(1 << playerLayer);
            if (ingredientLayer != -1) obstaclesMask &= ~(1 << ingredientLayer);
        }
        
        // Cast the ray against obstacles
        RaycastHit2D hit = Physics2D.Raycast(start, direction.normalized, distance, obstaclesMask);
        
        // Debug raycast visualization
        Debug.DrawRay(start, direction.normalized * distance, hit.collider == null ? Color.green : Color.red, 0.1f);
        
        // If something was hit, check what it is
        if (hit.collider != null)
        {
            // Ignore player and ingredients
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Ingredient"))
            {
                return true; // These aren't obstacles
            }
            
            // Check if it's a bush or wall by checking for specific components or tags
            if (hit.collider.CompareTag("Bush") || 
                hit.collider.CompareTag("Wall"))
            {
                return false; // It's a bush or wall - not clear
            }
            
            // For other unidentified colliders, try checking the layer name
            string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            if (layerName.Contains("Bush") || layerName.Contains("Wall") || layerName.Contains("Obstacle"))
            {
                return false; // Layer name suggests it's an obstacle
            }
            
            // If we're still not sure, check if it's a tilemap collider (often used for terrain)
            if (hit.collider.GetType().Name.Contains("Tilemap"))
            {
                return false; // Assume tilemaps are solid terrain
            }
        }
        
        return true; // No obstacles or only passable objects detected
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
    
    private void CheckForPlayer()
    {
        if (playerTransform == null) return;
        
        // Check if player is within detect radius
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distance <= detectRadius)
        {
            // Start following player
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
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;
        
        // Determine if moving
        bool isMoving = (transform.position - prevPosition).sqrMagnitude > 0.001f;
        
        // Set animation based on movement
        if (isMoving)
        {
            // Direction animations
            Vector2 movement = transform.position - prevPosition;
            
            // Check left/right for flipping
            spriteRenderer.flipX = movement.x < 0;
            
            // Determine if moving more horizontally or vertically
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                // Vertical movement
                if (movement.y > 0)
                {
                    animator.CrossFade("kid_walk_back", 0, 0);
                }
                else
                {
                    animator.CrossFade("kid_walk_front", 0, 0);
                }
            }
            else
            {
                // Horizontal movement
                animator.CrossFade("kid_walk_side", 0, 0);
            }
        }
        else
        {
            // Idle animation
            animator.CrossFade("kid_idle", 0, 0);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we've reached Grandma's house when following the player
        if (isFollowing && other.GetComponent<GrandmasHouse>() != null)
        {
            // Kid has been rescued - complete the recipe!
            if (recipeManager != null && 
                recipeManager.GetCurrentRecipe().RecipeName == "Lost Kid")
            {
                // Force completion of the Lost Kid recipe
                recipeManager.ForceCompleteLostKidRecipe();
                
                // Remove the kid
                gameObject.SetActive(false);
                Destroy(gameObject, 0.5f);
            }
        }
        
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
            animator.CrossFade("kid_idle", 0, 0); // We'll use idle as "death" pose
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
        else
        {
            // If GameOverManager doesn't exist, try to create it
            GameObject gameOverObj = new GameObject("GameOverManager");
            gameOverObj.AddComponent<GameOverManager>();
            
            // Give it time to initialize then show game over
            StartCoroutine(ShowGameOverAfterDelay(0));
        }
        
        // Disable the kid's collider and destroy after delay
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Make the sprite fade out
        StartCoroutine(FadeOut());
        
        // Destroy after animation
        Destroy(gameObject, 3f);
    }
    
    private IEnumerator ShowGameOverAfterDelay(int score)
    {
        yield return new WaitForSeconds(0.5f);
        
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver(score, "The lost child has perished...");
        }
        else
        {
            Debug.LogError("Failed to create GameOverManager!");
        }
    }
    
    private IEnumerator FadeOut()
    {
        if (spriteRenderer == null) yield break;
        
        float duration = 2.0f;
        float elapsedTime = 0f;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        
        spriteRenderer.color = endColor;
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.green;
            
            // Draw current position to first node
            if (Application.isPlaying)
            {
                Gizmos.DrawLine(transform.position, path[0].transform.position);
                
                // Draw rest of path
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);
                }
            }
        }
    }
} 