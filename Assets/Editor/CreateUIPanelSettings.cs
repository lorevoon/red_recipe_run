#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.IO;

public class CreateUIPanelSettings : MonoBehaviour
{
    [MenuItem("Tools/Create UI Panel Settings")]
    public static void CreatePanelSettings()
    {
        string resourcesFolder = "Assets/Resources/UI";
        string assetPath = resourcesFolder + "/GameUIPanelSettings.asset";

        // Check if directory exists, create if not
        if (!Directory.Exists(resourcesFolder))
        {
            Directory.CreateDirectory(resourcesFolder);
        }

        // Check if asset already exists
        if (File.Exists(assetPath))
        {
            Debug.Log("PanelSettings asset already exists at " + assetPath);
            return;
        }

        // Create PanelSettings asset
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        
        // Configure settings
        panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        panelSettings.referenceResolution = new Vector2Int(1920, 1080);
        panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
        panelSettings.match = 0.5f;
        panelSettings.sortingOrder = 1;
        
        // Save the asset
        AssetDatabase.CreateAsset(panelSettings, assetPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created PanelSettings at " + assetPath);
        
        // Select the asset in the project window
        Selection.activeObject = panelSettings;
    }
}
#endif 