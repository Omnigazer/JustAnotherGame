using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    public class TileRegion
    {
        // Tex meta
        int AtlasWidth => GameContent.Instance.atlas.Width;
        int AtlasHeight => GameContent.Instance.atlas.Height;

        public int tile_vertex_count;
        public int tile_index_count;
        // Buffers
        public IndexBuffer back_index_buffer;
        public int[] _back_index_buffer;
        //
        public VertexPositionColorTexture[] _backbuffer;
        public VertexBuffer BackBuffer { get; set; }

        public GraphicsDevice GraphicsDevice => GraphicsService.GraphicsDevice;
        BasicEffect basicEffect => GameService.Instance.RenderSystem.basicEffect;

        public TileRegion(int tile_count)
        {
            int vertex_count = 4 * tile_count;
            int index_count = 6 * tile_count;
            _backbuffer = new VertexPositionColorTexture[vertex_count];
            BackBuffer = new VertexBuffer(GameService.Instance.GraphicsDevice, typeof(VertexPositionColorTexture), _backbuffer.Length, BufferUsage.WriteOnly);
            _back_index_buffer = new int[index_count];
            back_index_buffer = new IndexBuffer(GameService.Instance.GraphicsDevice, typeof(int), index_count, BufferUsage.WriteOnly);
        }

        public (Vector2 offset, Vector2 size) GetTileTexCoords(short type)
        {
            if (GameContent.Instance.atlas_meta.ContainsKey(type))
            {
                var source_rect = GameContent.Instance.atlas_meta[type];
                return (new Vector2((float)source_rect.Left / AtlasWidth, (float)source_rect.Top / AtlasHeight),
                    new Vector2((float)source_rect.Width / AtlasWidth, (float)source_rect.Height / AtlasHeight));
            }
            return (Vector2.Zero, Vector2.Zero);
        }

        public void AddBackgroundTile(int i, int j, short type)
        {
            // Register into the grid here

            int tile_size = PhysicsSystem.TileSize;
            // var rect = new Rectangle(i * tile_size - tile_size / 2, j * tile_size - tile_size / 2, tile_size, tile_size);
            var rect = new Rectangle(i * tile_size, j * tile_size, tile_size, tile_size);
            var color = Color.White;

            var (offset, size) = GetTileTexCoords(type);

            _backbuffer[tile_vertex_count++] = new VertexPositionColorTexture(new Vector3(rect.Left, -rect.Bottom, 0), color, offset);
            _backbuffer[tile_vertex_count++] = new VertexPositionColorTexture(new Vector3(rect.Right, -rect.Bottom, 0), color, offset + new Vector2(size.X, 0));
            _backbuffer[tile_vertex_count++] = new VertexPositionColorTexture(new Vector3(rect.Left, -rect.Top, 0), color, offset + new Vector2(0, size.Y));
            _backbuffer[tile_vertex_count++] = new VertexPositionColorTexture(new Vector3(rect.Right, -rect.Top, 0), color, offset + size);

            _back_index_buffer[tile_index_count++] = tile_vertex_count - 4;
            _back_index_buffer[tile_index_count++] = tile_vertex_count - 3;
            _back_index_buffer[tile_index_count++] = tile_vertex_count - 2;

            _back_index_buffer[tile_index_count++] = tile_vertex_count - 2;
            _back_index_buffer[tile_index_count++] = tile_vertex_count - 3;
            _back_index_buffer[tile_index_count++] = tile_vertex_count - 1;
        }

        public void SetData()
        {
            BackBuffer.SetData(_backbuffer);
            back_index_buffer.SetData(_back_index_buffer);
        }

        public void ResetBuffers()
        {
            for (int i = 0; i < _back_index_buffer.Length; i++)
            {
                _back_index_buffer[i] = 0;
            }
            for (int i = 0; i < _backbuffer.Length; i++)
            {
                _backbuffer[i] = new VertexPositionColorTexture();
            }
            tile_index_count = 0;
            tile_vertex_count = 0;
        }

        public void Draw()
        {
            GraphicsDevice.SetVertexBuffer(BackBuffer);
            basicEffect.World = GameService.Instance.MainScene.RenderSystem.Camera.TranslationMatrix;

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.Indices = back_index_buffer;
                if (tile_vertex_count > 0)
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, tile_vertex_count / 2);
            }
        }
    }
}
