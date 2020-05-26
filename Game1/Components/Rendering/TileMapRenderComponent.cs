using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Objects;
using Omniplatformer.Scenes.Subsystems;
using Omniplatformer.Services;

namespace Omniplatformer.Components.Rendering
{
    public class TileMapRenderComponent : RenderComponent
    {
        int RegionWidth => (GameService.Instance.RenderSystem.Camera.ViewportWidth) / PhysicsSystem.TileSize;
        int RegionHeight => (GameService.Instance.RenderSystem.Camera.ViewportHeight) / PhysicsSystem.TileSize;

        TileRegion[] regions = new TileRegion[4];

        // current region indices
        int region_i, region_j;
        // player's current quadrant in region
        bool to_the_right, to_the_high;

        public void RebuildBuffers(bool force = false)
        {
            Vector2 coords = GameService.Instance.RenderSystem.Camera.Position;
            var (x, y) = GameService.Instance.PhysicsSystem.GetTileIndices(coords);

            if ((region_i == x / RegionWidth && region_j == y / RegionHeight) &&
                (to_the_right == x % RegionWidth >= RegionWidth / 2) &&
                (to_the_high == y % RegionHeight >= RegionHeight / 2) && !force)
                return;

            region_i = x / RegionWidth;
            region_j = y / RegionHeight;
            to_the_right = x % RegionWidth >= RegionWidth / 2;
            to_the_high = y % RegionHeight >= RegionHeight / 2;

            int h_offset = to_the_right ? 1 : -1;
            int v_offset = to_the_high ? 1 : -1;
            BuildBuffer(0, region_i, region_j);
            BuildBuffer(1, region_i + h_offset, region_j);
            BuildBuffer(2, region_i, region_j + v_offset);
            BuildBuffer(3, region_i + h_offset, region_j + v_offset);
        }

        /// <summary>
        /// Build the tile buffer at the specified region index with target coordinates' tiles
        /// </summary>
        /// <param name="region_index">Index in the local region array</param>
        /// <param name="region_i">X coordinate of the region in the grid</param>
        /// <param name="region_j">Y coordinate of the region in the grid</param>
        void BuildBuffer(int region_index, int region_i, int region_j)
        {
            int tile_count = RegionWidth * RegionHeight;
            var grid = ((Objects.TileMap)GameObject).Grid;
            // TODO: account for the resolution change
            if (regions[region_index] == null)
                regions[region_index] = new TileRegion(tile_count);

            var region = regions[region_index];
            region.ResetBuffers();

            for (int i = region_i * RegionWidth; i < (region_i + 1) * RegionWidth; i++)
                for (int j = region_j * RegionHeight; j < (region_j + 1) * RegionHeight; j++)
                {
                    var (middle_type, back_type) = grid[i, j];
                    if (middle_type != 0)
                        region.AddMiddleTile(i, j, middle_type);
                    if (back_type != 0)
                        region.AddBackgroundTile(i, j, back_type);
                }
            region.SetData();
        }

        public override void DrawToBackground()
        {
            foreach (var region in regions)
                region?.DrawToBackground();
        }

        public override void Draw()
        {
            foreach (var region in regions)
                region?.Draw();
        }
    }
}
