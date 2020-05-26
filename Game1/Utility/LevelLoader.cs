﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Objects;
using Omniplatformer.Objects.Characters;
using Omniplatformer.Objects.Terrain;
using Omniplatformer.Scenes.Subsystems;

namespace Omniplatformer.Utility
{
    public class LevelLoader
    {
        public LevelLoader()
        {

        }

        public static List<GameObject> LoadGroup(string json_path, Vector2 origin)
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
                    pos.AdjustPosition(origin);
                }
                // objects = SerializeService.Instance.GetObjects();
                return storage.Values.ToList();

                //return new Level((JObject)serializer.Deserialize(reader));
            }
        }

        static Vector2 getMinCoords(List<GameObject> list)
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

        /*
        public static void SaveGroup(List<GameObject> group, string json_path)
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
                    pos.AdjustPosition(-origin);
                    list.Add(obj.AsJson());
                    pos.AdjustPosition(origin);
                }
                serializer.Serialize(writer, new { objects = list });
            }
        }
        */

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
    }
}
