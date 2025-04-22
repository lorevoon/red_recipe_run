using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;
using System.Collections;

public class RecipeManager : Singleton<RecipeManager>
{

    [Header("UI Configuration")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private GameObject RecipePanel;
    [SerializeField] private Transform scrollViewContent;
    
    [Header("Recipe Completion")]
    [SerializeField] private int coinsReward = 20;
    [SerializeField] private UIDocument recipeCompletionPopup;

    // Required for properly displaying the UI
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private PanelSettings panelSettings;

    private SRecipe currentRecipe;
    public Dictionary<EIngredient, int> unspawnedIngredientsInRecipe;
    private int ingredientsLeft;
    private bool isRecipeOpen = false;
    public int difficulty = 0;
    
    // Popup UI elements
    private VisualElement popupContainer;
    private UnityEngine.UIElements.Button closeButton;
    private AudioClip rewardSound;
    private bool popupReady = false;
    private bool waitingForPopupClosure = false;
    private PlayerController playerController;
 


    void Start()
    {
        GenerateNewRecipe();
        SetupPopupUI();
        playerController = PlayerController.Instance;
        
        // Load the mana sound effect
        rewardSound = Resources.Load<AudioClip>("Audio/UI/mana");
        if (rewardSound == null)
        {
            Debug.LogError("Failed to load mana.wav sound effect");
        }
    }
    
    private void SetupPopupUI()
    {
        Debug.Log("Setting up popup UI...");
        
        // Find main canvas if not assigned
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogError("No Canvas found in the scene!");
                return;
            }
        }
        
        // Find or load panel settings
        if (panelSettings == null)
        {
            panelSettings = Resources.Load<PanelSettings>("UI/GameUIPanelSettings");
            if (panelSettings == null)
            {
                Debug.LogError("Failed to load PanelSettings!");
                return;
            }
        }
        
        // Create popup document if not assigned
        if (recipeCompletionPopup == null)
        {
            GameObject popupUIObj = new GameObject("RecipeCompletionPopup");
            popupUIObj.transform.SetParent(mainCanvas.transform, false);
            recipeCompletionPopup = popupUIObj.AddComponent<UIDocument>();
            
            // Set panel settings
            recipeCompletionPopup.panelSettings = panelSettings;
            
            // Load visual tree asset
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>("UI/RecipeCompletedPopup");
            if (visualTree != null)
            {
                recipeCompletionPopup.visualTreeAsset = visualTree;
                Debug.Log("Loaded popup UXML successfully");
            }
            else
            {
                Debug.LogError("Failed to load Recipe Completion Popup UXML");
                return;
            }
            
            // Load and apply stylesheet
            StyleSheet styleSheet = Resources.Load<StyleSheet>("UI/RecipeCompletedPopup");
            if (styleSheet != null && recipeCompletionPopup.rootVisualElement != null)
            {
                recipeCompletionPopup.rootVisualElement.styleSheets.Add(styleSheet);
                Debug.Log("Added stylesheet to popup");
            }
            else
            {
                Debug.LogError("Failed to load Recipe Completion Popup USS");
                return;
            }
        }
        
