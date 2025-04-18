using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI Documents")]
    [SerializeField] private UIDocument gameOverDocument;
    
    [Header("UI Assets")]
    [SerializeField] private string gameOverUXMLPath = "UI/GameOverUI";
    [SerializeField] private string gameOverUSSPath = "UI/GameOverUI";
    [SerializeField] private string panelSettingsPath = "UI/GameUIPanelSettings";
    
    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string gameplaySceneName = "RecipeRun";
    
    // UI Element references
    private VisualElement gameOverRoot;
    private Label scoreValueLabel;
    private Button playAgainButton;
    private Button mainMenuButton;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create UI Documents right away in Awake
            SetupUIDocument();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Initialize UI references - do this after a delay to ensure UIDocument is ready
        StartCoroutine(InitializeUIWithDelay());
    }
    
    private IEnumerator InitializeUIWithDelay()
    {
        // Wait for a frame to ensure UIDocument is ready
        yield return null;
        
        InitializeUI();
        
        // Hide game over screen initially
        HideGameOverScreen();
    }
    
    private void SetupUIDocument()
    {
        Debug.Log("Setting up GameOver UI Document");
        
        // Load panel settings
        PanelSettings panelSettings = Resources.Load<PanelSettings>(panelSettingsPath);
        if (panelSettings == null)
        {
            Debug.LogError("Failed to load PanelSettings from: " + panelSettingsPath);
        }
        
        // Create game over document if not assigned
        if (gameOverDocument == null)
        {
            Debug.Log("Creating new GameOver UIDocument");
            GameObject gameOverUIObj = new GameObject("GameOverUI");
            gameOverUIObj.transform.SetParent(transform);
            gameOverDocument = gameOverUIObj.AddComponent<UIDocument>();
            
            // Load visual tree asset
            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>(gameOverUXMLPath);
            if (visualTree == null)
            {
                Debug.LogError("Failed to load UXML from: " + gameOverUXMLPath);
            }
            else
            {
                gameOverDocument.visualTreeAsset = visualTree;
            }
            
            if (panelSettings != null)
            {
                gameOverDocument.panelSettings = panelSettings;
            }
        }
        
        // Load and apply stylesheet
        StyleSheet styleSheet = Resources.Load<StyleSheet>(gameOverUSSPath);
        if (styleSheet == null)
        {
            Debug.LogError("Failed to load USS from: " + gameOverUSSPath);
        }
        else if (gameOverDocument.rootVisualElement != null)
        {
            gameOverDocument.rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
    
    private void InitializeUI()
    {
        Debug.Log("Initializing GameOver UI elements");
        
        if (gameOverDocument == null)
        {
            Debug.LogError("GameOverDocument is null - cannot initialize UI");
            return;
        }
        
        if (gameOverDocument.rootVisualElement == null)
        {
            Debug.LogError("RootVisualElement is null - waiting for UIDocument to initialize");
            return;
        }
        
        // Get root element
        gameOverRoot = gameOverDocument.rootVisualElement.Q("game-over-container");
        if (gameOverRoot == null)
        {
            Debug.LogError("Could not find game-over-container element. Available elements: " + 
                           string.Join(", ", GetAllElementNames(gameOverDocument.rootVisualElement)));
            return;
        }
        
        // Get UI elements
        scoreValueLabel = gameOverRoot.Q<Label>("score-value");
        if (scoreValueLabel == null)
        {
            Debug.LogError("Could not find score-value label");
        }
        
        playAgainButton = gameOverRoot.Q<Button>("play-again-button");
        if (playAgainButton == null)
        {
            Debug.LogError("Could not find play-again-button");
        }
        
        mainMenuButton = gameOverRoot.Q<Button>("main-menu-button");
        if (mainMenuButton == null)
        {
            Debug.LogError("Could not find main-menu-button");
        }
        
        // Add button event handlers
        if (playAgainButton != null)
        {
            playAgainButton.clicked += OnPlayAgainClicked;
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.clicked += OnMainMenuClicked;
        }
        
        Debug.Log("GameOver UI initialization complete");
    }
    
    // Helper method to list all element names for debugging
    private string[] GetAllElementNames(VisualElement root)
    {
        if (root == null) return new string[0];
        
        List<string> names = new List<string>();
        GetElementNamesRecursive(root, names);
        return names.ToArray();
    }
    
    private void GetElementNamesRecursive(VisualElement element, List<string> names)
    {
        if (!string.IsNullOrEmpty(element.name))
        {
            names.Add(element.name);
        }
        
        foreach (var child in element.Children())
        {
            GetElementNamesRecursive(child, names);
        }
    }
    
    public void ShowGameOver(int score)
    {
        Debug.Log("ShowGameOver called with score: " + score);
        
        // Make sure UI is initialized
        if (gameOverDocument == null || gameOverDocument.rootVisualElement == null || gameOverRoot == null)
        {
            Debug.LogError("Game Over UI not initialized yet - reinitializing");
            SetupUIDocument();
            StartCoroutine(InitAndShowGameOver(score));
            return;
        }
        
        // Update score text
        if (scoreValueLabel != null)
        {
            scoreValueLabel.text = score.ToString();
            Debug.Log("Set score text to: " + score);
        }
        else
        {
            Debug.LogError("scoreValueLabel is null!");
        }
        
        // Show the game over screen
        if (gameOverRoot != null)
        {
            gameOverRoot.style.display = DisplayStyle.Flex; // Ensure it's displayed
            gameOverRoot.AddToClassList("visible");
            Debug.Log("Added 'visible' class to game over root");
        }
        else
        {
            Debug.LogError("gameOverRoot is null!");
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }
    
    private IEnumerator InitAndShowGameOver(int score)
    {
        // Wait for initialization
        yield return new WaitForEndOfFrame();
        InitializeUI();
        
        // Try showing again
        if (scoreValueLabel != null && gameOverRoot != null)
        {
            scoreValueLabel.text = score.ToString();
            gameOverRoot.style.display = DisplayStyle.Flex;
            gameOverRoot.AddToClassList("visible");
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("Failed to initialize UI elements for game over screen");
        }
    }
    
    private void HideGameOverScreen()
    {
        if (gameOverRoot != null)
        {
            gameOverRoot.RemoveFromClassList("visible");
        }
    }
    
    private void OnPlayAgainClicked()
    {
        Debug.Log("Play Again clicked");
        
        // Resume normal time scale
        Time.timeScale = 1f;
        
        // Hide game over screen
        HideGameOverScreen();
        
        // Reload the current level
        SceneManager.LoadScene(gameplaySceneName);
    }
    
    private void OnMainMenuClicked()
    {
        Debug.Log("Main Menu clicked");
        
        // Resume normal time scale
        Time.timeScale = 1f;
        
        // Hide game over screen
        HideGameOverScreen();
        
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
} 