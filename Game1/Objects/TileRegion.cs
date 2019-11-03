using Omniplatformer.Graphics;

namespace Omniplatformer.Objects
{
    public class TileRegion
    {
        TileBuffer BackLayer { get; set; }
        TileBuffer MiddleLayer { get; set; }

        public TileRegion(int tile_count)
        {
            MiddleLayer = new TileBuffer(tile_count);
            BackLayer = new TileBuffer(tile_count);
        }

        public void AddBackgroundTile(int i, int j, short type)
        {
            BackLayer.AddTile(i, j, type);
        }
        public void AddMiddleTile(int i, int j, short type)
        {
            MiddleLayer.AddTile(i, j, type);
        }

        public void ResetBuffers()
        {
            MiddleLayer.ResetBuffers();
            BackLayer.ResetBuffers();
        }

        public void SetData()
        {
            BackLayer.SetData();
            MiddleLayer.SetData();
        }

        public void DrawBack()
        {
            BackLayer.Draw();
        }

        public void Draw()
        {
            MiddleLayer.Draw();
        }

        public void DrawToBackground()
        {
            BackLayer.Draw();
        }
    }
}