        // Get UI elements after a frame to ensure UIDocument is ready
        StartCoroutine(InitializePopupUI());
    }
    
    private IEnumerator InitializePopupUI()
    {
        Debug.Log("Waiting to initialize popup UI...");
        yield return null;
        yield return null; // Wait an extra frame to ensure UIDocument is ready
        
        if (recipeCompletionPopup == null)
        {
            Debug.LogError("Recipe Completion Popup is null");
            yield break;
        }
        
        if (recipeCompletionPopup.rootVisualElement == null)
        {
            Debug.LogError("Root visual element is null");
            yield break;
        }
        
        Debug.Log("Root visual element found, searching for popup-container");
        
        popupContainer = recipeCompletionPopup.rootVisualElement.Q("popup-container");
        
        if (popupContainer == null)
        {
            Debug.LogError("Could not find popup-container element");
            // Debug what elements are available
            foreach (var element in recipeCompletionPopup.rootVisualElement.Children())
            {
                Debug.Log("Found element: " + element.name);
            }
            yield break;
        }
        
        Debug.Log("Found popup container, searching for close button");
        
        closeButton = popupContainer.Q<UnityEngine.UIElements.Button>("close-button");
        
        if (closeButton == null)
        {
            Debug.LogError("Could not find close-button element");
            yield break;
        }
        
        Debug.Log("Found close button, adding click event");
        
        closeButton.clicked += () => {
            Debug.Log("Popup close button clicked");
            HideCompletionPopup();
            OnPopupClosed();
        };
        
        // Hide popup initially
        HideCompletionPopup();
        popupReady = true;
        
        Debug.Log("Popup UI initialized successfully");
    }
    
    private void ShowCompletionPopup()
    {
        Debug.Log("Showing completion popup");
        
        if (!popupReady)
        {
            Debug.LogError("Popup not ready yet");
            return;
        }
        
        if (popupContainer != null)
        {
            // Make sure the popup is display:flex before making it visible
            popupContainer.style.display = DisplayStyle.Flex;
            popupContainer.AddToClassList("visible");
            
            // Force UI update
            recipeCompletionPopup.rootVisualElement.MarkDirtyRepaint();
            
            waitingForPopupClosure = true;
            
            // Disable player movement while popup is visible
            if (playerController != null)
            {
                playerController.EnableMovement(false);
            }
            else
            {
                Debug.LogError("PlayerController is null");
            }
        }
        else
        {
            Debug.LogError("Popup container is null");
        }
    }
    
    private void HideCompletionPopup()
    {
        Debug.Log("Hiding completion popup");
        
        if (popupContainer != null)
        {
            popupContainer.RemoveFromClassList("visible");
            waitingForPopupClosure = false;
            
            // Re-enable player movement after popup is closed
            if (playerController != null)
            {
                playerController.EnableMovement(true);
            }
        }
    }

    public void GenerateNewRecipe()
    {
        Debug.Log("Generating new recipe...");
        
        // First, make sure the Lost Kid recipe exists
        SRecipe lostKidRecipe = RecipeList.AllRecipes.FirstOrDefault(r => r.RecipeName == "Lost Kid");
        if (lostKidRecipe.RecipeName == null)
        {
            Debug.LogError("Lost Kid recipe not found in RecipeList.AllRecipes!");
            // Continue with normal recipe generation
        }
        else
        {
            // Give the Lost Kid recipe a 50% chance of being selected
            if (Random.value < 0.5f)
            {
                Debug.Log("Selected Lost Kid recipe (50% chance)");
                currentRecipe = lostKidRecipe;
                ingredientsLeft = GetTotalIngredients(currentRecipe);
                
                // Create a deep copy of the recipe ingredients
                unspawnedIngredientsInRecipe = new Dictionary<EIngredient, int>();
                foreach (var pair in currentRecipe.Ingredients)
                {
                    unspawnedIngredientsInRecipe[pair.Key] = pair.Value;
                }
                
                Debug.Log("New Recipe Generated: " + currentRecipe.RecipeName);
                return; // Skip the rest of the method
            }
            // 50% chance to continue with normal recipe selection
            Debug.Log("Not selecting Lost Kid recipe (50% chance)");
        }
        
        // Normal recipe selection process (for the other 50% of the time)
        List<SRecipe> possibleRecipes = new List<SRecipe>();
        
        switch(difficulty)
        {
            case 0: // 8-11 ingredients
                possibleRecipes = RecipeList.AllRecipes
                    .Where(recipe => GetTotalIngredients(recipe) >= 8 && GetTotalIngredients(recipe) <= 11)
                    .ToList();
                break;
                
            case 1: // Exactly 12 ingredients
                possibleRecipes = RecipeList.AllRecipes
                    .Where(recipe => GetTotalIngredients(recipe) == 12)
                    .ToList();
                break;
                
            case 2: // 13-15 ingredients
                possibleRecipes = RecipeList.AllRecipes
                    .Where(recipe => GetTotalIngredients(recipe) >= 13 && GetTotalIngredients(recipe) <= 15)
                    .ToList();
                break;
        }

        // Fallback to all recipes if none match difficulty
        if(!possibleRecipes.Any())
        {
            Debug.LogWarning($"No recipes found for difficulty {difficulty}. Using all recipes.");
            possibleRecipes = RecipeList.AllRecipes;
        }
        
        // Make sure we don't select Lost Kid in the normal process
        possibleRecipes = possibleRecipes.Where(r => r.RecipeName != "Lost Kid").ToList();
        
        if (!possibleRecipes.Any())
        {
            Debug.LogError("No recipes left after removing Lost Kid! Using all recipes.");
            possibleRecipes = RecipeList.AllRecipes;
        }
        
        // Select a random recipe from the filtered list
        int randomIndex = Random.Range(0, possibleRecipes.Count);
        currentRecipe = possibleRecipes[randomIndex];
        ingredientsLeft = GetTotalIngredients(currentRecipe);
        
        // Create a deep copy of the recipe ingredients
        unspawnedIngredientsInRecipe = new Dictionary<EIngredient, int>();
        foreach (var pair in currentRecipe.Ingredients)
        {
            unspawnedIngredientsInRecipe[pair.Key] = pair.Value;
        }
        
        Debug.Log("New Recipe Generated: " + currentRecipe.RecipeName);
    }

    private int GetTotalIngredients(SRecipe recipe)
    {
        return recipe.Ingredients.Sum(pair => pair.Value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !waitingForPopupClosure)
        {
            isRecipeOpen = !isRecipeOpen;
            ToggleRecipe(isRecipeOpen);
        }
    }

    public void ToggleRecipe(bool isOpen = true)
    {
        RecipePanel.SetActive(isOpen);

        if (isOpen)
        {
            UpdateRecipeUI();
        }
    }

    private void UpdateRecipeUI()
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }
        var entry = Instantiate(recipeEntryPrefab, scrollViewContent);
        entry.GetComponent<RecipeUI>().Initialize(currentRecipe);
        if (scrollViewContent.TryGetComponent<RectTransform>(out var rectTransform))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    public SRecipe GetCurrentRecipe()
    {
        return currentRecipe;
    }

    public bool IsInRecipe(EIngredient ingredient)
    {
        return currentRecipe.Ingredients.TryGetValue(ingredient, out int count) && count > 0;
    }

    public bool SubtractFromRecipe(EIngredient ingredient)
    {
        Debug.Log($"Subtracting {ingredient} from recipe");
        
        if (!(currentRecipe.Ingredients.TryGetValue(ingredient, out int count) && count > 0)) return false;
        
        // Subtract the ingredient from the recipe
        currentRecipe.Ingredients[ingredient] = count - 1;
        Debug.Log($"New count for {ingredient}: {currentRecipe.Ingredients[ingredient]}");
        
        // Check if recipe is complete after subtracting
        if (IsRecipeComplete())
        {
            Debug.Log("Recipe is complete!");
            
            // Award mana
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.AddCoins(coinsReward);
                Debug.Log($"Awarded {coinsReward} mana to player");
                
                // Play mana reward sound
                if (AudioManager.Instance != null && rewardSound != null)
                {
                    AudioManager.Instance.PlaySound(rewardSound);
                }
                else
                {
                    Debug.LogWarning("Could not play reward sound. AudioManager: " + 
                                    (AudioManager.Instance != null ? "Available" : "Null") + 
                                    ", Sound: " + (rewardSound != null ? "Available" : "Null"));
                }
            }
            
            // Increase difficulty if not at max
            if (difficulty < 2)
            {
                difficulty += 1;
                Debug.Log($"Increased difficulty to {difficulty}");
            }
            
            // Show completion popup - this should freeze the game until player clicks button
            ShowCompletionPopup();
            
            // Note: We don't generate a new recipe here - that happens in OnPopupClosed
        }
        else
        {
            // Update the UI if the recipe is open
            if (isRecipeOpen)
            {
                UpdateRecipeUI();
            }
        }
        
        return true;
    }
    
    public void OnPopupClosed()
    {
        Debug.Log("Popup closed, generating new recipe");
        
        // Generate a new recipe after popup is closed
        GenerateNewRecipe();
        
        // Update UI to show the new recipe
        UpdateRecipeUI();
        
        // Show the recipe UI briefly
        bool wasRecipeOpen = isRecipeOpen;
        ToggleRecipe(true);
        
        // If it wasn't open before, close it after a delay
        if (!wasRecipeOpen)
        {
            StartCoroutine(CloseRecipeAfterDelay(3f));
        }
    }
    
    private IEnumerator CloseRecipeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleRecipe(false);
        isRecipeOpen = false;
    }
    
    public bool IsRecipeComplete()
    {
        // Special case for "Lost Kid" recipe which has no ingredients
        if (currentRecipe.RecipeName == "Lost Kid")
        {
            // For the Lost Kid recipe, always return false from this method
            // The recipe will be marked complete through a different game mechanic
            return false;
        }
        
        // Check if all ingredients have counts of 0
        foreach (var ingredient in currentRecipe.Ingredients)
        {
            if (ingredient.Value > 0)
            {
                return false;
            }
        }
        return true;
    }

    public EIngredient GetRandomIngredientInRecipe()
    {
        if (ingredientsLeft <= 0)
        {
            Debug.Log("No ingredients left. Generating a random one");
            Array values = Enum.GetValues(typeof(EIngredient));
            return (EIngredient)values.GetValue(Random.Range(0, values.Length-1));
        }
        
        List<EIngredient> weightedList = new List<EIngredient>();

        foreach (var pair in unspawnedIngredientsInRecipe)
        {
            for (int i = 0; i < pair.Value; i++)
            {
                weightedList.Add(pair.Key);
            }
        }
        
        // Safety check: if the weighted list is empty, return a random ingredient
        if (weightedList.Count == 0)
        {
            Debug.LogWarning("weightedList is empty in GetRandomIngredientInRecipe. Using fallback.");
            Array values = Enum.GetValues(typeof(EIngredient));
            return (EIngredient)values.GetValue(Random.Range(0, values.Length-1));
        }
        
        EIngredient selected = weightedList[Random.Range(0, weightedList.Count)];
        
        unspawnedIngredientsInRecipe[selected]--;
        if (unspawnedIngredientsInRecipe[selected] <= 0)
        {
            unspawnedIngredientsInRecipe.Remove(selected);
        }

        ingredientsLeft--;

        Debug.Log("Generating " + selected);
        return selected;
    }

    public void ForceCompleteLostKidRecipe()
    {
        Debug.Log("Forcing completion of Lost Kid recipe");
        
        if (currentRecipe.RecipeName != "Lost Kid")
        {
            Debug.LogWarning("Cannot force completion - current recipe is not Lost Kid");
            return;
        }
        
        // Award mana
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.AddCoins(coinsReward);
            Debug.Log($"Awarded {coinsReward} mana to player");
            
            // Play mana reward sound
            if (AudioManager.Instance != null && rewardSound != null)
            {
                AudioManager.Instance.PlaySound(rewardSound);
            }
        }
        
        // Increase difficulty if not at max
        if (difficulty < 2)
        {
            difficulty += 1;
            Debug.Log($"Increased difficulty to {difficulty}");
        }
        
        // Show completion popup
        ShowCompletionPopup();
    }
}

