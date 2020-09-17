using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Objects.Items.Recipes
{
    public class CraftingRecipe
    {
        public Item Result { get; set; }
        public List<Item> Ingredients { get; set; }

        public CraftingRecipe(Item result, IEnumerable<Item> ingredients)
        {
            Result = result;
            Ingredients = ingredients.ToList();
        }

        public CraftingRecipe(Item result, params Item[] ingredients) : this(result, ingredients.AsEnumerable())
        {
        }
    }
}
