using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omniplatformer
{
    public class Collectible : GameObject
    {
        public Collectible(Vector2 center, Vector2 halfsize)
        {
            Components.Add(new PhysicsComponent(this, center, halfsize) { Pickupable = true });
            // TODO: Add Aqua Color to this renderer
            Components.Add(new RenderComponent(this, Color.Green));
        }
        public Bonus Bonus { get; set; }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new Collectible(coords, halfsize);
        }
    }
}
