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
        // public bool Hidden;
        public bool Tile;
        public int ZIndex { get; set; } = Layers.Default;
        public Color Color { get; set; } = Color.White;
        public Texture2D Texture { get; set; }
        public bool Tiled { get; set; }
        public PositionComponent pos { get; set; }
        public RenderComponent(GameObject obj) : base(obj)
        {
            this.pos = GetComponent<PositionComponent>();
        }

        public RenderComponent(GameObject obj, Color color) : this(obj, color, 0)
        {

        }

        public RenderComponent(GameObject obj, Color color, int z_index = 0) : this(obj, color, null, z_index)
        {

        }

        public RenderComponent(GameObject obj, Color color, Texture2D texture, int z_index = 0, bool tiled = false) : base(obj)
        {
            this.Color = color;
            Texture = texture;
            this.ZIndex = z_index;
            Tiled = tiled;
            this.pos = GetComponent<PositionComponent>();
        }

        public int CompareTo(RenderComponent obj)
        {
            return ZIndex - obj.ZIndex;
        }

        public virtual Color GetColor()
        {
            return Color;
        }

        protected virtual Texture2D getCurrentSprite()
        {
            return Texture ?? GameContent.Instance.whitePixel;
        }

        /*
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
        */

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
            // PositionComponent pos = GetComponent<PositionComponent>();
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor(), rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin, tiled: Tiled);

            // GraphicsService.DrawGame(GameContent.Instance.whitePixel, rect, Color.Red, rotation: pos.WorldPosition.RotationAngle, clamped_origin: Vector2.Zero);
        }

        // TODO: move these to more specific renderable implementations
        public virtual void Draw(float alpha)
        {
            // PositionComponent pos = GetComponent<PositionComponent>();
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor() * alpha, rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin);
        }
    }
}
