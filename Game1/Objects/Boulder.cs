using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omniplatformer.Components;

namespace Omniplatformer.Objects
{
    class Boulder : Projectile
    {
        public Boulder(Vector2 coords, Vector2 direction): base()
        {
            TTL = 50000;
            var movable = new ProjectileMoveComponent(this, coords, new Vector2(10, 10)) { Solid = false, Hittable = false, InverseMass = 1 };
            movable.CurrentMovement = direction;
            Components.Add(movable);
            Components.Add(new RenderComponent(this) { Texture = GameContent.Instance.boulder });
            Components.Add(new DamageHitComponent(this, damage: 3));
        }
    }
}
