using Microsoft.Xna.Framework;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUDStates.Editor.Factories
{
    public abstract class EditorFactory
    {
        public abstract GameObject Call(Vector2 coords, Vector2 halfsize);
        public abstract void Draw(Vector2 coords, Vector2 halfsize);

        public override string ToString()
        {
            return GetType().Name.ToString();
        }
    }
}
