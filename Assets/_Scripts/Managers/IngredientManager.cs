using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientManager : Singleton<IngredientManager>
{
    private Dictionary<EIngredient, SIngredientData> _ingredients;

    protected override void Awake()
    {
        base.Awake();
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
                    ID = int.Parse(csv.GetField("ID")),
                    Name = (EIngredient)Enum.Parse(typeof(EIngredient), csv.GetField("name")),
                    GenLevel = int.Parse(csv.GetField("genLevel")),
                    AvgVeinSize = int.Parse(csv.GetField("avgVeinSize")),
                    Description = csv.GetField("description"),
                    PrefabPath = csv.GetField("prefab")
                };

                _ingredients.Add(data.Name, data);
            }
        }
        
        // TODO save prefabs?
    }

    public SIngredientData GetIngredientData(EIngredient ingredient)
    {
        return _ingredients[ingredient];
    }
}
