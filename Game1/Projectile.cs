using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public abstract class Projectile : GameObject
    {
        const int default_ttl = 500;
        public Projectile(Vector2 center, Vector2 halfsize)
        {
            TTL = default_ttl;
        }
        public int TTL { get; set; }
        public override void Tick()
        {
            var movable = GetComponent<MoveComponent>();
            movable.Move();
            if (--TTL <= 0)
            {
                onDestroy();
            }
        }
    }
}
