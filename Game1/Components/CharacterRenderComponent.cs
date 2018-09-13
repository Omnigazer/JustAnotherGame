using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    class CharacterRenderComponent : AnimatedRenderComponent
    {
        protected Texture2D left_texture, right_texture;
        public Color defaultColor = Color.Gray;

        public CharacterRenderComponent(GameObject obj, Texture2D left_texture, Texture2D right_texture, Color? color = null) : base(obj)
        {
            Animations.Add(AnimationType.Attack, new AttackAnimation(this));
            Animations.Add(AnimationType.Hit, new HitAnimation(this));
            Animations.Add(AnimationType.Cast, new CastAnimation(this));
            ZIndex = Layers.Character;
            this.left_texture = left_texture;
            this.right_texture = right_texture;
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

        public override void Tick(float time_scale)
        {
            base.Tick(time_scale);
        }

        protected override Texture2D getCurrentSprite()
        {
            var pos = GetComponent<PositionComponent>();
            if (pos.WorldPosition.face_direction == HorizontalDirection.Left)
            {
                return left_texture;
            }
            else
            {
                return right_texture;
            }
        }
    }
}
