using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Utility.DataStructs;

namespace Omniplatformer.Objects.Interactibles
{
    public class Collectible : GameObject
    {
        public Collectible() { }

        public static Collectible Create()
        {
            var collectible = new Collectible();
            collectible.InitComponents();
            return collectible;
        }

        public void InitComponents()
        {
            Components.Add(new PhysicsComponent(this, Vector2.Zero, Vector2.Zero) { Pickupable = true });
            Components.Add(new RenderComponent(this, Color.Green));
        }
        public Bonus Bonus { get; set; }
    }
}
