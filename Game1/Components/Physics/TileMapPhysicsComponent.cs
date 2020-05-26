﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Omniplatformer.Objects;
using Omniplatformer.Scenes.Subsystems;

namespace Omniplatformer.Components.Physics
{
    public class TileMapPhysicsComponent : PhysicsComponent
    {
        [JsonIgnore]
        PhysicsSystem PhysicsSystem => Scene.PhysicsSystem;
        [JsonIgnore]
        public (short, short)[,] Grid { get; set; }
        public static int TileSize => PhysicsSystem.TileSize;

        public TileMapPhysicsComponent() { }

        public TileMapPhysicsComponent((short, short)[,] grid) : base(Vector2.Zero, Vector2.Zero)
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
                    if (i >= 0 && j >= 0 && i < Grid.GetLength(0) && j < Grid.GetLength(1) && Grid[i, j].Item1 != 0)
                        // yield return (i, j)Grid[i, j];
                        yield return (i, j);
                }
        }
    }
}
