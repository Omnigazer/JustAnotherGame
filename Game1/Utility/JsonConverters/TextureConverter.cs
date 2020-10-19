using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Omniplatformer.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Utility.JsonConverters
{
    public class TextureConverter : JsonConverter<Texture2D>
    {
        public override Texture2D ReadJson(JsonReader reader, Type objectType, Texture2D existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var val = (string)reader.Value;
            return GameContent.Instance.Load(val);
        }

        public override void WriteJson(JsonWriter writer, Texture2D value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }
    }
}
