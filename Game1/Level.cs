﻿using Microsoft.Xna.Framework;
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

namespace Omniplatformer
{
    public class Level
    {
        public List<GameObject> objects = new List<GameObject>();
        public List<Character> characters = new List<Character>();
        public Level()
        {

        }

        public void Save(string json_path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(json_path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                List<object> list = new List<object>();
                foreach (var obj in objects)
                {
                    list.Add(obj.AsJson());
                }
                serializer.Serialize(writer, new { objects = list });
            }
        }

        public void AddGroup(List<GameObject> group)
        {
            foreach(var obj in group)
            {
                objects.Add(obj);
            }
        }

        public static int[,] ImageTo2DByteArray(System.Drawing.Bitmap bmp)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            byte[] bytes = new byte[height * data.Stride];
            try
            {
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            }
            finally
            {
                bmp.UnlockBits(data);
            }

            int[,] result = new int[height, width];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int offset = y * data.Stride + x * 3;
                    result[y, x] = (((bytes[offset + 0] << 16) + (bytes[offset + 1] << 8) + bytes[offset + 2]));
                }
            return result;
        }

        public List<GameObject> LoadFromBitmap(string path)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(path);
            System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();
            int[,] array = ImageTo2DByteArray(bitmap);
            int scale = 1;
            var list = new List<GameObject>();
            int tile_size = PhysicsSystem.TileSize;
            var sectors = new string[bitmap.Width / scale, bitmap.Height / scale];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var color = System.Drawing.Color.FromArgb(array[j, i]);
                    // Color order is reversed here for some reason, i.e. R is swapped with B
                    switch (color) {
                        case var c when (c.B == 0 && c.G == 0 && c.R == 0):
                            {
                                sectors[i / scale, j / scale] = "solid";
                                break;
                            }
                        case var c when (c.B == 128 && c.G == 128 && c.R == 128):
                            {
                                sectors[i / scale, j / scale] = "background";
                                break;
                            }
                        case var c when (c.B < c.R && false):
                            {
                                sectors[i / scale, j / scale] = "liquid";
                                break;
                            }
                        case var c when (c.B == 0 && c.G == 127 && c.R == 14):
                            {
                                sectors[i / scale, j / scale] = "goblin";
                                break;
                            }
                        case var c when (c.B == 255 && c.G == 106 && c.R == 0):
                            {
                                sectors[i / scale, j / scale] = "goblin shaman";
                                break;
                            }

                    }
                }
            }
            for (int i = 0; i < bitmap.Width / scale; i++)
                for (int j = 0; j < bitmap.Height / scale; j++)
                {
                    switch (sectors[i, j]) {
                        case "solid":
                            {
                                var obj = new SolidPlatform(
                                new Vector2(i * tile_size - 100 * tile_size, -j * tile_size + 100 * tile_size),
                                new Vector2(tile_size / 2, tile_size / 2),
                                Vector2.Zero,
                                true
                                );
                                var drawable = (RenderComponent)obj;
                                if (j > 0 && sectors[i, j - 1] == "solid")
                                    drawable.TexBounds = (new Vector2(0.5f, 0), new Vector2(0.5f, 1));
                                else
                                    drawable.TexBounds = (new Vector2(0, 0), new Vector2(0.5f, 1));
                                list.Add(obj);
                                break;
                            }
                        case "background":
                            {
                                list.Add(new BackgroundQuad(
                                new Vector2(i * tile_size - 100 * tile_size, -j * tile_size + 100 * tile_size),
                                new Vector2(tile_size / 2, tile_size / 2),
                                Vector2.Zero,
                                true
                                ));
                                break;
                            }
                        case "liquid":
                            {
                                list.Add(new Liquid(
                                    new Vector2(i * tile_size - 100 * tile_size, -j * tile_size + 100 * tile_size),
                                    new Vector2(tile_size / 2, tile_size / 2),
                                    Vector2.Zero,
                                    true
                                    ));
                                break;
                            }
                        case "goblin":
                            {
                                list.Add(new Goblin(
                                    new Vector2(i * tile_size - 100 * tile_size, -j * tile_size + 100 * tile_size)
                                    ));
                                break;
                            }
                        case "goblin shaman":
                            {
                                list.Add(new GoblinShaman(
                                    new Vector2(i * tile_size - 100 * tile_size, -j * tile_size + 100 * tile_size)
                                    ));
                                break;
                            }
                    }

                }
            return list;
        }

        public void SaveGroup(List<GameObject> group, string json_path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(json_path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                List<object> list = new List<object>();
                var origin = getMinCoords(group);

                foreach (var obj in group)
                {
                    var pos = (PositionComponent)obj;
                    pos.local_position.Coords -= origin;
                    list.Add(obj.AsJson());
                    pos.local_position.Coords += origin;
                }
                serializer.Serialize(writer, new { objects = list });
            }
        }

        public List<GameObject> LoadGroup(string json_path, Vector2 origin)
        {
            // string json_path = @"E:\test_json.json";
            JsonSerializer serializer = new JsonSerializer();

            // using (StreamWriter sw = new StreamWriter(json_path))
            // using (JsonWriter writer = new JsonTextWriter(sw))

            using (StreamReader sr = new StreamReader(json_path))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                //foreach (var obj in objects)
                //{
                //    serializer.Deserialize(reader);
                //}

                JObject data = (JObject)serializer.Deserialize(reader);

                var storage = new Dictionary<Guid, GameObject>();

                foreach (var obj_data in data["objects"])
                {
                    var deserializer = new Deserializer((JObject)obj_data, storage);
                    string type_name = obj_data["type"].ToString();
                    // var type = Type.GetType(type_name);
                    // var obj = (GameObject)type.GetMethod("FromJson").Invoke(null, new object[] { (JObject)obj_data });
                    var obj = (GameObject)deserializer.decodeObject((JObject)obj_data);
                    var pos = (PositionComponent)obj;
                    pos.local_position.Coords += origin;
                }
                // objects = SerializeService.Instance.GetObjects();
                return storage.Values.ToList();

                //return new Level((JObject)serializer.Deserialize(reader));
            }
        }

        Vector2 getMinCoords(List<GameObject> list)
        {
            var pos_list = list.Select((obj) =>
            {
                return (PositionComponent)obj;
            }).Where((pos) => pos != null);
            var minx = pos_list.Min((pos) =>
            {
                return pos.WorldPosition.Coords.X;
            });
            var miny = pos_list.Min((pos) =>
            {
                return pos.WorldPosition.Coords.Y;
            });
            return new Vector2(minx, miny);
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
                //foreach (var obj in objects)
                //{
                //    serializer.Deserialize(reader);
                //}

                JObject data = (JObject)serializer.Deserialize(reader);

                var storage = new Dictionary<Guid, GameObject>();

                foreach (var obj_data in data["objects"])
                {
                    var deserializer = new Deserializer((JObject)obj_data, storage);
                    string type_name = obj_data["type"].ToString();
                    // var type = Type.GetType(type_name);
                    // var obj = (GameObject)type.GetMethod("FromJson").Invoke(null, new object[] { (JObject)obj_data });
                    var obj = (GameObject)deserializer.decodeObject((JObject)obj_data);
                }
                // objects = SerializeService.Instance.GetObjects();
                objects = storage.Values.ToList();

                //return new Level((JObject)serializer.Deserialize(reader));
            }
        }
    }}
