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
using Omniplatformer.Components.Actions;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Content;

namespace Omniplatformer.Spells
{
    class FireBolt : GameObject
    {
        public override void InitializeCustomComponents()
        {
            RegisterComponent(new RenderComponent(Color.White, "Textures/fire_bolt"));
            RegisterComponent(new FireBoltSpellComponent());
            RegisterComponent(new CastSpellActionComponent());
        }

        public static FireBolt Create()
        {
            var bolt = new FireBolt();
            bolt.InitializeComponents();
            return bolt;
        }
    }
}
