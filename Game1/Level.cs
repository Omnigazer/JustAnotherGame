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

namespace Omniplatformer
{
    public class Level
    {
        public List<GameObject> objects = new List<GameObject>();
        public List<Character> characters = new List<Character>();
        public Level()
        {

        }

        /*
        public Level(JObject data)
        {
            foreach (var obj_data in data["objects"])
            {
                switch ((string)obj_data["type"])
                {
                    case "SolidPlatform":
                        {
                            SolidPlatform platform;
                            var coords = new Vector2(float.Parse((string)obj_data["coords"]["x"]), float.Parse((string)obj_data["coords"]["y"]));
                            var halfsize = new Vector2(float.Parse((string)obj_data["halfsize"]["x"]), float.Parse((string)obj_data["halfsize"]["y"]));


                            if (obj_data["origin"]?.Type == JTokenType.Object)
                            {
                                Vector2 origin = new Vector2(float.Parse((string)obj_data["origin"]["x"]), float.Parse((string)obj_data["origin"]["y"]));
                                platform = new SolidPlatform(coords, halfsize, origin);
                            }
                            else
                            {
                                platform = new SolidPlatform(coords, halfsize);
                            }
                            objects.Add(platform);
                            break;
                        }
                }
            }

            foreach (var obj_data in data["objects"])
            {
                switch ((string)obj_data["type"])
                {
                    case "ToughZombie":
                        {
                            ToughZombie zombie;
                            var coords = new Vector2(float.Parse((string)obj_data["coords"]["x"]), float.Parse((string)obj_data["coords"]["y"]));
                            var halfsize = new Vector2(float.Parse((string)obj_data["halfsize"]["x"]), float.Parse((string)obj_data["halfsize"]["y"]));
                            zombie = new ToughZombie(coords, halfsize);

                            characters.Add(zombie);
                            break;
                        }
                }
            }
        }
        */

        public void Save(string json_path)
        {
            // json_path = @"E:\test_json.json";
            JsonSerializer serializer = new JsonSerializer();
            // using (StreamReader sr = new StreamReader(json_path))
            using (StreamWriter sw = new StreamWriter(json_path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            //using (JsonReader reader = new JsonTextReader(sr))
            {
                writer.Formatting = Formatting.Indented;
                //serializer.Serialize(writer, product);
                List<object> list = new List<object>();
                foreach (var obj in objects)
                {
                    list.Add(obj.AsJson());
                }
                // serializer.Serialize(writer, obj.AsJson());
                serializer.Serialize(writer, new { objects = list });

                //return new Level((JObject)serializer.Deserialize(reader));
            }
        }

        public void SaveGroup(List<GameObject> group, string json_path)
        {
            JsonSerializer serializer = new JsonSerializer();
            // using (StreamReader sr = new StreamReader(json_path))
            using (StreamWriter sw = new StreamWriter(json_path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            //using (JsonReader reader = new JsonTextReader(sr))
            {
                writer.Formatting = Formatting.Indented;
                //serializer.Serialize(writer, product);
                List<object> list = new List<object>();

                // var (minx, miny) =
                var origin = getMinCoords(group);

                foreach (var obj in group)
                {
                    var pos = (PositionComponent)obj;
                    pos.local_position.Coords -= origin;
                    list.Add(obj.AsJson());
                    pos.local_position.Coords += origin;
                }
                // serializer.Serialize(writer, obj.AsJson());
                serializer.Serialize(writer, new { objects = list });

                //return new Level((JObject)serializer.Deserialize(reader));
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
