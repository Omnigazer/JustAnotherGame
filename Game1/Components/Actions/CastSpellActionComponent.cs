using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Newtonsoft.Json;
using Omniplatformer.HUDStates;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;

namespace Omniplatformer.Components.Actions
{
    public class CastSpellActionComponent : ActionComponent
    {
        public override void Perform(GameObject actor, Position pos, bool is_down)
        {
            if (is_down)
            {
                var castable = actor.GetComponent<SpellCasterComponent>();
                GetComponent<SpellComponent>().Cast(castable, pos);
            }
        }
    }
}
