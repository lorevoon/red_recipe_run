using UnityEngine;
using UnityEngine.SceneManagement;

// This class ensures that all UI-related systems are properly initialized
public class UISetup : MonoBehaviour
{
    private static bool isInitialized = false;
    
    [SerializeField] private GameObject uiManagerPrefab;
    
    private void Awake()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            
            // Create UI Manager if it doesn't exist
            if (UIManager.Instance == null && uiManagerPrefab != null)
            {
                Instantiate(uiManagerPrefab);
            }
            else if (UIManager.Instance == null)
            {
                GameObject uiManagerObj = new GameObject("UIManager");
                uiManagerObj.AddComponent<UIManager>();
                DontDestroyOnLoad(uiManagerObj);
            }
            
            // Ensure InventoryManager exists
            if (InventoryManager.Instance == null)
            {
                GameObject inventoryManagerObj = new GameObject("InventoryManager");
                inventoryManagerObj.AddComponent<InventoryManager>();
                DontDestroyOnLoad(inventoryManagerObj);
            }
            
            // Ensure UpgradeManager exists
            if (UpgradeManager.Instance == null)
            {
                GameObject upgradeManagerObj = new GameObject("UpgradeManager");
                upgradeManagerObj.AddComponent<UpgradeManager>();
                DontDestroyOnLoad(upgradeManagerObj);
            }
        }
    }
    
    // Reset initialization state when returning to main menu
    private void OnDestroy()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            isInitialized = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
} 