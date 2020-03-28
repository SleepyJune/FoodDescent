using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class TileMatchChecker
{
    FoodManager foodManager;

    public TileMatchChecker()
    {
        foodManager = GameManager.instance.foodManager;
    }

    public TileMatchGroup CheckPath(Path path)
    {
        List<TileMatchGroup> possibleRecipes = new List<TileMatchGroup>();

        foreach(var recipe in foodManager.recipeObjects.Values)
        {
            if(CheckRecipe(recipe, path))
            {
                TileMatchGroup matchGroup = new TileMatchGroup(recipe);

                possibleRecipes.Add(matchGroup);
            }
        }

        foreach(var ingredient in foodManager.ingredientObjects.Values)
        {
            int num = CheckIngredient(ingredient, path);

            if (num >= 3)
            {
                TileMatchGroup matchGroup = new TileMatchGroup(ingredient);
                
                possibleRecipes.Add(matchGroup);
            }
        }

        return possibleRecipes.OrderByDescending(r => r.points).FirstOrDefault();
    }

    public int CheckIngredient(FoodObject ingredient, Path path)
    {
        int sum = 0;

        foreach (var point in path.waypoints)
        {
            var food = point.slot.food;

            if(food == ingredient)
            {
                sum += 1;
            }
        }

        return sum;
    }

    public bool CheckRecipe(FoodRecipeObject recipe, Path path)
    {
        HashSet<int> ingredients = new HashSet<int>();

        foreach(var point in path.waypoints)
        {
            var food = point.slot.food;

            ingredients.Add(food.id);
        }

        foreach(var ingredient in recipe.ingredients)
        {
            if (!ingredients.Contains(ingredient.id))
            {
                return false;
            }
        }

        return true;
    }
}
