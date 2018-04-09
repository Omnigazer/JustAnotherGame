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
        public bool Hidden;
        public int ZIndex { get; set; } = Layers.Default;
        public Color color = Color.AliceBlue;
        public Texture2D Texture { get; set; }
        public RenderComponent(GameObject obj) : base(obj)
        {

        }

        public RenderComponent(GameObject obj, Color color) : base(obj)
        {
            this.color = color;
        }

        public RenderComponent(GameObject obj, Color color, int z_index = 0) : base(obj)
        {
            this.color = color;
            this.ZIndex = z_index;
        }

        public RenderComponent(GameObject obj, Color color, Texture2D texture, int z_index = 0) : base(obj)
        {
            this.color = color;
            Texture = texture;
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
            return Texture ?? GameContent.Instance.whitePixel;
        }

        // TODO: Move this to the render component
        public Rectangle GetDrawRectangle()
        {
            PositionComponent pos = GetComponent<PositionComponent>();

            var height = GraphicsService.Instance.GraphicsDevice.Viewport.Height;
            var new_center = new Vector2(pos.WorldPosition.Center.X, height - pos.WorldPosition.Center.Y);

            var pt = pos.WorldPosition.halfsize.ToPoint();
            pt.X *= 2;
            pt.Y *= 2;
            return new Rectangle((new_center - pos.WorldPosition.halfsize).ToPoint(), pt);
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
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor(), rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin);
        }

        // TODO: move these to more specific renderable implementations
        public virtual void Draw(float alpha)
        {
            PositionComponent pos = GetComponent<PositionComponent>();
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor() * alpha, rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin);
        }
    }
}
