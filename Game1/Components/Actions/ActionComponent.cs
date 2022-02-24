using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Objects;
using Newtonsoft.Json;
using Omniplatformer.HUDStates;
using Omniplatformer.Components.Physics;
using Omniplatformer.Services;
using Microsoft.Xna.Framework;

namespace Omniplatformer.Components.Actions
{
    public abstract class ActionComponent : Component
    {
        public virtual void Perform(GameObject actor, MouseEventArgs e, bool is_down)
        {
            var coords = GameService.Instance.RenderSystem.ScreenToGame(e.Position);
            Perform(actor, new Position(coords, Vector2.Zero), is_down);
        }

        public virtual void Perform(GameObject actor, Position pos, bool is_down) { }
    }
}
