using UnityEngine;
using System.Collections.Generic; // Add this line to fix the error

public class PlayerController : Singleton<PlayerController>
{
    private float playerSpeed = 10; // speed player moves
    private float toolRotateSpeed = 3;
    public GameObject tool;
    public GameObject basket;
    private List<GameObject> heldIngredients = new List<GameObject>(); // Stores picked-up ingredients
    private float pickupRadius = 2f; // Radius for picking up ingredients

    private Vector3 ingredientOffset = new Vector3(0.5f, 0, 0); // Spacing between held items

    void Start() {
        if (tool == null)
            tool = GameObject.FindWithTag("Tool");
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
            TryPickUpIngredient();
            
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            // Try to pick up an ingredient
            DropIngredient();
            
        }
    }

    void TryPickUpIngredient()
    {
        Collider2D[] nearbyIngredients = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        GameObject closestIngredient = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in nearbyIngredients)
        {
            if (col.CompareTag("Ingredient") && !heldIngredients.Contains(col.gameObject))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIngredient = col.gameObject;
                }
            }
        }

        if (closestIngredient != null)
        {
            PickUpIngredient(closestIngredient); // pick up ingredient
        }
    }

    void PickUpIngredient(GameObject ingredient)
    {
        heldIngredients.Add(ingredient);
        ingredient.transform.SetParent(basket.transform);
        ingredient.GetComponent<Collider2D>().enabled = false;
        
        // Position all ingredients to account for the new one
        UpdateIngredientPositions();
    }

    void DropIngredient()
    {
        int lastIndex = heldIngredients.Count - 1;
        GameObject mostRecent = heldIngredients[lastIndex];
        
        mostRecent.transform.SetParent(null);
        mostRecent.GetComponent<Collider2D>().enabled = true;
        heldIngredients.RemoveAt(lastIndex);
        
        // Re-position remaining ingredients
        UpdateIngredientPositions();
    }

    void UpdateIngredientPositions()
    {
        for (int i = 0; i < heldIngredients.Count; i++)
        {
            heldIngredients[i].transform.localPosition = ingredientOffset * (i + 1);
            heldIngredients[i].transform.localRotation = Quaternion.identity;
        }
    }

  
    void Move()
    {
        if (tool == null) return; // Ensure the tool is not null before moving it

        if (Input.GetKey("down")) // Press up arrow key to move forward on the Y AXIS
        {
            //transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
            transform.eulerAngles = new Vector3(0, 0, 0); // Left
            //tool.transform.Translate(0, playerSpeed * Time.deltaTime, 0);
        }
        
        if (Input.GetKey("up")) // Press down arrow key to move backward on the Y AXIS
        {
            transform.eulerAngles = new Vector3(0, 0, 180); // Left
            //transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
            // tool.transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
        }
        
        if (Input.GetKey("right")) // Press left arrow key to move left on the X AXIS
        {
            transform.eulerAngles = new Vector3(0, 0, 90); // Right
            //transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
            // tool.transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
        }
        
        if (Input.GetKey("left")) // Press right arrow key to move right on the X AXIS
        {
            transform.eulerAngles = new Vector3(0, 0, -90); // Left
            //transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
            // tool.transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
        }

        // Then handle movement if any arrow key is pressed
        if (Input.GetKey("down") || Input.GetKey("up") || 
            Input.GetKey("right") || Input.GetKey("left"))
        {
            // Move in the direction the player is facing
            transform.Translate(Vector3.up * -playerSpeed * Time.deltaTime, Space.Self);
        }

        if (Input.GetKeyDown("space"))
        {
            // Rotate the tool to a specific angle (e.g., 45 degrees around the Z-axis)
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 45f);
            tool.transform.rotation = Quaternion.RotateTowards(tool.transform.rotation, targetRotation, toolRotateSpeed * Time.deltaTime);
        }
    }
}