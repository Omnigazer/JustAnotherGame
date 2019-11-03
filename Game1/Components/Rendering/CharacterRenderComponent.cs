using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Animations;
using Omniplatformer.Components.Physics;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Rendering
{
    class CharacterRenderComponent : AnimatedRenderComponent
    {
        protected Texture2D texture;
        public Color defaultColor = Color.Gray;

        public CharacterRenderComponent(GameObject obj, Texture2D texture, Color? color = null) : base(obj)
        {
            AddAnimation(new AttackAnimation(this));
            AddAnimation(new HitAnimation(this));
            AddAnimation(new CastAnimation(this));
            ZIndex = Layers.Character;
            this.texture = texture;
            defaultColor = color ?? Color.Gray;
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
            return defaultColor;
        }

        public void DrawHealth()
        {
            PositionComponent pos = GetComponent<PositionComponent>();
            var rect = pos.GetRectangle();
            Character obj = (Character)GameObject;
            var health_rect = new Rectangle(rect.Left, rect.Bottom + 5, rect.Width, 5);
            GraphicsService.DrawGame(GameContent.Instance.whitePixel, health_rect, Color.Gray, rotation: pos.WorldPosition.RotationAngle, clamped_origin: Vector2.Zero);
            health_rect.Width = (int)(health_rect.Width * (obj.CurrentHitPoints / obj.MaxHitPoints));
            GraphicsService.DrawGame(GameContent.Instance.whitePixel, health_rect, Color.Red, rotation: pos.WorldPosition.RotationAngle, clamped_origin: Vector2.Zero);
        }

        public override void Draw()
        {
            DrawHealth();
            base.Draw();
            /*
            if (CurrentAnimations.ContainsKey(AnimationType.Hit))
            {
                PositionComponent pos = GetComponent<PositionComponent>();
                // or just change the color
                GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), Color.Red, rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin);

            }
            else if (CurrentAnimations.ContainsKey(AnimationType.Attack))
            {
                base.Draw();
            }
            else
                base.Draw();
            */
        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
        }

        protected override Texture2D getCurrentSprite()
        {
            return texture;
        }
    }
}
