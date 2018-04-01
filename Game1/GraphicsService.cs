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

        public static void DrawGame(Texture2D texture, Rectangle rect, Color color)
        {            
            Instance.Draw(texture, game.GameToScreen(rect), color);                        
        }

        public static void DrawGame(Texture2D texture, Rectangle rect, Color color, float rotation)
        {
            Instance.Draw(texture: texture, destinationRectangle: game.GameToScreen(rect), color: color, rotation: rotation, origin: Vector2.Zero,
                effects: SpriteEffects.None, layerDepth: 0, sourceRectangle: null); // default parameters            
        }
    }    
}
