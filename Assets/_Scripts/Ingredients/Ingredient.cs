using UnityEngine;

public class Ingredient : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private float _moveForce = 40f;
    
    public EIngredient IngredientType;
    [SerializeField] private float weight = 1.0f;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        _rigidbody2D.AddForce(direction.normalized * _moveForce);
    }

    public void OnDrop()
    {
        Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        _rigidbody2D.AddForce(direction.normalized * _moveForce);
    }
    
    // Public properties for controlled access
    // public Ingredients IngredientType => ingredientType;
    // public float Weight => weight;
}