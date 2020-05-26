using System;
using Microsoft.Xna.Framework;
using Omniplatformer.Objects;

namespace Omniplatformer.Components.Physics
{

    public class PlatformMoveComponent : DynamicPhysicsComponent
    {
        // internal position offset counter
        float position = 0;
        int direction = 1;
        float speed = 5;
        int horizontal_amp = 250;

        public override void ProcessMovement(float dt)
        {
            CurrentMovement = new Vector2(speed * direction, 0);
            position += speed * direction * dt;
            if (Math.Abs(position) >= horizontal_amp)
            {
                direction *= -1;
            }
        }
    }
}
