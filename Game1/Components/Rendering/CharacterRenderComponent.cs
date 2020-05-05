using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Animations;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Rendering
{
    class CharacterRenderComponent : AnimatedRenderComponent
    {
        public CharacterRenderComponent() { }
        public CharacterRenderComponent(GameObject obj, Color color, string texture = null) : base(obj, color, texture)
        {
            AddAnimation(new AttackAnimation(this));
            AddAnimation(new HitAnimation(this));
            AddAnimation(new CastAnimation(this));
            ZIndex = Layers.Character;
        }

        public override Color GetColor()
        {
            // TODO: get rid of this and move it into player-specific renderer
            var movable = GetComponent<PlayerMoveComponent>();
            if (movable != null)
            {
                if (movable.IsPinnedToWall())
                {
                    return Color.Yellow;
                }
                else if (movable.remaining_jumps <= 0)
                {
                    return Color.Red;
                }
            }
            return Color;
        }

        public void DrawHealth()
        {
            var hit_points = GetComponent<HitPointComponent>();
            if (hit_points == null)
                return;

            PositionComponent pos = GetComponent<PositionComponent>();
            var rect = pos.GetRectangle();
            var health_rect = new Rectangle(rect.Left, rect.Bottom + 5, rect.Width, 5);
            GraphicsService.DrawGame(GameContent.Instance.whitePixel, health_rect, Color.Gray, rotation: pos.WorldPosition.RotationAngle, clamped_origin: Vector2.Zero);
            health_rect.Width = (int)(health_rect.Width * (hit_points.CurrentHitPoints / hit_points.MaxHitPoints));
            GraphicsService.DrawGame(GameContent.Instance.whitePixel, health_rect, Color.Red, rotation: pos.WorldPosition.RotationAngle, clamped_origin: Vector2.Zero);
        }

        public override void Draw()
        {
            DrawHealth();
            base.Draw();
        }

        protected override Texture2D getCurrentSprite()
        {
            return Texture;
        }
    }
}
