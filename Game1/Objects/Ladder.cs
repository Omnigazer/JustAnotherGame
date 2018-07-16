using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components;
using Omniplatformer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    class Ladder : GameObject
    {
        public Ladder(Vector2 center, Vector2 halfsize)// : this(center, halfsize, null)
        {
            Solid = false; Climbable = true;
            Components.Add(new PositionComponent(this, center, halfsize));
            // TODO: Add Purple Color to this renderer
            Components.Add(new RenderComponent(this, Color.Purple, GameContent.Instance.ladder, 0, true));
        }

        /*
        public Ladder(Vector2 center, Vector2 halfsize, GameObject parent)
        {
            Solid = false; Climbable = true;
            Components.Add(new PositionComponent(this, center, halfsize) { parent_pos = (parent != null) ? (PositionComponent)parent : null });
            // TODO: Add Purple Color to this renderer
            Components.Add(new RenderComponent(this, Color.Purple, GameContent.Instance.ladder, 0, true));
        }
        */

        public override object AsJson()
        {
            return new
            {
                Id,
                type = GetType().AssemblyQualifiedName,
                Position = PositionJson.ToJson(this)
            };
        }

        public static GameObject FromJson(Deserializer deserializer)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(deserializer.getData());
            return new Ladder(coords, halfsize);
        }
    }
}
