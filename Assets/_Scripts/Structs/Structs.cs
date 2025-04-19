using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public struct SIngredientData
{
    public EIngredient Name;
    public int ID;
    public int GenLevel;
    public int AvgVeinSize;
    public string Description;
    public string PrefabPath;
}

public struct SBushLevelData
{
    public EBushLevel Level;
    public float MaxDurability;
    public float Percentage;
    public float GapPercentage;
    public string ColorHex;
}

public struct SRecipe
{
    public int RecipeNumber;
    public string RecipeName;
    public Dictionary<EIngredient, int> Ingredients;
    public string Description;
}