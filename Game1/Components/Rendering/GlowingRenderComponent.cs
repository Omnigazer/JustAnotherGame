using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components.Physics;
using Omniplatformer.Content;
using Omniplatformer.Objects;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Rendering
{
    public class GlowingRenderComponent : RenderComponent
    {
        public Color GlowColor { get; set; } = Color.Orange;
        public int Radius { get; set; } = 100;
        public GlowingRenderComponent(GameObject obj) : base(obj)
        {

        }

        public GlowingRenderComponent(GameObject obj, Color color) : base(obj, color)
        {

        }

        public GlowingRenderComponent(GameObject obj, Color color, Texture2D texture) : base(obj, color, texture)
        {

        }

        public GlowingRenderComponent(GameObject obj, Color color, int z_index) : base(obj, color, z_index)
        {

        }

        public override void DrawToLightMask()
        {
            //var projectile = projectiles[i];
            var pos = GetComponent<PositionComponent>();
            // var mask_halfsize = new Vector2(100, 100);
            var mask_halfsize = pos.WorldPosition.halfsize + new Vector2(Radius);
            var rect = new Rectangle((pos.WorldPosition.Center - mask_halfsize).ToPoint(), (mask_halfsize * 2).ToPoint());
            // spriteBatch.Draw(lightMask, GameToScreen(rect), GetLightColor());
            var lightMask = GameContent.Instance.lightMask;
            // TODO: find a better way to apply glow to stuff
            GraphicsService.DrawGameCentered(lightMask, rect, Scene.RenderSystem.GetLightColor(GlowColor));
        }
    }
}
