using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class GameContent
    {
        public Texture2D character { get; set; }
        public Texture2D cursor { get; set; }
        public Texture2D whitePixel { get; set; }
        public Texture2D greenPixel { get; set; }
        public Texture2D bolt { get; set; }
        public Texture2D chaos_orb { get; set; }
        public Texture2D testLiquid { get; set; }
        public Texture2D causticsMap { get; set; }
        public Texture2D ladder { get; set; }
        public Texture2D shield { get; set; }
        public Texture2D testTile { get; set; }
        public Texture2D backgroundTile { get; set; }
        public Texture2D background { get; set; }
        public Texture2D boulder { get; set; }

        public Texture2D atlas { get; set; }
        public Dictionary<short, Rectangle> atlas_meta { get; set; }

        public SoundEffect startSound { get; set; }
        public SpriteFont defaultFont { get; set; }

        public Texture2D alphaMask { get; set; }
        public Texture2D lightMask { get; set; }
        public Texture2D healthBarLightMask { get; set; }
        public Texture2D distortMask { get; set; }

        public Effect MultiplyEffect { get; set; }
        public Effect AdditiveEffect { get; set; }
        public Effect DistortEffect { get; set; }
        public Effect BlurEffect { get; set; }

        // public Song vampireKiller { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();

        public static GameContent Instance { get; private set; }
        ContentManager Content { get; set; }

        public static void Init(ContentManager content)
        {
            Instance = new GameContent(content);
        }

        public Dictionary<short, Rectangle> ImportTileMetadata(string path)
        {
            var meta = new Dictionary<short, Rectangle>();
            using (StreamReader sr = new StreamReader(path))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                var data = (JObject)serializer.Deserialize(reader);
                foreach (JProperty token in data.Children())
                {
                    var obj = token.Value;
                    var location = new Point(obj["location"]["x"].Value<int>(), obj["location"]["y"].Value<int>());
                    var size = new Point(obj["size"]["x"].Value<int>(), obj["size"]["y"].Value<int>());
                    meta.Add(short.Parse(token.Name), new Rectangle(location, size));
                }
            }

            return meta;
        }

        private GameContent(ContentManager Content)
        {
            this.Content = Content;
            //load images
            atlas = Content.Load<Texture2D>("Textures/atlas");
            atlas_meta = ImportTileMetadata("Content/test.meta");
            background = Content.Load<Texture2D>("Textures/background0");
            testTile = Content.Load<Texture2D>("Textures/test_tile");
            boulder = Content.Load<Texture2D>("Textures/boulder");
            backgroundTile = Content.Load<Texture2D>("Textures/background_tile");
            character = Content.Load<Texture2D>("Textures/character");
            cursor = Content.Load<Texture2D>("Textures/cursor2");
            bolt = Content.Load<Texture2D>("Textures/bolt");
            chaos_orb = Content.Load<Texture2D>("Textures/chaos_orb");
            testLiquid = Content.Load<Texture2D>("Textures/testliquid");
            causticsMap = Content.Load<Texture2D>("Textures/caustics_atlas");
            ladder = Content.Load<Texture2D>("Textures/ladder");
            shield = Content.Load<Texture2D>("Textures/shield");
            alphaMask = Content.Load<Texture2D>("Textures/alphaMask");
            lightMask = Content.Load<Texture2D>("Textures/lightMask");
            healthBarLightMask = Content.Load<Texture2D>("Textures/healthBarlightMask");
            distortMask = Content.Load<Texture2D>("Textures/distortmask");
            // TEST ZONE


            MultiplyEffect = Content.Load<Effect>("Effects/lighteffect");
            AdditiveEffect = Content.Load<Effect>("Effects/additiveeffect");
            // AlphaEffect = Content.Load<Effect>("alphaeffect");
            DistortEffect = Content.Load<Effect>("Effects/distorteffect");
            BlurEffect = Content.Load<Effect>("Effects/blureffect");

            whitePixel = new Texture2D(GraphicsService.Instance.GraphicsDevice, 1, 1);

            // Create a 1D array of color data to fill the pixel texture with.
            Color[] colorData = {
                    Color.White
                };
            whitePixel.SetData(colorData);

            greenPixel = new Texture2D(GraphicsService.Instance.GraphicsDevice, 1, 1);
            Color[] colorData2 = {
                    Color.Green,
                };
            greenPixel.SetData(colorData2);
            //load sounds
            // startSound = Content.Load<SoundEffect>("StartSound");

            //load fonts
            defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");
        }

        public void LoadLevel()
        {
            // level = LoadJson(Path.Combine(Content.RootDirectory, "Data", @"json.txt"));
        }
    }
}
