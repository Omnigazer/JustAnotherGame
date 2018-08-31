using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;

namespace Omniplatformer
{
    public class Liquid : GameObject
    {

        public Liquid(Vector2 coords, Vector2 halfsize) : this(coords, halfsize, Position.DefaultOrigin)
        {

        }

        public Liquid(Vector2 coords, Vector2 halfsize, Vector2 origin)
        {
            Components.Add(new RenderComponent(this, Color.Aqua * 0.5f, Layers.Liquid));
            Components.Add(new PhysicsComponent(this, coords, halfsize, origin) { Liquid = true });
        }

        public override object AsJson()
        {
            return new { type = GetType().AssemblyQualifiedName, Position = PositionJson.ToJson(this) };
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new Liquid(coords, halfsize, origin);
        }
    }
}
