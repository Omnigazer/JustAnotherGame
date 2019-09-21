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
        public static Vector2? GetThrowVector(float force, float inverse_mass, float distance, float height = 0)
        {
            float speed = force * inverse_mass;
            int dir = Math.Sign(distance);
            distance = Math.Abs(distance);
            float g = PhysicsSystem.G;
            float angle = (float)Math.Atan2(speed * speed - Math.Sqrt(Math.Pow(speed, 4) - g * (g * distance * distance + 2 * height * speed * speed)), g * distance);

            if (!float.IsNaN(angle))
            {
                var direction = new Vector2((float)Math.Cos(angle) * dir, (float)Math.Sin(angle));
                direction.Normalize();
                direction *= force;
                return direction;
            }

            return null;
        }
    }
}
