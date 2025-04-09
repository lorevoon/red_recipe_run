using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    private void Start()
    {
        Debug.Log($"ItemPickup: Started on {gameObject.name}");
        if (pickupLayer.value == 0)
        {
            Debug.LogError("ItemPickup: Pickup Layer is not set! Please set it to 'Ingredient' in the Inspector.");
        }
        Debug.Log($"ItemPickup: Current pickup layer value: {pickupLayer.value}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Debug.Log("ItemPickup: Pickup key pressed");
            TryPickupItem();
        }
    }

    private void TryPickupItem()
    {
        Debug.Log($"ItemPickup: Trying to pickup item at position {transform.position}");
        
        // First, check for all 2D colliders using a larger radius for debugging
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius * 2);
        Debug.Log($"ItemPickup: Found {allColliders.Length} total colliders in extended range");
        foreach (Collider2D col in allColliders)
        {
            Debug.Log($"ItemPickup: Found collider '{col.gameObject.name}' on layer {LayerMask.LayerToName(col.gameObject.layer)}");
        }

        // Now check for items on the specific layer
        Collider2D[] itemColliders = Physics2D.OverlapCircleAll(transform.position, pickupRadius, pickupLayer);
        Debug.Log($"ItemPickup: Found {itemColliders.Length} colliders on pickup layer");
        
        foreach (Collider2D collider in itemColliders)
        {
            IngredientItem item = collider.GetComponent<IngredientItem>();
            if (item != null)
            {
                Debug.Log($"ItemPickup: Found valid ingredient item: {item.IngredientType}");
                PickupItem(item);
                break;
            }
            else
            {
                Debug.Log($"ItemPickup: Object {collider.gameObject.name} has no IngredientItem component");
            }
        }
    }

    private void PickupItem(IngredientItem item)
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(item.IngredientType);
            Debug.Log($"ItemPickup: Successfully picked up {item.IngredientType}");
            Destroy(item.gameObject);
        }
        else
        {
            Debug.LogError("ItemPickup: InventoryManager instance is null!");
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the pickup radius
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, pickupRadius);
        
        // Draw an extended radius for debugging
        UnityEditor.Handles.color = new Color(1, 1, 0, 0.3f);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, pickupRadius * 2);
    }
} 