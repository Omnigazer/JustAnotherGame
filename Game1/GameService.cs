using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
{
    public class GameService
    {
        public static Game1 Instance { get; set; }

        public static void Init(Game1 game)
        {
            Instance = game;
        }

        public static Player Player => Instance.player;
        public static List<Character> Characters => Instance.characters;
        public static List<GameObject> Platforms => Instance.platforms;
    }
}
