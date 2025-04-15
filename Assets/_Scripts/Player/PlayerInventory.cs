using System.Collections.Generic;
using UnityEngine;
using System.Collections; // Add this with other using directives

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> InventoryList = new List<GameObject>();

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip dropSound;
    private AudioSource _audioSource;
    
    [Header("Player")]
    private PlayerController _playerController;
    private PlayerAnimation _playerAnimator;
    private float _pickupRadius = 2f; // Radius for picking up ingredients
    private int _max_ingredients = 25;
    private int _curr_ingredients;

    private float _slowing_factor = 0.9f; // each 
    private float _quickening_factor = 1.1f;

    private void Start()
    {
        _playerController = PlayerController.Instance;
        if (_playerController == null)
        {
            Debug.LogError("PlayerController instance not found!");
        }

        // initialize audio
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f; // 2D sound
        }
        
        _curr_ingredients = 0;
    }

    public void AddItem(GameObject ingredient)
    {
        InventoryList.Add(ingredient);
    }

    public GameObject RemoveItem()
    {
        GameObject ingredient = InventoryList[^1];
        InventoryList.RemoveAt(InventoryList.Count - 1);
        return ingredient;
    }

    public bool IsIngredientInInventory(GameObject ingredient)
    {
        return InventoryList.Contains(ingredient);
    }
    
    
    public void TryPickUpIngredient()
    {
        Collider2D[] nearbyIngredients = Physics2D.OverlapCircleAll(transform.position, _pickupRadius);
        GameObject closestIngredient = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in nearbyIngredients)
        {
            if (col.CompareTag("Ingredient") && !InventoryList.Contains(col.gameObject))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIngredient = col.gameObject;
                }
            }
        }

        if (closestIngredient != null && _curr_ingredients < _max_ingredients)
        {
            PickUpIngredient(closestIngredient); // pick up ingredient
            _curr_ingredients += 1;
        }
    }


   public void PickUpIngredient(GameObject ingredient)
   { 
    //    AddItem(ingredient);
    //    Transform basketTransform = PlayerController.Instance.Basket.transform; 
    //    basketTransform.localScale *= 1.05f; // Increase basket size by 5%

    //     _playerController._maxSpeed *= _slowing_factor;
    //     _playerController._moveForce *= _slowing_factor;

    //    ingredient.transform.SetParent(basketTransform);
    //    //ingredient.SetActive(false);

    //    // Hide it but disable collider
    //     SpriteRenderer ing = ingredient.GetComponent<SpriteRenderer>();
    //     if (ing != null) ing.enabled = false;

    //     Collider2D col = ingredient.GetComponent<Collider2D>();
    //     if (col != null) col.enabled = false;

    //    // Add the ingredient to the InventoryManager
    //    Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
    //    if (ingredientComponent != null)
    //    {
    //        InventoryManager.Instance.AddItem(ingredientComponent.IngredientType);
    //    }
        AddItem(ingredient);
        Transform basketTransform = PlayerController.Instance.Basket.transform; 
        basketTransform.localScale *= 1.05f; // Increase basket size by 5%

        _playerController._maxSpeed *= _slowing_factor;
        _playerController._moveForce *= _slowing_factor;

        // Start the movement coroutine

        float left_distance = Vector2.Distance(ingredient.transform.position, _playerController.left_hand_pos.position);
        float right_distance = Vector2.Distance(ingredient.transform.position, _playerController.right_hand_pos.position);

        if (left_distance < right_distance) 
        {
            StartCoroutine(MoveToHand(ingredient, _playerController.left_hand_pos));
        }
        else {
            StartCoroutine(MoveToHand(ingredient, _playerController.right_hand_pos));
        }
        

        // Add the ingredient to the InventoryManager
        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        if (ingredientComponent != null)
        {
            InventoryManager.Instance.AddItem(ingredientComponent.IngredientType);
        }

        // 🔊 Play pickup sound
        if (_audioSource != null && pickupSound != null)
        {
            Debug.Log("Playing pickup sound!");
            _audioSource.PlayOneShot(pickupSound);
        }
        
    }

    public void DropIngredient()
    {
        if (InventoryList.Count <= 0) return;

        GameObject mostRecent = RemoveItem();

        Transform basketTransform = PlayerController.Instance.Basket.transform; 
        basketTransform.localScale *= 0.95f; // Decrease basket size by 5%

        if (_playerController._maxSpeed < _playerController._maxAllowedSpeed)
        {
            float potential_speed = _playerController._maxSpeed * _quickening_factor;
            _playerController._maxSpeed = Mathf.Min(potential_speed, _playerController._maxAllowedSpeed);
            _playerController._moveForce *= _quickening_factor;
        }

        // random offset calculation 
        //Vector2 randomDirection = Random.insideUnitCircle.normalized;
        // float randomDistance = Random.Range(1f, 1.5f); // Always between 0.5-1.5 units
        // Vector3 dropPosition = transform.position + (Vector3)(randomDirection * randomDistance);
        float moveX = Input.GetAxisRaw("Horizontal");
        int facingDirection = _playerController.LastFacingDirection;

       // float offset = _playerController.LastFacingDirection == 1 ? 0f : 1.1f; // X-axis offset only

        // drop away from player
        Vector3 dropOrigin = facingDirection == 1 ? _playerController.right_hand_pos.position : _playerController.right_hand_pos.position ;
        float dropOffset = 0.75f;
        if (facingDirection == 1) {
            dropOffset -= 0.1f;
        }
        Vector3 dropPosition = dropOrigin + new Vector3(facingDirection * dropOffset, 0f, 0f);

        // Call OnDrop BEFORE changing position to disable bobbing and reset startPosition
        IngredientItem item = mostRecent.GetComponent<IngredientItem>();
        if (item != null)
        {
            item.OnDrop(); // reset position to dropped position
        }

        // Unparent the object
        mostRecent.transform.SetParent(null);

        // Make the item appear again
        SpriteRenderer dropped = mostRecent.GetComponent<SpriteRenderer>();
        if (dropped != null)
        {
            dropped.enabled = true;
            dropped.sortingOrder = 999; // On top
        }

        Collider2D col = mostRecent.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        // Temporarily stop physics for smooth positioning
       // Temporarily stop physics for smooth positioning
        Rigidbody2D rb = mostRecent.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.simulated = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Set drop origin to correct hand
            mostRecent.transform.position = dropOrigin;

            // Apply randomized throw force
            float horizontalForce = Random.Range(2f, 4f);   // Sideways
            float verticalForce = Random.Range(-2f, -4f);     // Downward

            Vector2 throwDir = new Vector2(facingDirection * horizontalForce, verticalForce);
            rb.AddForce(throwDir, ForceMode2D.Impulse);
        }

        // Ensure the rotation is reset to avoid unwanted rotation from the parent
        mostRecent.transform.rotation = Quaternion.identity;
        mostRecent.transform.position = dropPosition;
        mostRecent.transform.localScale = Vector3.one;

        mostRecent.GetComponent<Ingredient>().OnDrop();

        // Remove the ingredient from the InventoryManager
        Ingredient ingredientComponent = mostRecent.GetComponent<Ingredient>();
        if (ingredientComponent != null)
        {
            InventoryManager.Instance.RemoveItem(ingredientComponent.IngredientType);
        }

        // 🔊 Play drop sound
        if (_audioSource != null && dropSound != null)
        {
            _audioSource.PlayOneShot(dropSound);
        }
    }

    private IEnumerator MoveToHand(GameObject ingredient, Transform handTransform)
    {
        // Disable physics during collection
        Rigidbody2D rb = ingredient.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }

        // Disable collider immediately
        Collider2D col = ingredient.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startPosition = ingredient.transform.position;
        Vector3 startScale = ingredient.transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            ingredient.transform.position = Vector3.Lerp(
                startPosition, 
                handTransform.position, 
                t
            );
            
            ingredient.transform.localScale = Vector3.Lerp(
                startScale, 
                startScale * 0.5f, 
                t
            );

            yield return null;  // Wait for next frame
        }

        // Final positioning
        ingredient.transform.SetParent(PlayerController.Instance.Basket.transform);
        ingredient.transform.localPosition = Vector3.zero;
        
        SpriteRenderer sr = ingredient.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }

}