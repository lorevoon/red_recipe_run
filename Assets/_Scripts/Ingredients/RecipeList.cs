using UnityEngine;
using System.Collections.Generic;

public static class RecipeList
{
    public struct Recipe
    {
        public int RecipeNumber;
        public string RecipeName;
        public List<EIngredient> Ingredients;
        public string Description;
    }

    public static readonly List<Recipe> AllRecipes = new List<Recipe>
    {

        new Recipe {
            RecipeNumber = 0,
            RecipeName = "Tropical Fruit Jam",
            Ingredients = new List<EIngredient> { EIngredient.Pineapple, EIngredient.Strawberry,
                EIngredient.Cherry, EIngredient.DragonFruit },
            Description = "A vibrant jam bursting with tropical flavors, perfect for spreading on toast."
        },

        new Recipe {
            RecipeNumber = 1,
            RecipeName = "Honey Glazed Melon",
            Ingredients = new List<EIngredient> { EIngredient.Honey, EIngredient.MelonHoneydew,
                EIngredient.Lemon },
            Description = "Refreshing melon slices drizzled with sweet honey and a hint of lemon."
        },

        new Recipe {
            RecipeNumber = 2,
            RecipeName = "Stuffed Peppers",
            Ingredients = new List<EIngredient> { EIngredient.PepperRed, EIngredient.PepperGreen,
                EIngredient.Onion, EIngredient.Tomato },
            Description = "Colorful peppers stuffed with a savory mix of vegetables, baked to perfection."
        },

        // Fruit Medley Tart
        new Recipe {
            RecipeNumber = 3,
            RecipeName = "Fruit Medley Tart",
            Ingredients = new List<EIngredient> { EIngredient.Apple, EIngredient.Peach,
                EIngredient.Strawberry, EIngredient.Jam },
            Description = "A delightful tart filled with seasonal fruits and topped with a layer of sweet jam."
        },

        new Recipe {
            RecipeNumber = 4,
            RecipeName = "Eggplant Parmesan",
            Ingredients = new List<EIngredient> { EIngredient.Eggplant, EIngredient.Tomato, EIngredient.Onion },
            Description = "A hearty dish with layers of breaded eggplant, tomato sauce, and cheese."
        },

        new Recipe {
            RecipeNumber = 5,
            RecipeName = "Avocado Toast",
            Ingredients = new List<EIngredient> { EIngredient.Bread, EIngredient.Avocado, EIngredient.Lemon, EIngredient.PepperGreen },
            Description = "A trendy and nutritious snack featuring creamy avocado on toasted bread."
        },

        new Recipe {
            RecipeNumber = 6,
            RecipeName = "Cherry Lemonade",
            Ingredients = new List<EIngredient> { EIngredient.Cherry, EIngredient.Lemon },
            Description = "A refreshing drink combining tart cherries and zesty lemon."
        },

        new Recipe {
            RecipeNumber = 7,
            RecipeName = "Jerky Snack Mix",
            Ingredients = new List<EIngredient> { EIngredient.Jerky, EIngredient.Bug, EIngredient.PepperRed },
            Description = "A protein-packed snack mix with savory jerky and a spicy kick from roasted bugs."
        },

        new Recipe {
            RecipeNumber = 8,
            RecipeName = "Fruit Smoothie",
            Ingredients = new List<EIngredient> { EIngredient.Strawberry, EIngredient.MelonHoneydew, EIngredient.Honey },
            Description = "A creamy smoothie that blends fresh fruits with a touch of honey for sweetness."
        },

        new Recipe {
            RecipeNumber = 9,
            RecipeName = "Tomato Basil Soup",
            Ingredients = new List<EIngredient> { EIngredient.Tomato, EIngredient.Onion, EIngredient.PepperGreen },
            Description = "A comforting soup made from ripe tomatoes and fresh vegetables."
        },

        new Recipe {
            RecipeNumber = 10,
            RecipeName = "Peach Muffins",
            Ingredients = new List<EIngredient> { EIngredient.Peach, EIngredient.Bread, EIngredient.Honey },
            Description = "Moist muffins infused with juicy peaches, perfect for breakfast or a snack."
        },

        new Recipe {
            RecipeNumber = 11,
            RecipeName = "Pineapple Upside-Down Cake",
            Ingredients = new List<EIngredient> { EIngredient.Pineapple, EIngredient.Cherry, EIngredient.Bread, EIngredient.Honey },
            Description = "A sweet cake topped with caramelized pineapple and cherries."
        },

        new Recipe {
            RecipeNumber = 12,
            RecipeName = "Vegetable Stir-Fry",
            Ingredients = new List<EIngredient> { EIngredient.Eggplant, EIngredient.PepperRed, EIngredient.PepperGreen, EIngredient.Onion },
            Description = "A colorful stir-fry featuring fresh vegetables, sautéed to perfection."
        },

        new Recipe {
            RecipeNumber = 13,
            RecipeName = "Peach and Honey Smoothie",
            Ingredients = new List<EIngredient> { EIngredient.Peach, EIngredient.Honey, EIngredient.MelonHoneydew, EIngredient.Lemon },
            Description = "A refreshing smoothie with a sweet peach and honey flavor."
        },

        new Recipe {
            RecipeNumber = 14,
            RecipeName = "Spicy Tomato Salsa",
            Ingredients = new List<EIngredient> { EIngredient.Tomato, EIngredient.Onion, EIngredient.PepperRed, EIngredient.PepperGreen },
            Description = "A zesty salsa perfect for dipping or as a topping."
        },

        new Recipe {
            RecipeNumber = 15,
            RecipeName = "Avocado Salad",
            Ingredients = new List<EIngredient> { EIngredient.Avocado, EIngredient.Tomato, EIngredient.Onion, EIngredient.Lemon },
            Description = "A fresh salad combining creamy avocado with tangy tomatoes and onions."
        },

        new Recipe {
            RecipeNumber = 16,
            RecipeName = "Melon Salad",
            Ingredients = new List<EIngredient> { EIngredient.MelonHoneydew, EIngredient.Cherry, EIngredient.Lemon, EIngredient.Honey },
            Description = "A refreshing fruit salad drizzled with honey and lemon."
        },

        new Recipe {
            RecipeNumber = 17,
            RecipeName = "Eggplant Dip",
            Ingredients = new List<EIngredient> { EIngredient.Eggplant, EIngredient.Onion, EIngredient.Tomato },
            Description = "A savory dip made from roasted eggplant and vegetables."
        },

        new Recipe {
            RecipeNumber = 18,
            RecipeName = "Honey Fruit Bowl",
            Ingredients = new List<EIngredient> { EIngredient.Strawberry, EIngredient.MelonHoneydew, EIngredient.Cherry, EIngredient.Honey },
            Description = "A colorful bowl of fresh fruits drizzled with honey."
        },

        new Recipe {
            RecipeNumber = 19,
            RecipeName = "Pepper and Eggplant Bake",
            Ingredients = new List<EIngredient> { EIngredient.Eggplant, EIngredient.PepperRed, EIngredient.PepperGreen, EIngredient.Tomato },
            Description = "A baked dish featuring layers of eggplant and peppers in tomato sauce."
        },

         new Recipe {
            RecipeNumber = 20,
            RecipeName = "Dragon Fruit Salad",
            Ingredients = new List<EIngredient> { EIngredient.DragonFruit, EIngredient.MelonHoneydew, EIngredient.Lemon, EIngredient.Honey },
            Description = "A vibrant salad featuring refreshing dragon fruit and melon."
         }

    };
}
