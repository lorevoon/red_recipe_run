using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostKidManager : Singleton<LostKidManager>
{
    [Header("References")]
    [SerializeField] private GameObject lostKidPrefab;
    
    private GameObject currentLostKid;
    private RecipeManager recipeManager;
    private MapManager mapManager;
    
    private void Start()
    {
        recipeManager = RecipeManager.Instance;
        mapManager = MapManager.Instance;
        
        // Subscribe to recipe changed event
        StartCoroutine(CheckForLostKidRecipe());
    }
    
    private IEnumerator CheckForLostKidRecipe()
    {
        // Wait for recipe manager to initialize
        yield return new WaitForSeconds(1f);
        
        // Continuously check if Lost Kid recipe is active
        while (true)
        {
            if (recipeManager != null)
            {
                SRecipe currentRecipe = recipeManager.GetCurrentRecipe();
                
                // If Lost Kid recipe is active and kid not spawned
                if (!string.IsNullOrEmpty(currentRecipe.RecipeName) && 
                    currentRecipe.RecipeName == "Lost Kid" && 
                    currentLostKid == null)
                {
                    SpawnLostKid();
                }
                // If recipe changed and kid should be removed
                else if ((string.IsNullOrEmpty(currentRecipe.RecipeName) || 
                          currentRecipe.RecipeName != "Lost Kid") && 
                         currentLostKid != null)
                {
                    DestroyLostKid();
                }
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    private void SpawnLostKid()
    {
        if (lostKidPrefab == null)
        {
            Debug.LogError("Lost Kid prefab is not assigned!");
            return;
        }
        
        // Find a valid spawn point
        Vector3 spawnPosition = FindRandomEmptyLocation();
        
        // Spawn the Lost Kid
        currentLostKid = Instantiate(lostKidPrefab, spawnPosition, Quaternion.identity);
        
        Debug.Log("Lost Kid spawned at " + spawnPosition);
    }
    
    private void DestroyLostKid()
    {
        if (currentLostKid != null)
        {
            Destroy(currentLostKid);
            currentLostKid = null;
            
            Debug.Log("Lost Kid removed as recipe changed");
        }
    }
    
    private Vector3 FindRandomEmptyLocation()
    {
        // Default position if no empty spaces are found
        Vector3 position = new Vector3(0, 0, 0);
        
        if (mapManager == null)
        {
            Debug.LogError("MapManager is null, can't find empty location!");
            return position;
        }
        
        // Get grid dimensions
        int width = mapManager.BushTypeGrid.GetLength(0);
        int height = mapManager.BushTypeGrid.GetLength(1);
        
        // Try 50 random positions to find an empty spot
        for (int i = 0; i < 50; i++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            
            // Check if position is empty (no bush, wall, etc.)
            if (mapManager.BushTypeGrid[x, y] == EGrid.Empty)
            {
                // Convert grid position to world position
                position = new Vector3(x + 0.5f, y + 0.5f, 0);
                
                // Check if not too close to player
                float distToPlayer = Vector3.Distance(position, PlayerController.Instance.transform.position);
                
                if (distToPlayer > 10f)
                {
                    // Good position found
                    return position;
                }
            }
        }
        
        // If no good positions found in the random attempts, do a more thorough search
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (mapManager.BushTypeGrid[x, y] == EGrid.Empty)
                {
                    position = new Vector3(x + 0.5f, y + 0.5f, 0);
                    
                    // Check if not too close to player
                    float distToPlayer = Vector3.Distance(position, PlayerController.Instance.transform.position);
                    
                    if (distToPlayer > 10f)
                    {
                        // Good position found
                        return position;
                    }
                }
            }
        }
        
        Debug.LogWarning("Couldn't find ideal spawn position for Lost Kid, using last checked position");
        return position;
    }
} 