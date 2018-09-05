using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{

    public class PlatformMoveComponent : DynamicPhysicsComponent
    {
        public PlatformMoveComponent(GameObject obj, Vector2 coords, Vector2 halfsize) : base(obj, coords, halfsize)
        {
        }

        // internal position offset counter
        int position = 0;
        int direction = 1;
        int speed = 5;
        int horizontal_amp = 250;

        public override void ProcessMovement()
        {
            CurrentMovement = new Vector2(speed * direction, 0);
            position += speed * direction;
            if (Math.Abs(position) >= horizontal_amp)
            {
                direction *= -1;
            }
        }
    }
}
