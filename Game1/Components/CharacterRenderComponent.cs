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

        public CharacterRenderComponent(GameObject obj, Texture2D left_texture, Texture2D right_texture) : base(obj)
        {
            ZIndex = Layers.Character;
            this.left_texture = left_texture;
            this.right_texture = right_texture;
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
            return Color.Gray;
        }        

        public override void Draw()
        {            
            if (CurrentAnimation == Animation.Hit)
            {                
                PositionComponent pos = GetComponent<PositionComponent>();
                // or just change the color
                GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), Color.Red);                
            }
            else
                base.Draw();            
        }        

        protected override Texture2D getCurrentSprite()
        {
            var movable = GetComponent<CharMoveComponent>();
            if (movable.face_direction == Direction.Left)
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
