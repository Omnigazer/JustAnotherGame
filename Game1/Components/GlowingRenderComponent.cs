using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class GlowingRenderComponent : RenderComponent
    {
        public GlowingRenderComponent(GameObject obj) : base(obj)
        {

        }

        public GlowingRenderComponent(GameObject obj, Color color) : base(obj, color)
        {

        }

        public GlowingRenderComponent(GameObject obj, Color color, int z_index) : base(obj, color, z_index)
        {

        }

        public override void DrawToLightMask()
        {
            //var projectile = projectiles[i];
            var pos = GetComponent<PositionComponent>();
            var mask_halfsize = new Vector2(100, 100);
            var rect = new Rectangle((pos.WorldPosition.Center - mask_halfsize).ToPoint(), (mask_halfsize * 2).ToPoint());
            // spriteBatch.Draw(lightMask, GameToScreen(rect), GetLightColor());
            var lightMask = GameContent.Instance.lightMask;
            // TODO: find a better way to apply glow to stuff
            GraphicsService.DrawGameCentered(lightMask, rect, GraphicsService.RenderSystem.GetLightColor(Color.Orange));
        }
    }
}
