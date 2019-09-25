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

        public Object decodeToken(JToken token)
        {
            if(token.Type == JTokenType.Object)
            {
                return decodeObject((JObject)token);
            }
            else if (token.Type == JTokenType.Null)
            {
                return null;
            }
            return null;
        }

        public Object decodeObject(JObject inner)
        {
            var id_str = inner["Id"];
            Guid id = Guid.Empty;
            if (id_str != null)
            {
                id = Guid.Parse(inner["Id"].ToString());
                if (storage.ContainsKey(id))
                {
                    return storage[id];
                }
            }
            Type type = Type.GetType(inner["type"].ToString());
            object obj = type.GetMethod("FromJson").Invoke(null, new object[] { new Deserializer(inner, storage) });

            if (id != Guid.Empty)
            {
                GameObject gobj = (GameObject)obj;
                gobj.Id = id;
                storage.Add(gobj.Id, gobj);
            }


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
