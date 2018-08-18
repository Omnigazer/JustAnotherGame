using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public abstract class ViewControl
    {
        public Point Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Rect => new Rectangle(Position, new Point(Width, Height));
        public List<ViewControl> Content = new List<ViewControl>();

        public bool ContainsMouse { get; private set; }

        public virtual void Draw(Point position)
        {
            var spriteBatch = GraphicsService.Instance;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (var control in Content)
            {
                control.Draw(position + Position);
            }
            spriteBatch.End();
        }
    }
}
