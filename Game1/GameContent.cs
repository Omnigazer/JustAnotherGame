using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public static GameContent Instance { get; private set; }

        public static void Init(ContentManager content)
        {
            Instance = new GameContent(content);
        }

        private GameContent(ContentManager Content)
        {
            //load images
            characterLeft = Content.Load<Texture2D>("char-left");
            characterRight = Content.Load<Texture2D>("char-right");
            cursor = Content.Load<Texture2D>("cursor2");
            bolt = Content.Load<Texture2D>("bolt");
            testLiquid = Content.Load<Texture2D>("testliquid");
            causticsMap = Content.Load<Texture2D>("caustics_atlas");


            // TEST ZONE
            alphaMask = Content.Load<Texture2D>("alphaMask");
            lightMask = Content.Load<Texture2D>("lightMask");
            healthBarLightMask = Content.Load<Texture2D>("healthBarlightMask");
            distortMask = Content.Load<Texture2D>("distortmask");

            MultiplyEffect = Content.Load<Effect>("lighteffect");
            AdditiveEffect = Content.Load<Effect>("additiveeffect");
            // AlphaEffect = Content.Load<Effect>("alphaeffect");
            DistortEffect = Content.Load<Effect>("distorteffect");
            BlurEffect = Content.Load<Effect>("blureffect");

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
            defaultFont = Content.Load<SpriteFont>("DefaultFont");
        }
    }
}
