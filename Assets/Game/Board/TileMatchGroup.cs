using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TileMatchGroup
{
    public FoodRecipeObject foodRecipe;
    public FoodObject foodObject;

    public bool isDish;

    public int points;

    public int rank;

    public TileMatchGroup(FoodObject foodObject)
    {
        this.foodObject = foodObject;
        points = foodObject.energy;
    }

    public TileMatchGroup(FoodRecipeObject foodRecipe)
    {
        this.foodRecipe = foodRecipe;
        isDish = true;

        points = foodRecipe.dish.energy;
    }
}
