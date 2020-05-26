using Omniplatformer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Microsoft.Xna.Framework;
using Omniplatformer.Objects.Projectiles;
using Omniplatformer.Services;

namespace Omniplatformer.Components
{
    public abstract class SpellComponent : Component
    {
        public virtual void Cast(SpellCasterComponent caster) { }
        public virtual void Cast(SpellCasterComponent caster, Position pos) { }
    }
}
