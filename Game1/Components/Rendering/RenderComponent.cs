using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components.Physics;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Rendering
{
    public class RenderComponent : Component, IComparable<RenderComponent>
    {
        // public bool Hidden;
        public int ZIndex { get; set; } = Layers.Default;
        public Color DefaultColor { get; set; } = Color.White;
        public Color Color { get; set; }
        public float Opacity { get; set; } = 1;
        public Texture2D Texture { get; set; }
        // offset, size
        public (Vector2, Vector2) TexBounds { get; set; } = (Vector2.Zero, new Vector2(1));
        public bool Tiled { get; set; }
        public PositionComponent pos { get; set; }
        public RenderComponent(GameObject obj) : base(obj)
        {
            pos = GetComponent<PositionComponent>();
            Color = DefaultColor;
        }

        public RenderComponent(GameObject obj, Color color, Texture2D texture = null, int z_index = 0, bool tiled = false) : base(obj)
        {
            Color = DefaultColor = color;
            Texture = texture;
            ZIndex = z_index;
            Tiled = tiled;
            pos = GetComponent<PositionComponent>();
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

        public virtual void DrawToLightMask() { }
        public virtual void DrawToForeground() { }
        public virtual void DrawToRevealingMask() { }
        public virtual void DrawToBackground() { }

        public virtual void Draw()
        {
            GraphicsService.DrawGame(getCurrentSprite(), pos.GetRectangle(), GetColor() * Opacity, rotation: pos.WorldPosition.RotationAngle, clamped_origin: pos.WorldPosition.Origin, tiled: Tiled, flipped: pos.WorldPosition.FaceDirection == HorizontalDirection.Left);
        }
    }
}
