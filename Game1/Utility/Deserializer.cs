using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility
{
    public class Deserializer
    {
        Dictionary<Guid, GameObject> storage;
        JObject json;

        /*
        Deserialzer(filename)
        {
            // storage = new ...
            // json = readFromFile;
        }
        */

        public Deserializer(JObject json, Dictionary<Guid, GameObject> storage)
        {
            //Assign
            this.json = json;
            this.storage = storage;
        }

        /*
        int decodeInt(String key)
        {

        }
        int decodeString(String key)
        {
        }
        int decodeFloat(String key)
        {
        }
        */

        public JObject getData()
        {
            return json;
        }

        public Object decodeObject(JObject inner)
        {
            var id_str = inner["Id"];
            Guid id = Guid.Empty;
            GameObject obj;
            if (id_str != null)
            {
                id = Guid.Parse(inner["Id"].ToString());
                if (storage.ContainsKey(id))
                {
                    return storage[id];
                }
            }
            Type type = Type.GetType(inner["type"].ToString());

            if (type == typeof(SolidPlatform))
                obj = SolidPlatform.FromJson(new Deserializer(inner, storage));
            else if (type == typeof(BackgroundQuad))
                obj = BackgroundQuad.FromJson(new Deserializer(inner, storage));
            else
                obj = (GameObject)type.GetMethod("FromJson").Invoke(null, new object[] { new Deserializer(inner, storage) });
            if (id != Guid.Empty)
                obj.Id = id;
            storage.Add(obj.Id, obj);
            return obj;
        }

        public Object decodeObject(String key)
        {
            JObject inner = (JObject)json[key];
            return decodeObject(inner);
        }
    }

    /*
    public class SomeClass
    {
        SomeClass(Deserialzer deserializer)
        {
            fieldA = deserializer.decodeInt("int");
            fieldB = deserializer.decodeObject("object");
        }

        static SomeClass method(Deserialzer deserializer)
        {
            return new SomeClass(deserializer);
        }
    }
    */
}
