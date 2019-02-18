using Microsoft.Xna.Framework;
using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    class BallisticsHelper
    {
        public static Vector2? GetThrowVector(float module, float distance, float height = 0)
        {
            int dir = Math.Sign(distance);
            distance = Math.Abs(distance);
            distance += 50;
            // float module = 20;
            // var distance = (player_pos.WorldPosition.Coords - pos.WorldPosition.Coords).X;
            // old formula
            // float angle = (float)Math.Asin(distance / (module * module)) / 2;
            float g = PhysicsSystem.G;
            float angle = (float)Math.Atan2(module * module - Math.Sqrt(Math.Pow(module, 4) - g * (g * distance * distance + 2 * height * module * module)), g * distance);


            if (!float.IsNaN(angle))
            {
                var direction = new Vector2((float)Math.Cos(angle) * dir, (float)Math.Sin(angle));
                direction.Normalize();
                direction *= module;
                return direction;
            }

            return null;
        }
    }
}
