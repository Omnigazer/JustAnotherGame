using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Physics;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;
using Omniplatformer.Components.Character;

namespace Omniplatformer.Spells
{
    class FireBolt : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new FireBoltSpellComponent());
        }

        public static FireBolt Create()
        {
            var bolt = new FireBolt();
            bolt.InitializeComponents();
            return bolt;
        }
    }
}
