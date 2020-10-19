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
        public GlowingRenderComponent() { }
        public GlowingRenderComponent(Color color, string texture = null, int z_index = 0) : base(color, texture, z_index) { }

        public override void Draw()
        {
        }

        public override void DrawToLightMask()
        {
            var positionable = GetComponent<PositionComponent>();
            var mask_halfsize = positionable.WorldPosition.Halfsize + new Vector2(Radius);
            var rect = new Rectangle((positionable.WorldPosition.Center - mask_halfsize).ToPoint(), (mask_halfsize * 2).ToPoint());
            var light_mask = GameContent.Instance.lightMask;
            // TODO: find a better way to apply glow to stuff
            GraphicsService.DrawGameCentered(light_mask, rect, Scene.RenderSystem.GetLightColor(GlowColor));
        }
    }
}
