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
        
        // Create a list to store valid spawn positions
        List<Vector3> validPositions = new List<Vector3>();
        
        // First, find all valid positions
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                // Check if position is empty and not near breakable blocks
                if (mapManager.BushTypeGrid[x, y] == EGrid.Empty)
                {
                    bool isValid = true;
                    
                    // Check surrounding 3x3 area for breakable blocks
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            int checkX = x + dx;
                            int checkY = y + dy;
                            
                            if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                            {
                                // If any surrounding tile is a breakable block, this position is invalid
                                if (mapManager.BushTypeGrid[checkX, checkY] == EGrid.Bush)
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }
                        if (!isValid) break;
                    }
                    
                    if (isValid)
                    {
                        Vector3 pos = new Vector3(x + 0.5f, y + 0.5f, 0);
                        
                        // Check if not too close to player
                        float distToPlayer = Vector3.Distance(pos, PlayerController.Instance.transform.position);
                        if (distToPlayer > 10f)
                        {
                            validPositions.Add(pos);
                        }
                    }
                }
            }
        }
        
        // If we found valid positions, randomly select one
        if (validPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validPositions.Count);
            position = validPositions[randomIndex];
        }
        else
        {
            Debug.LogWarning("No valid spawn positions found for Lost Kid! Using default position.");
        }
        
        return position;
    }
} 