using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

[CreateAssetMenu(menuName = "Game/Recipe Object")]
public class FoodRecipeObject: GameDataObject
{
    public FoodObject dish;

    public FoodObject[] ingredients;

    public HashSet<FoodObject> ingredientsHash;

    public void Initialize()
    {
        ingredientsHash = new HashSet<FoodObject>();

        foreach (var item in ingredients)
        {
            ingredientsHash.Add(item);
        }
    }
}
