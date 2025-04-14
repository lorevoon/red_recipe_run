using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> InventoryList = new List<GameObject>();
    
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
       AddItem(ingredient);
       Transform basketTransform = PlayerController.Instance.Basket.transform; 
       basketTransform.localScale *= 1.05f; // Increase basket size by 5%

        _playerController._maxSpeed *= _slowing_factor;
        _playerController._moveForce *= _slowing_factor;

       ingredient.transform.SetParent(basketTransform);
       //ingredient.SetActive(false);

       // Hide it but disable collider
        SpriteRenderer ing = ingredient.GetComponent<SpriteRenderer>();
        if (ing != null) ing.enabled = false;

        Collider2D col = ingredient.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

       // Add the ingredient to the InventoryManager
       Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
       if (ingredientComponent != null)
       {
           InventoryManager.Instance.AddItem(ingredientComponent.IngredientType);
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

        float offset = _playerController.LastFacingDirection == 1 ? 0f : 1.1f; // X-axis offset only

        Vector3 dropOrigin = _playerController._hand_pos.position;
        Vector3 dropPosition = dropOrigin + new Vector3(offset * facingDirection, 0f, 0f);

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
        Rigidbody2D rb = mostRecent.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // rb.velocity = Vector2.zero;
            // rb.angularVelocity = 0f;
            // rb.isKinematic = true; // temporarily stop physics

            // rb.position = dropPosition;  // Move to drop position
            // rb.rotation = 0f;

            // rb.isKinematic = false;  // re-enable physics

            rb.isKinematic = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            mostRecent.transform.position = dropOrigin;

            // Apply throw force

            float sideways = Random.Range(2.5f, 4f);
            float downward = Random.Range(-1.5f, -3f);
            Vector2 throwDir = new Vector2(sideways * facingDirection, downward).normalized;
            float throwForce = 3f;
            rb.AddForce(throwDir * throwForce, ForceMode2D.Impulse);
        }

        // Ensure the rotation is reset to avoid unwanted rotation from the parent
        mostRecent.transform.rotation = Quaternion.identity;
        mostRecent.transform.position = dropPosition;

        mostRecent.GetComponent<Ingredient>().OnDrop();

        // Remove the ingredient from the InventoryManager
        Ingredient ingredientComponent = mostRecent.GetComponent<Ingredient>();
        if (ingredientComponent != null)
        {
            InventoryManager.Instance.RemoveItem(ingredientComponent.IngredientType);
        }
    }

}