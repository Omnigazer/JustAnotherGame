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
            collectible.InitializeComponents();
            return collectible;
        }

        public override void InitializeCustomComponents()
        {
            RegisterComponent(new PhysicsComponent(Vector2.Zero, Vector2.Zero) { Pickupable = true });
            RegisterComponent(new RenderComponent(Color.Green));
        }
        public Bonus Bonus { get; set; }
    }
}
