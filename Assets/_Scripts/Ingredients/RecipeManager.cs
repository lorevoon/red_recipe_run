using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("UI Configuration")]
    [SerializeField] private GameObject recipeEntryPrefab;
    [SerializeField] private GameObject RecipePanel;
    [SerializeField] private Transform scrollViewContent;


    private RecipeList.Recipe currentRecipe;
    private bool isRecipeOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        int randomIndex = Random.Range(0, RecipeList.AllRecipes.Count);
        currentRecipe = RecipeList.AllRecipes[randomIndex];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("RecipeManager: L key detected");
            ToggleRecipe();
        }
    }

    private void ToggleRecipe()
    {
        if (RecipePanel == null)
        {
            Debug.LogError("RecipeManager: RecipePanel is null when trying to toggle!");
            return;
        }

        isRecipeOpen = !isRecipeOpen;
        Debug.Log($"RecipeManager: Setting panel {(isRecipeOpen ? "ACTIVE" : "INACTIVE")}");
        RecipePanel.SetActive(isRecipeOpen);

        if (isRecipeOpen)
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
    }

    public RecipeList.Recipe GetCurrentRecipe()
    {
        return currentRecipe;
    }

}

