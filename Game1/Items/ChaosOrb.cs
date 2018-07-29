using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Utility;
using Newtonsoft.Json.Linq;

namespace Omniplatformer
{
    public class ChaosOrb : GameObject
    {
        // public int Damage { get; set; }
        // public Vector2 Knockback { get; set; }
        // public override GameObject Source => _source?.Source ?? this;
        // private GameObject _source;

        public ChaosOrb(Texture2D texture = null)
        {
            if (texture == null)
                texture = GameContent.Instance.cursor;
            Solid = false;
            Team = Team.Friend;
            // Damage = damage;
            var halfsize = new Vector2(3, 25);
            Components.Add(new PositionComponent(this, Vector2.Zero, halfsize, 0, new Vector2(0.5f, 0.1f)));
            Components.Add(new RenderComponent(this, Color.White, texture));
        }

        public override object AsJson()
        {
            return new
            {
                Id,
                type = GetType().AssemblyQualifiedName
            };
        }

        /*
        public static GameObject FromJson(Deserializer deserializer)
        {
            // var item = new WieldedItem((int)data["damage"]) { Id = id };
            // var data = deserializer.getData();
            // var item = new WieldedItem((int)data["damage"]);
            // SerializeService.Instance.RegisterObject(item);
            // return item;
        }
        */
    }
}
