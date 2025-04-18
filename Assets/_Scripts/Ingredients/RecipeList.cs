using UnityEngine;
using System.Collections.Generic;

public static class RecipeList
{
    public static readonly List<SRecipe> AllRecipes = new List<SRecipe>
    {

        new SRecipe {
            RecipeNumber = 0,
            RecipeName = "Tropical Fruit Jam",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Pineapple, 0}, {EIngredient.Strawberry, 0},
                {EIngredient.Cherry, 0}, {EIngredient.DragonFruit, 0} },
            Description = "A vibrant jam bursting with tropical flavors, perfect for spreading on toast."
        },

        new SRecipe {
            RecipeNumber = 1,
            RecipeName = "Honey Glazed Melon",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Honey, 0}, {EIngredient.MelonHoneydew, 0},
                {EIngredient.Lemon, 0} },
            Description = "Refreshing melon slices drizzled with sweet honey and a hint of lemon."
        },

        new SRecipe {
            RecipeNumber = 2,
            RecipeName = "Stuffed Peppers",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.PepperRed, 0}, {EIngredient.PepperGreen, 0},
                {EIngredient.Onion, 0}, {EIngredient.Tomato, 0} },
            Description = "Colorful peppers stuffed with a savory mix of vegetables, baked to perfection."
        },

        // Fruit Medley Tart
        new SRecipe {
            RecipeNumber = 3,
            RecipeName = "Fruit Medley Tart",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Apple, 0}, {EIngredient.Peach, 0},
                {EIngredient.Strawberry, 0}, {EIngredient.Jam, 0} },
            Description = "A delightful tart filled with seasonal fruits and topped with a layer of sweet jam."
        },

        new SRecipe {
            RecipeNumber = 4,
            RecipeName = "Eggplant Parmesan",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Eggplant, 0}, 
                {EIngredient.Tomato, 0}, {EIngredient.Onion, 0} },
            Description = "A hearty dish with layers of breaded eggplant, tomato sauce, and cheese."
        },

        new SRecipe {
            RecipeNumber = 5,
            RecipeName = "Avocado Toast",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Bread, 0}, {EIngredient.Avocado, 0}, 
                {EIngredient.Lemon, 0}, {EIngredient.PepperGreen, 0} },
            Description = "A trendy and nutritious snack featuring creamy avocado on toasted bread."
        },

        new SRecipe {
            RecipeNumber = 6,
            RecipeName = "Cherry Lemonade",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Cherry, 0}, {EIngredient.Lemon, 0} },
            Description = "A refreshing drink combining tart cherries and zesty lemon."
        },

        new SRecipe {
            RecipeNumber = 7,
            RecipeName = "Jerky Snack Mix",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Jerky, 0}, {EIngredient.Bug, 0}, 
                {EIngredient.PepperRed, 0} },
            Description = "A protein-packed snack mix with savory jerky and a spicy kick from roasted bugs."
        },

        new SRecipe {
            RecipeNumber = 8,
            RecipeName = "Fruit Smoothie",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Strawberry, 0}, 
                {EIngredient.MelonHoneydew, 0}, {EIngredient.Honey, 0} },
            Description = "A creamy smoothie that blends fresh fruits with a touch of honey for sweetness."
        },

        new SRecipe {
            RecipeNumber = 9,
            RecipeName = "Tomato Basil Soup",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Tomato, 0}, {EIngredient.Onion, 0}, 
                {EIngredient.PepperGreen, 0} },
            Description = "A comforting soup made from ripe tomatoes and fresh vegetables."
        },

        new SRecipe {
            RecipeNumber = 10,
            RecipeName = "Peach Muffins",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Peach, 0}, {EIngredient.Bread, 0}, 
                {EIngredient.Honey, 0} },
            Description = "Moist muffins infused with juicy peaches, perfect for breakfast or a snack."
        },

        new SRecipe {
            RecipeNumber = 11,
            RecipeName = "Pineapple Upside-Down Cake",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Pineapple, 0}, {EIngredient.Cherry, 0}, 
                {EIngredient.Bread, 0}, {EIngredient.Honey, 0} },
            Description = "A sweet cake topped with caramelized pineapple and cherries."
        },

        new SRecipe {
            RecipeNumber = 12,
            RecipeName = "Vegetable Stir-Fry",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Eggplant, 0}, {EIngredient.PepperRed, 0}, 
                {EIngredient.PepperGreen, 0}, {EIngredient.Onion, 0} },
            Description = "A colorful stir-fry featuring fresh vegetables, sautéed to perfection."
        },

        new SRecipe {
            RecipeNumber = 13,
            RecipeName = "Peach and Honey Smoothie",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Peach, 0}, {EIngredient.Honey, 0}, 
                {EIngredient.MelonHoneydew, 0}, {EIngredient.Lemon, 0} },
            Description = "A refreshing smoothie with a sweet peach and honey flavor."
        },

        new SRecipe {
            RecipeNumber = 14,
            RecipeName = "Spicy Tomato Salsa",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Tomato, 0}, {EIngredient.Onion, 0}, 
                {EIngredient.PepperRed, 0}, {EIngredient.PepperGreen, 0} },
            Description = "A zesty salsa perfect for dipping or as a topping."
        },

        new SRecipe {
            RecipeNumber = 15,
            RecipeName = "Avocado Salad",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Avocado, 0}, {EIngredient.Tomato, 0}, 
                {EIngredient.Onion, 0}, {EIngredient.Lemon, 0} },
            Description = "A fresh salad combining creamy avocado with tangy tomatoes and onions."
        },

        new SRecipe {
            RecipeNumber = 16,
            RecipeName = "Melon Salad",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.MelonHoneydew, 0}, {EIngredient.Cherry, 0}, 
                {EIngredient.Lemon, 0}, {EIngredient.Honey, 0} },
            Description = "A refreshing fruit salad drizzled with honey and lemon."
        },

        new SRecipe {
            RecipeNumber = 17,
            RecipeName = "Eggplant Dip",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Eggplant, 0}, {EIngredient.Onion, 0}, 
                {EIngredient.Tomato, 0} },
            Description = "A savory dip made from roasted eggplant and vegetables."
        },

        new SRecipe {
            RecipeNumber = 18,
            RecipeName = "Honey Fruit Bowl",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Strawberry, 0}, 
                {EIngredient.MelonHoneydew, 0}, {EIngredient.Cherry, 0}, {EIngredient.Honey, 0} },
            Description = "A colorful bowl of fresh fruits drizzled with honey."
        },

        new SRecipe {
            RecipeNumber = 19,
            RecipeName = "Pepper and Eggplant Bake",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.Eggplant, 0}, {EIngredient.PepperRed, 0}, 
                {EIngredient.PepperGreen, 0}, {EIngredient.Tomato, 0} },
            Description = "A baked dish featuring layers of eggplant and peppers in tomato sauce."
        },

         new SRecipe {
            RecipeNumber = 20,
            RecipeName = "Dragon Fruit Salad",
            Ingredients = new Dictionary<EIngredient, int> { {EIngredient.DragonFruit, 0}, 
                {EIngredient.MelonHoneydew, 0}, {EIngredient.Lemon, 0}, {EIngredient.Honey, 0} },
            Description = "A vibrant salad featuring refreshing dragon fruit and melon."
         }

    };
}
