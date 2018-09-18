using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Animations
{
    public class AttackAnimation : Animation
    {
        public override AnimationType AnimationType => AnimationType.Attack;
        int current_step;

        public AttackAnimation(AnimatedRenderComponent drawable) : base(drawable)
        {

        }

        public override void Start(float duration)
        {
            current_step = 0;
            base.Start(duration);
        }

        public override void End()
        {
            Drawable.pos.ResetAnchor(AnchorPoint.Hand);
            base.End();
        }

        class A { public static void B() { } }

        public override void Tick(float time_scale)
        {
            // var (CurrentTime, Duration, current_step) = CurrentAnimations[AnimationType.Attack];
            float first_backswing_part = 0.3f;
            float forward_part = 0.1f;
            float backward_part = 1 - forward_part - first_backswing_part;
            float amp = (float)Math.PI / 2;
            float first_backswing_step = time_scale * 0.3f * amp / (first_backswing_part * Duration);
            float forward_step = time_scale * 1.3f * amp / ((forward_part) * Duration);
            float back_step = time_scale * amp / (backward_part * Duration);
            float step = amp / Duration;
            // PositionComponent pos = Drawable.posGetComponent<PositionComponent>();
            PositionComponent pos = Drawable.pos;
            var anchor = pos.CurrentAnchors[AnchorPoint.Hand];
            switch (current_step)
            {
                case 0:
                    {
                        anchor = new Position(anchor) { RotationAngle = anchor.RotationAngle - first_backswing_step };
                        if (CurrentTime >= first_backswing_part * Duration)
                            current_step++;
                        break;
                    }
                case 1:
                    {
                        anchor = new Position(anchor) { RotationAngle = anchor.RotationAngle + forward_step };
                        if (CurrentTime >= (first_backswing_part + forward_part) * Duration)
                        {
                            Drawable.onAnimationHit(AnimationType.Attack);
                            current_step++;
                        }
                        break;
                    }
                case 2:
                    {
                        anchor = new Position(anchor) { RotationAngle = anchor.RotationAngle - back_step };
                        break;
                    }
            }
            pos.CurrentAnchors[AnchorPoint.Hand] = anchor;
            base.Tick(time_scale);
        }
    }
}
