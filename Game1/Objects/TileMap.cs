using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Objects
{
    public class TileMap : GameObject
    {
        public int GridWidth { get; set; } = 5000;
        public int GridHeight { get; set; } = 5000;
        public short[,] Grid { get; set; }

        public TileMap()
        {
            Grid = new short[GridWidth, GridHeight];
            var drawable = new TileMapRenderComponent(this);
            Components.Add(drawable);
            var physicable = new TileMapPhysicsComponent(this, Grid);
            Components.Add(physicable);
        }

        public void RegisterTile(Tile tile)
        {
            // short val = (short)(tile.Type == 's' ? 1 : 2);
            short val = (short)tile.Type;
            Grid[tile.Row, tile.Col] = val;
        }

        public void RemoveTile(Tile tile)
        {
            Grid[tile.Row, tile.Col] = 0;
            var drawable = (TileMapRenderComponent)this;
            // drawable.ReloadBuffer();
            // drawable.
        }
    }
}
