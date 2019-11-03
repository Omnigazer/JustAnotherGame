using Omniplatformer.Objects.Characters;

namespace Omniplatformer.Services
{
    public class GameService
    {
        public static Game1 Instance { get; set; }

        public static void Init(Game1 game)
        {
            Instance = game;
        }

        public static Player Player => Instance.Player;
        // public static List<Character> Characters => Instance.characters;
        // public static List<GameObject> Objects => Instance.objects;
    }
}
