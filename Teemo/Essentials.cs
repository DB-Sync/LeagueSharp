using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Teemo
{
    internal class Essentials
    {
        public static Spell Q;

        public static Spell W;

        public static Spell E;

        public static Spell R;

        public static ShroomTables ShroomPositions;

        public static FileHandler FileHandler;

        public static Orbwalking.Orbwalker Orbwalker;

        public static Menu Config;

        public static Obj_AI_Hero Player => ObjectManager.Player;

        public static int LastR;

        public static bool IsShroomed(Vector3 position)
        {
            return
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(obj => obj.Name == "Noxious Trap")
                    .Any(obj => position.Distance(obj.Position) <= 250);
        }

        public static float RRange => 300*R.Level;

        public static readonly string[] Marksman =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Jinx", "Kalista",
            "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "Twitch", "Urgot", "Varus",
            "Vayne"
        };
    }
}