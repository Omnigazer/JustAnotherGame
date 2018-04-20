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
        public Texture2D characterLeft { get; set; }
        public Texture2D characterRight { get; set; }
        public Texture2D cursor { get; set; }
        public Texture2D whitePixel { get; set; }
        public Texture2D greenPixel { get; set; }
        public Texture2D bolt { get; set; }
        public Texture2D testLiquid { get; set; }
        public Texture2D causticsMap { get; set; }
        public Texture2D ladder { get; set; }

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

        public Level level = new Level();

        public static GameContent Instance { get; private set; }
        ContentManager Content { get; set; }

        public static void Init(ContentManager content)
        {
            Instance = new GameContent(content);
        }

        private GameContent(ContentManager Content)
        {
            this.Content = Content;
            //load images
            characterLeft = Content.Load<Texture2D>("Textures/char-left");
            characterRight = Content.Load<Texture2D>("Textures/char-right");
            cursor = Content.Load<Texture2D>("Textures/cursor2");
            bolt = Content.Load<Texture2D>("Textures/bolt");
            testLiquid = Content.Load<Texture2D>("Textures/testliquid");
            causticsMap = Content.Load<Texture2D>("Textures/caustics_atlas");
            ladder = Content.Load<Texture2D>("Textures/ladder");
            alphaMask = Content.Load<Texture2D>("Textures/alphaMask");
            lightMask = Content.Load<Texture2D>("Textures/lightMask");
            healthBarLightMask = Content.Load<Texture2D>("Textures/healthBarlightMask");
            distortMask = Content.Load<Texture2D>("Textures/distortmask");

            // vampireKiller = Content.Load<Song>("castlevania");
            Songs.Add(Content.Load<Song>("Music/castlevania"));
            Songs.Add(Content.Load<Song>("Music/starker_tower"));
            Songs.Add(Content.Load<Song>("Music/wicked_child"));
            Songs.Add(Content.Load<Song>("Music/heart_of_fire"));
            Songs.Add(Content.Load<Song>("Music/walking_on_the_edge"));


            // TEST ZONE


            MultiplyEffect = Content.Load<Effect>("Effects/lighteffect");
            AdditiveEffect = Content.Load<Effect>("Effects/additiveeffect");
            // AlphaEffect = Content.Load<Effect>("alphaeffect");
            DistortEffect = Content.Load<Effect>("Effects/distorteffect");
            BlurEffect = Content.Load<Effect>("Effects/blureffect");

            whitePixel = new Texture2D(GraphicsService.Instance.GraphicsDevice, 1, 1);
            whitePixel = new Texture2D(GraphicsService.Instance.GraphicsDevice, 2, 2);

            // Create a 1D array of color data to fill the pixel texture with.
            Color[] colorData = {
                    // Color.White
                    Color.White, Color.White, Color.White, Color.White
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

        public Level LoadJson(string json_path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(json_path))
            // using (JsonWriter writer = new JsonTextWriter(sw))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                // serializer.Serialize(writer, product);

                return new Level((JObject)serializer.Deserialize(reader));
            }
        }
    }
}
