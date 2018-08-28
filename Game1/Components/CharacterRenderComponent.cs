using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        Color defaultColor = Color.Gray;

        public CharacterRenderComponent(GameObject obj, Texture2D left_texture, Texture2D right_texture, Color? color = null) : base(obj)
        {
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
            if (CurrentAnimations.ContainsKey(Animation.Hit))
            {
                PositionComponent pos = GetComponent<PositionComponent>();
                // or just change the color
                GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), Color.Red, rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin);

            }
            else if (CurrentAnimations.ContainsKey(Animation.Attack))
            {
                base.Draw();
            }
            else
                base.Draw();
        }

        public override void Tick(float time_scale)
        {
            // TODO: FIX THIS SHIT!!!
            if (CurrentAnimations.ContainsKey(Animation.Attack))
            {
                var (ticks, length) = CurrentAnimations[Animation.Attack];
                float forward_part = 0.125f;
                float backward_part = 1 - forward_part;
                float amp = (float)Math.PI / 2;
                float forward_step =  amp / (forward_part * length);
                float back_step = amp / (backward_part * length);
                float step = amp / length;
                PositionComponent pos = GetComponent<PositionComponent>();
                var anchor = pos.CurrentAnchors[AnchorPoint.Hand];
                if (ticks == forward_part * length)
                {
                    onAnimationHit(Animation.Attack);
                }
                if (ticks <= forward_part * length)
                    anchor = new Position(anchor) { RotationAngle = anchor.RotationAngle + forward_step };
                else
                    anchor = new Position(anchor) { RotationAngle = anchor.RotationAngle - back_step };
                pos.CurrentAnchors[AnchorPoint.Hand] = anchor;
            }
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
