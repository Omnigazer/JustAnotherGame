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

namespace Omniplatformer
{
    public class Level
    {
        public List<GameObject> objects = new List<GameObject>();
        Game1 Game => GameService.Instance;
        // public List<Character> characters = new List<Character>();
        public Texture2D Background { get; set; }

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

        public void LoadFromBitmap()
        {
            objects = LevelLoader.LoadFromBitmap(@"E:/Games/level1.png");

            Game.Groups.Add("bitmap", new List<GameObject>());
            foreach (var obj in objects)
            {
                Game.Groups["bitmap"].Add(obj);
                Game.AddToMainScene(obj);
            }

            Game.RenderSystem.CurrentBackground = GameContent.Instance.background;
        }

        public void LoadPlayer(Player player)
        {
            var player_pos = (PositionComponent)player;
            player_pos.SetLocalCenter(new Vector2(100, -7500));
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
