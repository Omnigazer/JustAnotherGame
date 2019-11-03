using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omniplatformer.Components.Rendering;

namespace Omniplatformer.Utility
{
    /// <summary>
    /// Helps determine tile texture coordinates.
    /// </summary>
    class LegacyTileHelper
    {
        object[,] Grid { get; set; }
        string[,] TypeGrid => (string[,])Grid;

        public LegacyTileHelper(object[,] grid)
        {
            Grid = grid;
        }

        public void SetTileTexBounds(RenderComponent drawable, int i, int j, string type)
        {
            float x = 0, y = 0;
            Vector2 size = new Vector2(0.33f, 0.33f);
            // if (j > 0 && TypeGrid[i, j - 1] != type)
            if (j > 0 && !CheckForTile(TypeGrid[i, j - 1], type))
            {
                y = 0;
            }
            else if (!CheckForTile(TypeGrid[i, j + 1], type))
                y = 0.66f;
            else y = 0.33f;

            if (i > 0 && !CheckForTile(TypeGrid[i - 1, j], type))
                x = 0;
            else if (!CheckForTile(TypeGrid[i + 1, j], type))
                x = 0.66f;
            else x = 0.33f;

            Vector2 offset = new Vector2(x, y);
            drawable.TexBounds = (offset, size);
        }

        public void SetTileTexBounds(RenderComponent drawable, int i, int j)
        {
            float x = 0, y = 0;
            Vector2 size = new Vector2(0.33f, 0.33f);
            // if (j > 0 && TypeGrid[i, j - 1] != type)
            if (j > 0 && !CheckForTile(Grid[i, j + 1]))
            {
                y = 0;
            }
            else if (!CheckForTile(Grid[i, j - 1]))
                y = 0.66f;
            else y = 0.33f;

            if (i > 0 && !CheckForTile(Grid[i - 1, j]))
                x = 0;
            else if (!CheckForTile(Grid[i + 1, j]))
                x = 0.66f;
            else x = 0.33f;

            Vector2 offset = new Vector2(x, y);
            drawable.TexBounds = (offset, size);
        }

        /*
        public void SetTileTexBounds(RenderComponent drawable, int i, int j, Component component)
        {
            float x = 0, y = 0;
            Vector2 size = new Vector2(0.33f, 0.33f);
            // if (j > 0 && TypeGrid[i, j - 1] != type)
            if (j > 0 && !CheckForTile((Component)Grid[i, j - 1], component))
            {
                y = 0;
            }
            else if (!CheckForTile((Component)Grid[i, j + 1], component))
                y = 0.66f;
            else y = 0.33f;

            if (i > 0 && !CheckForTile((Component)Grid[i - 1, j], component))
                x = 0;
            else if (!CheckForTile((Component)Grid[i + 1, j], component))
                x = 0.66f;
            else x = 0.33f;

            Vector2 offset = new Vector2(x, y);
            drawable.TexBounds = (offset, size);
        }
        */

        // Check whether there's a compatible type in the nearby tile
        public bool CheckForTile(string type, string target_type)
        {
            // return new string[] { "solid", "background" }.Contains(type);
            return new string[] { target_type }.Contains(type);
        }

        // Check whether there's a compatible type in the nearby tile
        public bool CheckForTile(object type)
        {
            // return new string[] { "solid", "background" }.Contains(type);
            // return new Component[] { component }.Contains(type);
            return type != null;
        }
    }
}
