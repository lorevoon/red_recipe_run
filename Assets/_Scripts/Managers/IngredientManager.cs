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
                    GenLevel = int.Parse(csv.GetField("genLevel")),
                    AvgVeinSize = int.Parse(csv.GetField("avgVeinSize")),
                    Description = csv.GetField("description"),
                    PrefabPath = csv.GetField("prefabPath")
                };

                _ingredients.Add(data.Name, data);
            }
        }

        _ingredientPrefabs = new GameObject[_ingredients.Count];
        foreach (var data in _ingredients)
        {
            _ingredientPrefabs[data.Value.ID] = 
                Resources.Load("Prefabs/Ingredients/" + data.Value.PrefabPath).GameObject();
        }
    }

    public SIngredientData GetIngredientData(EIngredient ingredient)
    {
        return _ingredients[ingredient];
    }
    
    public void SpawnIngredients(Vector3Int gridPosition)
    {
        Instantiate(_ingredientPrefabs[_ingredients[EIngredient.Apple].ID],
            new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0), Quaternion.identity);
    }
}
