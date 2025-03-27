using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private float playerSpeed = 10; // speed player moves
    private float toolRotateSpeed = 3;
    public GameObject tool;

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldIngredient == null)
            {
                // Try to pick up an ingredient
                PickUpIngredient();
            }
            else
            {
                // Drop the currently held ingredient
                DropIngredient();
            }
        }
    }

    void PickUpIngredient()
    {
        Collider2D[] nearbyIngredients = Physics2D.OverlapCircleAll(transform.position, pickupRadius);
        GameObject closestIngredient = null;
        float closestDistance = Mathf.Infinity;

        // Find the closest ingredient with the "Ingredient" tag
        foreach (Collider2D col in nearbyIngredients)
        {
            if (col.CompareTag("Ingredient"))
            {
                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIngredient = col.gameObject;
                }
            }
        }

        // If we found an ingredient, pick it up
        if (closestIngredient != null)
        {
            heldIngredient = closestIngredient;
            heldIngredient.transform.SetParent(tool.transform); // Attach to tool
            heldIngredient.transform.localPosition = Vector3.zero; // Center on tool
            heldIngredient.GetComponent<Collider2D>().enabled = false; // Disable collision
        }
    }

    void DropIngredient()
    {
        if (heldIngredient != null)
        {
            heldIngredient.transform.SetParent(null); // Detach from tool
            heldIngredient.GetComponent<Collider2D>().enabled = true; // Re-enable collision
            heldIngredient = null;
        }
    }

  
    void Move()
    {
        if (tool == null) return; // Ensure the tool is not null before moving it

        if (Input.GetKey("down")) // Press up arrow key to move forward on the Y AXIS
        {
            transform.Translate(0, playerSpeed * Time.deltaTime, 0);
            //tool.transform.Translate(0, playerSpeed * Time.deltaTime, 0);
        }
        
        if (Input.GetKey("up")) // Press down arrow key to move backward on the Y AXIS
        {
            transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
            // tool.transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
        }
        
        if (Input.GetKey("right")) // Press left arrow key to move left on the X AXIS
        {
            transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
            // tool.transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
        }
        
        if (Input.GetKey("left")) // Press right arrow key to move right on the X AXIS
        {
            transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
            // tool.transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
        }

        if (Input.GetKeyDown("space"))
        {
            // Rotate the tool to a specific angle (e.g., 45 degrees around the Z-axis)
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 45f);
            tool.transform.rotation = Quaternion.RotateTowards(tool.transform.rotation, targetRotation, toolRotateSpeed * Time.deltaTime);
        }
    }
}