// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI; 
// using System.Linq;
//
// public class RecipeManager : MonoBehaviour
// {
//     public static RecipeManager Instance { get; private set; }
//
//     [Header("UI Configuration")]
//     [SerializeField] private GameObject recipeEntryPrefab;
//     [SerializeField] private GameObject RecipePanel;
//     [SerializeField] private Transform RecipeContent;
//     [SerializeField] private int difficulty = 1;
//
//
//     private SRecipe currentRecipe;
//     private bool isRecipeOpen = false;
//
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
//
//     void Start()
//     {
//         GenerateNewRecipe();
//     }
//
//
//     public void GenerateNewRecipe()
//     {
//         
//         int minIngredients = difficulty + 1;
//         var possibleRecipes = RecipeList.AllRecipes
//             .Where(recipe => recipe.Ingredients.Count == minIngredients)
//             .ToList();
//
//         if (possibleRecipes.Count == 0)
//         {
//             possibleRecipes = RecipeList.AllRecipes;
//         }
//
//         int randomIndex = Random.Range(0, possibleRecipes.Count);
//         SRecipe selectedRecipe = possibleRecipes[randomIndex];
//         currentRecipe = possibleRecipes[randomIndex];
//     }
//
//     public void RenewRecipe()
//     {
//         
//         if (difficulty <= 3){
//             difficulty += 1;
//         }
//         GenerateNewRecipe();
//         UpdateRecipeUI();
//         
//     }
//
//
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.L))
//         {
//             Debug.Log("RecipeManager: L key detected");
//             ToggleRecipe();
//         }
//         if (InventoryManager.Instance.CheckRecipeComplete())
//         {
//             RenewRecipe();
//         }
//
//     }
//
//     private void ToggleRecipe()
//     {
//         if (RecipePanel == null)
//         {
//             Debug.LogError("RecipeManager: RecipePanel is null when trying to toggle!");
//             return;
//         }
//
//         isRecipeOpen = !isRecipeOpen;
//         Debug.Log($"RecipeManager: Setting panel {(isRecipeOpen ? "ACTIVE" : "INACTIVE")}");
//         RecipePanel.SetActive(isRecipeOpen);
//
//         if (isRecipeOpen)
//         {
//             UpdateRecipeUI();
//         }
//     }
//
//
//     private void UpdateRecipeUI()
//     {
//         foreach (Transform child in RecipeContent)
//         {
//             Destroy(child.gameObject);
//         }
//         var entry = Instantiate(recipeEntryPrefab, RecipeContent);
//         // Now passing the full recipe to the UI component
//         entry.GetComponent<RecipeUI>().Initialize(currentRecipe);
//         Canvas.ForceUpdateCanvases();
//         LayoutRebuilder.ForceRebuildLayoutImmediate(RecipeContent.GetComponent<RectTransform>());
//
//     }
//     
//     public RecipeList.Recipe GetCurrentRecipe()
//     {
//         return currentRecipe;
//     }
//
// }
//
