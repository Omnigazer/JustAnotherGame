using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omniplatformer.Characters;
using Omniplatformer.Utility;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Omniplatformer.Components;
// using System.Drawing;
// using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Scenes;
using System.Runtime.Serialization.Formatters.Binary;
using ZeroFormatter;
using Omniplatformer.Objects;

namespace Omniplatformer
{
    public class Level : Scene
    {
        // public List<GameObject> objects = new List<GameObject>();
        // public IEnumerable<GameObject> Objects => new List<GameObject>();
        Game1 Game => GameService.Instance;
        // public List<Character> characters = new List<Character>();
        public Texture2D Background { get; set; }
        public Objects.TileMap TileMap { get; set; }

        public Level()
        {

        }

        public void Save(string json_path)
        {
            JsonSerializer serializer = new JsonSerializer();
            BinaryFormatter bf = new BinaryFormatter();
            using (StreamWriter sw = new StreamWriter(json_path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                List<object> list = new List<object>();
                foreach (var obj in this.PhysicsSystem.objects)
                {
                    list.Add(obj.GameObject.AsJson());
                }

                List<Tile> tile_list = new List<Tile>();
                var grid = PhysicsSystem.TileMap.Grid;
                for (int i = 0; i < grid.GetLength(0); i++)
                    for (int j = 0; j < grid.GetLength(1); j++)
                    {
                        if (grid[i, j] != (0, 0))
                        {
                            // char type = grid[i, j] == 1 ? 's' : 'b';
                            var s = new Tile()
                            {
                                Type = grid[i,j],
                                Row = i,
                                Col = j
                            };
                            tile_list.Add(s);
                        }
                    }

                var cont = new GridContainer(tile_list);
                var bytes = ZeroFormatterSerializer.Serialize(cont);

                string tile_path = json_path + ".tile";
                using (var fs = new FileStream(tile_path, FileMode.Create))
                    fs.Write(bytes, 0, bytes.Length);

                serializer.Serialize(writer, new { objects = list });
            }
        }

        public void AddGroup(List<GameObject> group)
        {
            foreach(var obj in group)
            {
                RegisterObject(obj);
            }
        }

        public void LoadFromBitmap(string level_name)
        {
            string image_path = String.Format("Content/Data/LevelBitmaps/{0}.png", level_name);
            foreach (var obj in LevelLoader.LoadFromBitmap(image_path))
            {
                RegisterObject(obj);
            }

            // Game.Groups.Add("bitmap", new List<GameObject>());
            // foreach (var obj in Objects)
            // {
                // Game.Groups["bitmap"].Add(obj);
            //    Game.AddToMainScene(obj);
            // }

            RenderSystem.CurrentBackground = GameContent.Instance.background;
        }

        // TODO: should be moved to level initializer
        public void Load(string name)
        {
            string json_path = String.Format("Content/Data/{0}.json", name);
            JsonSerializer serializer = new JsonSerializer();

            // using (StreamWriter sw = new StreamWriter(json_path))
            // using (JsonWriter writer = new JsonTextWriter(sw))

            using (StreamReader sr = new StreamReader(json_path))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JObject data = (JObject)serializer.Deserialize(reader);

                var storage = new Dictionary<Guid, GameObject>();

                foreach (var obj_data in data["objects"])
                {
                    var deserializer = new Deserializer((JObject)obj_data, storage);
                    string type_name = obj_data["type"].ToString();
                    // var type = Type.GetType(type_name);
                    // var obj = (GameObject)type.GetMethod("FromJson").Invoke(null, new object[] { (JObject)obj_data });
                    var obj = (GameObject)deserializer.decodeObject((JObject)obj_data);
                    RegisterObject(obj);
                }
                // objects = SerializeService.Instance.GetObjects();

                /*
                foreach (var obj_data in data["tiles"])
                {
                    var deserializer = new Deserializer((JObject)obj_data, storage);
                    string type_name = obj_data["type"].ToString();
                    // var type = Type.GetType(type_name);
                    // var obj = (GameObject)type.GetMethod("FromJson").Invoke(null, new object[] { (JObject)obj_data });
                    var obj = (GameObject)deserializer.decodeObject((JObject)obj_data);
                }
                */

                /*
                foreach (var obj in storage.Values)
                    RegisterObject(obj);
                */
            }

            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(json_path + ".tile", FileMode.Open))
            {
                TileMap = new Objects.TileMap();

                var container = ZeroFormatterSerializer.Deserialize<GridContainer>(fs);
                List<Tile> tiles = container.List;

                foreach (Tile tile in tiles) {
                    // TileMap.RegisterTile(new Tile() { Col = tile.Col, Row = tile.Row, Type = ((short)tile.Type, 0) });
                    TileMap.RegisterTile(tile);
                    /*
                    GameObject obj = null;
                    switch(tile.Type)
                    {
                        case 's':
                            {
                                obj = new SolidPlatform(new Vector2(tile.Row * PhysicsSystem.TileSize, tile.Col * PhysicsSystem.TileSize), new Vector2(PhysicsSystem.TileSize / 2, PhysicsSystem.TileSize / 2), Vector2.Zero, true);
                                var drawable = (RenderComponent)obj;
                                // drawable.TexBounds = (tile.TexCoords1, tile.TexCoords2);
                                // drawable.TexBounds = (new Vector2(tile.X1, tile.Y1), new Vector2(tile.X2, tile.Y2));
                                break;
                            }
                        case 'b':
                            {
                                obj = new BackgroundQuad(new Vector2(tile.Row * PhysicsSystem.TileSize, tile.Col * PhysicsSystem.TileSize), new Vector2(PhysicsSystem.TileSize / 2, PhysicsSystem.TileSize / 2), Vector2.Zero, true);
                                var drawable = (RenderComponent)obj;
                                // drawable.TexBounds = (tile.TexCoords1, tile.TexCoords2);
                                // drawable.TexBounds = (new Vector2(tile.X1, tile.Y1), new Vector2(tile.X2, tile.Y2));
                                break;
                            }
                    }

                    // RegisterObject(new SolidPlatform(new Vector2((tile.Row - 2500) * PhysicsSystem.TileSize, (tile.Col - 2500) * PhysicsSystem.TileSize), new Vector2(PhysicsSystem.TileSize / 2, PhysicsSystem.TileSize / 2), Vector2.Zero, true));
                    RegisterObject(obj);
                    */
                }
                RegisterObject(TileMap);
            }

            // TileHelper tile_helper = new TileHelper(PhysicsSystem.tiles);
            // tile_helper.ProcessTiles();

            RenderSystem.CurrentBackground = GameContent.Instance.background;
        }
    }}
