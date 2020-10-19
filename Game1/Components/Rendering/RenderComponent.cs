using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Omniplatformer.Components.Physics;
using Omniplatformer.Content;
using Omniplatformer.Enums;
using Omniplatformer.Objects;
using Omniplatformer.Services;
using Omniplatformer.Utility.JsonConverters;

namespace Omniplatformer.Components.Rendering
{
    public class RenderComponent : Component, IComparable<RenderComponent>
    {
        // public bool Hidden;
        public int ZIndex { get; set; } = Layers.Default;

        public Color DefaultColor { get; set; } = Color.White;
        public Color Color { get; set; }
        public float Opacity { get; set; } = 1;

        [JsonProperty, JsonConverter(typeof(TextureConverter))]
        public Texture2D Texture { get; set; }

        public bool Tiled { get; set; }

        // public PositionComponent pos { get; set; }
        PositionComponent _pos;

        [JsonIgnore]
        public PositionComponent pos
        {
            get
            {
                if (_pos == null)
                    _pos = GetComponent<PositionComponent>();
                return _pos;
            }
        }

        public RenderComponent() { Color = DefaultColor; }

        public RenderComponent(Color color, string texture = null, int z_index = 0, bool tiled = false)
        {
            Color = DefaultColor = color;
            Texture = GameContent.Instance.Load(texture);
            ZIndex = z_index;
            Tiled = tiled;
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
