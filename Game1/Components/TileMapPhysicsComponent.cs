using Microsoft.Xna.Framework;
using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class TileMapPhysicsComponent : PhysicsComponent
    {
        PhysicsSystem PhysicsSystem => Scene.PhysicsSystem;
        public short[,] Grid { get; set; }
        public static int TileSize => PhysicsSystem.TileSize;

        public TileMapPhysicsComponent(GameObject obj, short[,] grid): base(obj, Vector2.Zero, Vector2.Zero)
        // public TileMapPhysicsComponent(GameObject obj, short[,] grid) : base(obj)
        {
            Solid = true;
            InverseMass = 0;
            Grid = grid;
            Friction = 0.3f;
        }

        public void ProcessCollision(PhysicsComponent physicable)
        {

        }

        // public IEnumerable<short> GetTilesFor(PhysicsComponent body)
        public IEnumerable<(int, int)> GetTilesFor(PhysicsComponent body)
        {
            var rect = body.GetRectangle();

            int left_index = rect.Left / TileSize - 1;
            int width = rect.Width / TileSize + 2;
            int top_index = rect.Top / TileSize - 1;
            int height = rect.Height / TileSize + 2;
            for (int i = left_index; i <= left_index + width; i++)
                for (int j = top_index; j <= top_index + height; j++)
                {
                    if (i >= 0 && j >= 0 && i < Grid.GetLength(0) && j < Grid.GetLength(1) && Grid[i, j] != 0)
                        // yield return (i, j)Grid[i, j];
                        yield return (i, j);
                }
        }
    }
}
