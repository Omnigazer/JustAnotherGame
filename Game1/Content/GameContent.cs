﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Omniplatformer.Services;
using Omniplatformer.Utility;

namespace Omniplatformer.Content
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
        private ContentManager Content { get; set; }

        public Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();

        public static void Init(ContentManager content)
        {
            Instance = new GameContent(content);
        }

        public Texture2D Load(string path)
        {
            if (path == null)
                return null;
            if (!TextureCache.ContainsKey(path))
            {
                TextureCache.Add(path, Content.Load<Texture2D>(path));
            }
            return TextureCache[path];
        }

        private GameContent(ContentManager content)
        {
            Content = content;
            //load images
            atlas = Content.Load<Texture2D>("Textures/atlas");
            atlas_meta = AtlasMetaImporter.NewImportTileMetadata("Content/Textures/atlas.atlas");
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
    }
}
