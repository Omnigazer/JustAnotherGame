using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public static class GraphicsService
    {
        static Game1 game;
        public static RenderSystem RenderSystem => game.RenderSystem;
        public static SpriteBatch Instance { get; private set; }
        public static GraphicsDevice GraphicsDevice => game.GraphicsDevice;

        public static void Init(SpriteBatch spriteBatch, Game1 game)
        {
            Instance = spriteBatch;
            GraphicsService.game = game;
        }

        public static void DrawGameCentered(Texture2D texture, Rectangle rect, Color color)
        {
            // Instance.Draw(texture, game.GameToScreen(rect, new Vector2(0.5f, 0.5f)), color);
            DrawGame(texture, rect, color, 0, new Vector2(0.5f, 0.5f));
        }

        public static void DrawGameCentered(Texture2D texture, Rectangle rect, Color color, float rotation)
        {
            // DrawGame(texture, rect, color, rotation, new Vector2(0.5f, 0.5f));
            DrawGame(texture, rect, color, rotation, new Vector2(0.5f, 0.5f));
            // DrawGame(texture, rect, color, rotation, new Vector2(1, 1));
        }

        public static void DrawScreen(Texture2D texture, Rectangle rect, Color color, float rotation, Vector2 clamped_origin, float scale = 1, bool tiled = false)
        {
            rect = new Rectangle(rect.X, rect.Y, (int)(rect.Width * scale), (int)(rect.Height * scale));
            // clamped_origin = new Vector2(clamped_origin.X, 1 - clamped_origin.Y);
            var bounds = texture.Bounds;
            if (tiled)
            {
                bounds.Size = new Point((int)(rect.Size.X / scale), (int)(rect.Size.Y / scale));
                // bounds.Inflate(0 * bounds.Width, 0 * bounds.Height);
            }


            // var origin = new Vector2(texture.Bounds.Width * clamped_origin.X, texture.Bounds.Height * clamped_origin.Y);
            var origin = new Vector2(bounds.Width * clamped_origin.X, bounds.Height * clamped_origin.Y);
            var screen_rect = rect;// game.GameToScreen(rect, clamped_origin);
            //if (origin.Length() > 0)
               // screen_rect.Offset(rect.Size.X * clamped_origin.X, rect.Size.Y * clamped_origin.Y);
            // if (clamped_origin.Length() > 0) { if (origin.Length() > 0) { } }
            // var bounds = texture.Bounds;
            // bounds.Inflate((x_tiles - 1) * bounds.Width,  (y_tiles - 1) * bounds.Height);
            Instance.Draw(texture: texture, destinationRectangle: screen_rect, color: color, rotation: rotation, origin: origin,
                effects: SpriteEffects.None, layerDepth: 0, sourceRectangle: bounds); // default parameters
        }

        public static void DrawGame(Texture2D texture, Rectangle rect, Color color, float rotation, Vector2 clamped_origin, bool tiled = false)
        {
            clamped_origin = new Vector2(clamped_origin.X, 1 - clamped_origin.Y);
            var bounds = texture.Bounds;
            if (tiled)
            {
                bounds.Size = rect.Size;
                // bounds.Inflate(0 * bounds.Width, 0 * bounds.Height);
            }


            // var origin = new Vector2(texture.Bounds.Width * clamped_origin.X, texture.Bounds.Height * clamped_origin.Y);
            var origin = new Vector2(bounds.Width * clamped_origin.X, bounds.Height * clamped_origin.Y);
            var screen_rect = GameToScreen(rect, clamped_origin);
            if (origin.Length() > 0)
                screen_rect.Offset(rect.Size.X * clamped_origin.X, rect.Size.Y * clamped_origin.Y);
            // if (clamped_origin.Length() > 0) { if (origin.Length() > 0) { } }
            // var bounds = texture.Bounds;
            // bounds.Inflate((x_tiles - 1) * bounds.Width,  (y_tiles - 1) * bounds.Height);


            Instance.Draw(texture: texture, destinationRectangle: screen_rect, color: color, rotation: rotation, origin: origin,
                effects: SpriteEffects.None, layerDepth: 0, sourceRectangle: bounds); // default parameters
        }

        public static Rectangle GameToScreen(Rectangle rect, Vector2 clamped_origin)
        {
            // rect.Location = new Point(rect.Location.X, -rect.Location.Y);
            // manual flip
            rect.Location = new Point(rect.Location.X, -rect.Location.Y - rect.Height);
            // rect.Location = new Point(rect.Location.X, -rect.Location.Y - rect.Height + (int)(2 * rect.Height * clamped_origin.Y));

            return rect;
        }
    }
}
