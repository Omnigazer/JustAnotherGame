using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Utility;

namespace Omniplatformer.Objects.Items
{
    public class WoodenStick : Item
    {
        readonly string texture = "Textures/wooden_stick";

        public WoodenStick()
        {
            MaxCount = 8;
        }

        public static WoodenStick Create(int count = 1)
        {
            var item = new WoodenStick() { Count = count };
            item.InitializeComponents();
            return item;
        }

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new RenderComponent(Color.White, texture));
        }
    }
}
