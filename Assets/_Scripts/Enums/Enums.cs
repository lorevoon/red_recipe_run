using System;
using UnityEngine;

// public class EIngredient
// {
//     Cookie, Brownie, Stein, Moonshine, Whiskey, Tart, Sushi, Sashimi, Saki, Boar, Marmalade, Jam, Apple, AppleWorm, Turnip, Potato, Eggs, Honeycomb, Pineapple, Bacon, Beer, Steak, Wine, Fish, Cheese, Chicken, Bread, Eggplant, PepperRed, PepperGreen, Grubs, Grub, Tomato, Strawberry, Peach, Lemon, PiePumpkin, PieLemon, PieApple, Pickle, Pretzel, Pepperoni, FishFillet, Honey, Jerky, PotatoRed, MelonHoneydew, MelonCantaloupe, MelonWater, Waffles, ChickenLeg, Cherry, Ribs, Sardines, DragonFruit, Sausages, Avocado, FishSteak, Bug, Olive, PickledEggs, Roll, Onion
//     public enum EIngredientType
//     {
//         Marmalade,
//         Jam,
//         Apple, 
//         Pineapple,
//         Bread,
//         Eggplant, 
//         PepperRed,
//         PepperGreen,
//         Tomato,
//         Strawberry,
//         Peach, 
//         Lemon,
//         Honey,
//         Jerky,
//         MelonHoneydew,
//         Cherry,
//         DragonFruit,
//         Avocado,
//         Bug,
//         Onion
//     }
//     //Based on the ingredient name, return a tuple with the ingredient colour, name, and recipe number(s)
//     /*
//     0 -Tropical Fruit Jam: Ingredients: Pineapple, Strawberry, Cherry, Dragon Fruit. Description: A vibrant jam bursting with tropical flavors, perfect for spreading on toast.
//     1- Honey Glazed Melon: Ingredients: Honey, Melon Honeydew, Lemon. Description: Refreshing melon slices drizzled with sweet honey and a hint of lemon.
//     2- Stuffed Peppers: Ingredients: Pepper Red, Pepper Green, Onion, Tomato. Description: Colorful peppers stuffed with a savory mix of vegetables, baked to perfection.
//     3- Fruit Medley Tart: Ingredients: Apple, Peach, Strawberry, Jam. Description: A delightful tart filled with seasonal fruits and topped with a layer of sweet jam.
//     4- Eggplant Parmesan: Ingredients: Eggplant, Tomato, Onion, Cheese (if available). Description: A hearty dish with layers of breaded eggplant, tomato sauce, and cheese.
//     5- Avocado Toast: Ingredients: Bread, Avocado, Lemon, Pepper Green. Description: A trendy and nutritious snack featuring creamy avocado on toasted bread.
//     6- Cherry Lemonade: Ingredients: Cherry, Lemon, Water, Sugar (if available). Description: A refreshing drink combining tart cherries and zesty lemon.
//     7- Jerky Snack Mix: Ingredients: Jerky, Bug, Pepper Red. Description: A protein-packed snack mix with savory jerky and a spicy kick from roasted bugs.
//     8- Fruit Smoothie: Ingredients: Banana (if available), Strawberry, Melon Honeydew, Honey. Description: A creamy smoothie that blends fresh fruits with a touch of honey for sweetness.
//     9- Tomato Basil Soup: Ingredients: Tomato, Onion, Pepper Green, Basil (if available). Description: A comforting soup made from ripe tomatoes and fresh vegetables.
//     10- Peach Muffins: Ingredients: Peach, Egg, Bread (if using as flour), Honey. Description: Moist muffins infused with juicy peaches, perfect for breakfast or a snack.
//     11- Pineapple Upside-Down Cake: Ingredients: Pineapple, Cherry, Bread, Honey. Description: A sweet cake topped with caramelized pineapple and cherries.
//     12- Vegetable Stir-Fry: Ingredients: Eggplant, Pepper Red, Pepper Green, Onion. Description: A colorful stir-fry featuring fresh vegetables, sautéed to perfection.
//     13- Peach and Honey Smoothie: Ingredients: Peach, Honey, Melon Honeydew, Lemon. Description: A refreshing smoothie with a sweet peach and honey flavor.
//     14- Spicy Tomato Salsa: Ingredients: Tomato, Onion, Pepper Red, Pepper Green. Description: A zesty salsa perfect for dipping or as a topping.
//     15- Avocado Salad: Ingredients: Avocado, Tomato, Onion, Lemon. Description: A fresh salad combining creamy avocado with tangy tomatoes and onions.
//     16- Melon Salad: Ingredients: Melon Honeydew, Cherry, Lemon, Honey. Description: A refreshing fruit salad drizzled with honey and lemon.
//     17- Eggplant Dip: Ingredients: Eggplant, Onion, Tomato, Garlic (if available). Description: A savory dip made from roasted eggplant and vegetables.
//     18- Honey Fruit Bowl: Ingredients: Strawberry, Melon Honeydew, Cherry, Honey. Description: A colorful bowl of fresh fruits drizzled with honey.
//     19- Pepper and Eggplant Bake: Ingredients: Eggplant, Pepper Red, Pepper Green, Tomato. Description: A baked dish featuring layers of eggplant and peppers in tomato sauce.
//     20- Dragon Fruit Salad: Ingredients: Dragon Fruit, Melon Honeydew, Lemon, Honey. Description: A vibrant salad featuring refreshing dragon fruit and melon.
//     */
//     public static (string, string, int[]) GetIngredientInfo(EIngredientType ingredient)
//     {
//         switch(ingredient){
//             case EIngredientType.Marmalade:
//                 return ("red", "Marmalade", new int[]{0});
//             case EIngredientType.Jam:
//                 return ("red", "Jam", new int[]{0});
//             case EIngredientType.Apple:
//                 return ("red", "Apple", new int[]{3});
//             case EIngredientType.Pineapple:
//                 return ("yellow", "Pineapple", new int[]{0, 11});
//             case EIngredientType.Bread:
//                 return ("brown", "Bread", new int[]{5, 10, 11});
//             case EIngredientType.Eggplant:
//                 return ("purple", "Eggplant", new int[]{4, 12, 17});
//             case EIngredientType.PepperRed:
//                 return ("red", "Pepper Red", new int[]{2, 9, 14, 19});
//             case EIngredientType.PepperGreen:
//                 return ("green", "Pepper Green", new int[]{2, 5, 9, 15, 19});
//             case EIngredientType.Tomato:
//                 return ("red", "Tomato", new int[]{2, 4, 9, 14, 19});
//             case EIngredientType.Strawberry:
//                 return ("red", "Strawberry", new int[]{0, 8, 18});
//             case EIngredientType.Peach:
//                 return ("orange", "Peach", new int[]{0, 10, 13, 18});
//             case EIngredientType.Lemon:
//                 return ("yellow", "Lemon", new int[]{1, 6, 13, 16, 20});
//             case EIngredientType.Honey:
//                 return ("yellow", "Honey", new int[]{1, 8, 13, 18});
//             case EIngredientType.Jerky:
//                 return ("brown", "Jerky", new int[]{7});
//             case EIngredientType.MelonHoneydew:
//                 return ("green", "Melon Honeydew", new int[]{1, 8, 13, 16, 20});
//             case EIngredientType.Cherry:
//                 return ("red", "Cherry", new int[]{0, 6, 16, 18});
//             case EIngredientType.DragonFruit:
//                 return ("pink", "Dragon Fruit", new int[]{0, 8, 20});
//             case EIngredientType.Avocado:
//                 return ("green", "Avocado", new int[]{5, 15});
//             case EIngredientType.Bug:
//                 return ("brown", "Bug", new int[]{7});
//             case EIngredientType.Onion:
//                 return ("white", "Onion", new int[]{2, 4, 12, 15, 19});
//             default:
//                 return ("", "", new int[]{});
// }
    
public enum ECamera
{
    RecipeRun,
    GrandmasHouse,
    MainMenu
}

public enum EGrid
{
    Bush,
    Item,
    Wall,
    Empty,
    Unreachable
}

public enum EIngredient
{
    Marmalade,
    Jam,
    Apple,
    Pineapple,
    Bread,
    Eggplant,
    PepperRed,
    PepperGreen,
    Tomato,
    Strawberry,
    Peach,
    Lemon,
    Honey,
    Jerky,
    MelonHoneydew,
    Cherry,
    DragonFruit,
    Avocado,
    Bug,
    Onion
}

public enum EBushLevel
{
    Level1,
    Level2,
    Level3,
    Level4
}

public enum EWolfStates
{
    Wander,
    Chase,
    Bite
}

public enum EUpgradeType
{
    MovementSpeed,
    InventorySpace,
    MaxHearts
}