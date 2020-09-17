using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Objects.Items.Recipes
{
    public class CraftingRecipeGroup : IEnumerable<CraftingRecipe>
    {
        public string Name { get; set; }
        public List<CraftingRecipe> Recipes { get; set; }

        public CraftingRecipeGroup(string name)
        {
            Name = name;
            Recipes = new List<CraftingRecipe>();
        }

        public CraftingRecipeGroup(string name, IEnumerable<CraftingRecipe> recipes)
        {
            Name = name;
            Recipes = recipes.ToList();
        }

        public void Add(CraftingRecipe recipe)
        {
            Recipes.Add(recipe);
        }

        public IEnumerator<CraftingRecipe> GetEnumerator()
        {
            return ((IEnumerable<CraftingRecipe>)Recipes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Recipes).GetEnumerator();
        }
    }
}
