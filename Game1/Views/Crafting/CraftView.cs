using System;
using Omniplatformer.Enums;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Services;
using Omniplatformer.Objects.Items;
using System.Collections.Generic;
using Omniplatformer.Views.BasicControls;
using Omniplatformer.Content;
using Microsoft.Xna.Framework;
using Omniplatformer.Views.InventoryNS;
using Omniplatformer.Objects.Items.Recipes;

namespace Omniplatformer.Views.Character
{
    /// <summary>
    /// Handles the inventory view logic
    /// </summary>
    public class CraftView : ViewControl
    {
        // List<CraftingRecipe> Recipes;
        Player Player => GameService.Player;

        ViewControl header;
        List<ViewControl> containers = new List<ViewControl>();

        public CraftView()
        {
        }

        public override void SetupNode()
        {
            var item2 = WoodenClub.Create(2);
            item2.Count = 2;
            var Recipes = new CraftingRecipeGroup("Weapons")
            {
                new CraftingRecipe(WoodenClub.Create(4), WoodenStick.Create(1))
            };
            var Recipes2 = new CraftingRecipeGroup("Not weapons")
            {
                new CraftingRecipe(WoodenClub.Create(4), Redberry.Create(1))
            };

            header = new Row();

            Width = 700;
            Node.Height = 360;
            Node.Overflow = Facebook.Yoga.YogaOverflow.Scroll;
            RegisterChild(header);
            RegisterRecipeGroup(Recipes);
            RegisterRecipeGroup(Recipes2);
        }

        public void RegisterRecipeGroup(CraftingRecipeGroup recipe_group)
        {
            var tab = new Column() { Node = { MinWidth = 50 }, BorderThickness = 2 };
            tab.RegisterChild(new Label(recipe_group.Name));
            var container = new Column();
            foreach (var recipe in recipe_group)
            {
                var row = new Row()
                {
                    Node =
                    {
                        Width = 500,
                        Padding = 5
                    }
                };
                var slot_view = new SlotView(new Objects.InventoryNS.ItemSlot() { Item = recipe.Result }) { Node = { MarginRight = 40 } };
                slot_view.MouseClick += (sender, e) => { CraftItem(recipe); };
                row.RegisterChild(slot_view);
                foreach (var item in recipe.Ingredients)
                {
                    row.RegisterChild(new SlotView(new Objects.InventoryNS.ItemSlot() { Item = item }));
                }
                container.RegisterChild(row);
            }
            tab.MouseClick += (sender, e) =>
            {
                foreach (var control in containers)
                {
                    control.Visible = false;
                }
                container.Visible = true;
            };

            header.RegisterChild(tab);
            RegisterChild(container);
            containers.Add(container);

            foreach (var control in containers)
            {
                control.Visible = false;
            }
            if (containers.Count > 0)
                containers[0].Visible = true;
        }

        public override void DrawSelf()
        {
            var spriteBatch = GraphicsService.Instance;
            Rectangle outer_rect = GlobalRect;
            spriteBatch.Draw(GameContent.Instance.whitePixel, outer_rect, Color.Gray * 0.5f);
        }

        private bool HasIngredients(Objects.InventoryNS.Inventory inventory, CraftingRecipe recipe)
        {
            // Iterate through all of the ingredients in the specified recipe in the parameter.
            foreach (Item ingredient in recipe.Ingredients)
            {
                // If the assigned value is false, the method will return false.
                if (!inventory.HasItem(ingredient.ItemId, ingredient.Count))
                    return false;
            }
            return true;
        }

        private void CraftItem(CraftingRecipe recipe)
        {
            var empty_slot = Player.Inventory.GetEmptySlot();
            if (HasIngredients(Player.Inventory, recipe) && empty_slot != null)
            {
                foreach (Item ingredient in recipe.Ingredients)
                {
                    // Find a slot containing the ingredient and take the item from it
                    Player.Inventory.GetSlotForItem(ingredient.ItemId, ingredient.Count).DrainItem(ingredient.Count);
                }
                empty_slot = Player.Inventory.GetEmptySlot();
                // empty_slot.Item = recipe.Result.Copy();
                empty_slot.Item = (Item)recipe.Result.Clone();
            }
        }
    }
}
