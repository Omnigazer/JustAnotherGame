using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class RenderComponent : Component, IComparable<RenderComponent>
    {
        public int ZIndex { get; set; } = Layers.Default;
        public Color color = Color.AliceBlue;
        public RenderComponent(GameObject obj) : base(obj)
        {
            
        }

        public RenderComponent(GameObject obj, Color color) : base(obj)
        {
            this.color = color;
        }

        public RenderComponent(GameObject obj, Color color, int z_index) : base(obj)
        {
            this.color = color;
            this.ZIndex = z_index;
        }

        public int CompareTo(RenderComponent obj)
        {
            return ZIndex - obj.ZIndex;
        }

        public virtual Color GetColor()
        {
            return color;
        }        

        protected virtual Texture2D getCurrentSprite()
        {            
            return GameContent.Instance.whitePixel;
        }

        // TODO: Move this to the render component
        public Rectangle GetDrawRectangle()
        {            
            PositionComponent pos = GetComponent<PositionComponent>();

            var height = GraphicsService.Instance.GraphicsDevice.Viewport.Height;
            var new_center = new Vector2(pos.Center.X, height - pos.Center.Y);

            var pt = pos.halfsize.ToPoint();
            pt.X *= 2;
            pt.Y *= 2;
            return new Rectangle((new_center - pos.halfsize).ToPoint(), pt);
        }

        public virtual void DrawToLightMask()
        {

        }

        public virtual void DrawToForeground()
        {

        }

        public virtual void DrawToRevealingMask()
        {

        }

        public virtual void Draw()
        {
            PositionComponent pos = GetComponent<PositionComponent>();            
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor(), rotation: pos.RotationAngle);
        }        

        // TODO: move these to more specific renderable implementations        
        public virtual void Draw(float alpha)
        {
            PositionComponent pos = GetComponent<PositionComponent>();
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor() * alpha, rotation: pos.RotationAngle);            
        }        
    }
}
