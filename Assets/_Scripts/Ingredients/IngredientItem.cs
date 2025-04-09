using UnityEngine;

public class IngredientItem : MonoBehaviour
{
    [SerializeField] private EIngredient ingredientType = EIngredient.Apple;
    public EIngredient IngredientType 
    { 
        get => ingredientType;
        set => ingredientType = value;
    }
    
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobHeight = 0.2f;
    
    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
        
        // Ensure we have a Collider2D
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError($"IngredientItem: No Collider2D found on {gameObject.name}");
        }
        
        // Ensure we're on the right layer
        if (gameObject.layer != LayerMask.NameToLayer("Ingredient"))
        {
            Debug.LogWarning($"IngredientItem: {gameObject.name} is not on the Ingredient layer!");
        }
    }

    private void Update()
    {
        // Rotate the item
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        float yOffset = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobHeight;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }
} 