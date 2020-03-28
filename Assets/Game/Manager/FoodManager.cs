using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
public class FoodManager : MonoBehaviour
{
    public Dictionary<int, FoodObject> foodObjects;
    public Dictionary<int, FoodRecipeObject> recipeObjects;

    public Dictionary<int, FoodObject> ingredientObjects;

    List<FoodObject> ingredientObjectList;
    List<FoodRecipeObject> recipeObjectList;

    void Start()
    {
        foodObjects = GameManager.instance.databaseManager.foodObjects;
        recipeObjects = GameManager.instance.databaseManager.recipeObjects;

        //add ingredients
        ingredientObjects = new Dictionary<int, FoodObject>();

        foreach (var food in foodObjects.Values)
        {
            if (!recipeObjects.Values.Any(r => r.dish.id == food.id))
            {
                ingredientObjects.Add(food.id, food);
            }
            else
            {
                food.isDish = true;
            }
        }

        ingredientObjectList = ingredientObjects.Values.ToList();
        recipeObjectList = recipeObjects.Values.ToList();
    }

    public FoodObject GenerateRandomFood()
    {
        var index = UnityEngine.Random.Range(0, ingredientObjectList.Count);

        return ingredientObjectList[index];
    }
}

