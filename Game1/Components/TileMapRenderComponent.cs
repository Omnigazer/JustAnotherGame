using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class TileMapRenderComponent : RenderComponent
    {
        public GraphicsDevice GraphicsDevice => GameObject.Game.GraphicsDevice;

        // Buffers
        IndexBuffer back_index_buffer;
        int[] _back_index_buffer;
        //
        VertexPositionColorTexture[] _backbuffer;
        VertexBuffer BackBuffer { get; set; }

        // Tex meta
        int AtlasWidth => 1280;
        int AtlasHeight => 1280;
        int TileWidth => 64;
        int TileHeight => 64;


        // List<RenderComponent> tiles = new List<RenderComponent>();
        // List<RenderComponent> background_tiles = new List<RenderComponent>();
        int tile_vertex_count;
        int tile_index_count;


        public TileMapRenderComponent(GameObject obj) : base(obj)
        {
            InitVertexBuffers();
        }

        public TileMapRenderComponent(GameObject obj, Color color) : base(obj, color)
        {

        }

        public TileMapRenderComponent(GameObject obj, Color color, int z_index = 0) : base(obj, color, z_index)
        {

        }

        public TileMapRenderComponent(GameObject obj, Color color, Texture2D texture, int z_index = 0, bool tiled = false) : base(obj, color, texture, z_index, tiled)
        {

        }

        public void InitVertexBuffers()
        {
            _backbuffer = new VertexPositionColorTexture[2000000];
            back_index_buffer = new IndexBuffer(GraphicsDevice, typeof(int), 3000000, BufferUsage.WriteOnly);
            _back_index_buffer = new int[3000000];
            BackBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorTexture), _backbuffer.Length, BufferUsage.None);
        }

        public void UploadBuffers()
        {
            BackBuffer.SetData(_backbuffer);
            back_index_buffer.SetData(_back_index_buffer);
        }

        public (Vector2 offset, Vector2 size) GetTileTexCoords(short type)
        {
            var tex_width = ((float)TileWidth) / AtlasWidth;
            int gtype = ((type - 1) / 9) * 3;
            int ltype = (type - 1) % 9 + 1;
            var offset = new Vector2(gtype + (ltype - 1) % 3, (ltype - 1) / 3) * tex_width;
            var size = new Vector2(tex_width, tex_width);
            return (offset, size);
        }

        public void AddBackgroundTile(int i, int j, short type)
        {
            // Register into the grid here

            int tile_size = PhysicsSystem.TileSize;
            var rect = new Rectangle(i * tile_size - tile_size / 2, j * tile_size - tile_size / 2, tile_size, tile_size);
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

        // public void RemoveTile(RenderComponent tile)
        public void RemoveTile(int i, int j)
        {
            // var tile = (RenderComponent)obj;

            // VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[tiles.Count * 6];
            // TileBuffer.GetData(vertices);

            /*
            for(int i = tiles.IndexOf(tile) * 6; i < _tilebuffer.Length - 12; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    _tilebuffer[i + j] = _tilebuffer[i + j + 6];
                }
            }

            int i = tiles.IndexOf(tile) * 6;
            for (int j = 0; j < 6; j++)
            {
                var x = new VertexPositionColorTexture();
                _tilebuffer[i + j] = x;
            }

            TileBuffer.SetData(_tilebuffer);

            tiles.Remove(tile);
            */
        }

        public override void Draw()
        {
            GraphicsDevice.SetVertexBuffer(BackBuffer);
            BasicEffect basicEffect = new BasicEffect(GameObject.Game.GraphicsDevice);

            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = true;
            // basicEffect.View = Matrix.CreateScale(0.5f);
            basicEffect.World = GameObject.CurrentScene.RenderSystem.Camera.TranslationMatrix;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, 2560, 1440, 0, 0, 1);
            // basicEffect.Texture = GameContent.Instance.backgroundTile;
            basicEffect.Texture = GameContent.Instance.atlas;

            foreach (var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, background_tiles.Count * 2);
                //GraphicsDevice.Indices.SetData()
                GraphicsDevice.Indices = back_index_buffer;
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, tile_vertex_count / 2);
            }
        }
    }
}
