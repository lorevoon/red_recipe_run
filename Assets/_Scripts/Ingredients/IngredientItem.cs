using UnityEngine;
using System.Collections;

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

    [SerializeField] private float throwForce = 5f;  // Added throw force
    [SerializeField] private float throwTorque = 100f;  // Added throw torque for rotation
    [SerializeField] private Vector3 throwOffset = new Vector3(0f, 0f, 0f); 

    private Vector3 originalStartPosition;
    private float timeOffset;
    private bool isBobbing = true;  // Whether or not bobbing is allowed
    private Rigidbody2D rb;  // To hold the Rigidbody2D component

    private void Start()
    {
        originalStartPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
        rb = GetComponent<Rigidbody2D>();

        // Ensure we have a Collider2D
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError($"IngredientItem: No Collider2D found on {gameObject.name}");
        }
    }

    private void Update()
    {
        // If bobbing is allowed, continue bobbing and rotating
        if (isBobbing)
        {
            // Rotate the item visually
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            // Bobbing animation
            float yOffset = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobHeight;
            transform.position = originalStartPosition + new Vector3(0, yOffset, 0);
        }
    }

    // Stop bobbing when the ingredient is picked up
    public void OnPickUp()
    {
        isBobbing = false;
    }

    // Stop bobbing and reset position when the ingredient is dropped
    public void OnDrop()
    {
        isBobbing = false;

        // Update start position to prevent bobbing at the original position when dropped
        originalStartPosition = transform.position;

        // Delay bobbing restart after drop to let it settle
        StartCoroutine(StartBobbingAfterDelay());
    }

     private IEnumerator StartBobbingAfterDelay()
    {
        // Wait for a short time to ensure the object has settled into its final dropped position
        yield return new WaitForSeconds(0.5f); // Adjust the delay as necessary
        isBobbing = true;  // Allow bobbing to start again
        originalStartPosition = transform.position;  // Set the new position as the "start" for bobbing
    }

}
