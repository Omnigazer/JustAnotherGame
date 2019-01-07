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
        public Projectile(GameObject source = null)
        {
            TTL = default_ttl;
            _source = source;
        }

        private GameObject _source;

        public float TTL { get; set; }
        public override GameObject Source => _source?.Source ?? this;
        public override void Tick(float dt)
        {
            TTL -= dt;
            if (TTL <= 0)
            {
                onDestroy();
            }
            base.Tick(dt);
        }
    }
}
