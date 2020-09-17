using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Components.Items;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Utility;

namespace Omniplatformer.Objects.Items
{
    public class Redberry : Item
    {
        readonly string texture = "Textures/redberry";

        public Redberry()
        {
            MaxCount = 16;
        }

        public static Redberry Create(int count = 1)
        {
            var item = new Redberry() { Count = count };
            item.InitializeComponents();
            return item;
        }

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new RenderComponent(Color.White, texture));
            RegisterComponent(new HealingConsumableComponent() { HealingValue = 5 });
        }
    }
}
