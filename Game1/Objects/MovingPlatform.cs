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
    class MovingPlatform : GameObject
    {
        public MovingPlatform(Vector2 position)
        {
            Components.Add(new PositionComponent(this, position, new Vector2(100, 10)));
            Components.Add(new RenderComponent(this));
            Components.Add(new PlatformMoveComponent(this));
        }

        public override object AsJson()
        {
            return PositionJson.ToJson(this);
        }

        public static GameObject FromJson(JObject data)
        {
            var (coords, halfsize, origin) = PositionJson.FromJson(data);
            return new MovingPlatform(coords);
        }
    }
}
