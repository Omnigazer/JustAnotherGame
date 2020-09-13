using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates.Editor.Factories
{
    public class SolidPlatformFactory : EditorFactory
    {
        SolidPlatform template;

        public SolidPlatformFactory()
        {
            template = SolidPlatform.Create(Vector2.Zero, Vector2.Zero);
        }

        public override void Draw(Vector2 coords, Vector2 halfsize)
        {
            template.GetComponent<PositionComponent>().SetWorldCenter(coords);
            template.GetComponent<PositionComponent>().SetLocalHalfsize(halfsize);
            template.GetComponent<RenderComponent>().Draw();
        }

        public override GameObject Call(Vector2 coords, Vector2 halfsize)
        {
            return SolidPlatform.Create(coords, halfsize);
        }
    }
}
