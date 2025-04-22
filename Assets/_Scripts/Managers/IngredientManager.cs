using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientManager : Singleton<IngredientManager>
{
    private MapManager _mapManager;
    private Dictionary<EIngredient, SIngredientData> _ingredients;
    // [NamedArray(typeof(EIngredient))] private GameObject[] _ingredientPrefabs;
    private GameObject[] _ingredientPrefabs;

    protected override void Awake()
    {
        base.Awake();
        _mapManager = MapManager.Instance;
        _ingredients = new Dictionary<EIngredient, SIngredientData>();
        Init(Path.Combine(Application.streamingAssetsPath, "Tables/IngredientsTable.csv"));
    }
    
    private void Init(string dataPath)
    {
        Debug.Assert(dataPath != null && File.Exists(dataPath));

        using (var reader = new StreamReader(dataPath))
        using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                SIngredientData data = new SIngredientData
                {
                    Name = (EIngredient)Enum.Parse(typeof(EIngredient), csv.GetField("name")),
                    ID = int.Parse(csv.GetField("ID")),
                    // GenLevel = int.Parse(csv.GetField("genLevel")),
                    // AvgVeinSize = int.Parse(csv.GetField("avgVeinSize")),
                    // Description = csv.GetField("description"),
                    PrefabPath = csv.GetField("prefabPath")
                };

                _ingredients.Add(data.Name, data);
            }
        }

        _ingredientPrefabs = new GameObject[_ingredients.Count];
        foreach (var data in _ingredients)
        {
            string prefabPath = "Prefabs/Ingredients/" + data.Value.PrefabPath.Trim();
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"Failed to load ingredient prefab for {data.Key} at path: {prefabPath}");
                continue;
            }
            
            _ingredientPrefabs[data.Value.ID] = prefab;
            
            // Verify the loaded prefab has the correct ingredient type
            Ingredient ingredientComponent = prefab.GetComponent<Ingredient>();
            if (ingredientComponent != null)
            {
                if (ingredientComponent.IngredientType != data.Key)
                {
                    Debug.LogWarning($"Ingredient prefab mismatch! Prefab {prefabPath} has type {ingredientComponent.IngredientType} but should be {data.Key}. Fixing...");
                    ingredientComponent.IngredientType = data.Key;
                }
            }
            else
            {
                Debug.LogError($"Ingredient component missing on prefab {prefabPath}");
            }
        }
        
        Debug.Log($"Loaded {_ingredientPrefabs.Length} ingredient prefabs");
    }

    public SIngredientData GetIngredientData(EIngredient ingredient)
    {
        return _ingredients[ingredient];
    }
    
    public void SpawnIngredients(Vector3Int gridPosition, EIngredient ingredient)
    {
        Debug.Log($"Spawning ingredient: {ingredient} at position {gridPosition}");
        
        try
        {
            if (!_ingredients.ContainsKey(ingredient))
            {
                Debug.LogError($"Ingredient {ingredient} not found in _ingredients dictionary!");
                return;
            }
            
            int id = _ingredients[ingredient].ID;
            if (id < 0 || id >= _ingredientPrefabs.Length || _ingredientPrefabs[id] == null)
            {
                Debug.LogError($"Invalid ingredient ID {id} for {ingredient} or prefab is null!");
                return;
            }
            
            GameObject prefab = _ingredientPrefabs[id];
            GameObject newIngredient = Instantiate(prefab, new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0), Quaternion.identity);
            
            // Verify the spawned ingredient has the right type
            Ingredient ingredientComponent = newIngredient.GetComponent<Ingredient>();
            if (ingredientComponent != null)
            {
                if (ingredientComponent.IngredientType != ingredient)
                {
                    Debug.LogWarning($"Fixing ingredient type on spawned object from {ingredientComponent.IngredientType} to {ingredient}");
                    ingredientComponent.IngredientType = ingredient;
                }
            }
            else
            {
                Debug.LogError($"Spawned ingredient has no Ingredient component!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception when spawning {ingredient}: {e.Message}\n{e.StackTrace}");
        }
    }
}
