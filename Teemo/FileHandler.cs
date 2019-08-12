namespace Teemo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class FileHandler
    {
        #region Fields

        public static List<Vector3> Position = new List<Vector3>();

        private static readonly string ShroomLocation = Config.AppDataDirectory + @"\PandaTeemo\";

        private static string xFile = ShroomLocation + Utility.Map.GetMap().Type + @"\" + "xFile" + ".txt";

        private static string yFile = ShroomLocation + Utility.Map.GetMap().Type + @"\" + "yFile" + ".txt";

        private static string zFile = ShroomLocation + Utility.Map.GetMap().Type + @"\" + "zFile" + ".txt";

        private static string[] xString;

        private static string[] zString;

        private static string[] yString;

        private static int[] xInt;

        private static int[] zInt;

        private static int[] yInt;
        
        #endregion

        #region Methods

        public FileHandler()
        {
            #region Initialize
            DoChecks();
            #endregion
        }

        private static void DoChecks()
        {
            #region Check Missing Files

            if (!Directory.Exists(ShroomLocation))
            {
                Directory.CreateDirectory(ShroomLocation);
                Directory.CreateDirectory(ShroomLocation + Utility.Map.MapType.CrystalScar);
                Directory.CreateDirectory(ShroomLocation + Utility.Map.MapType.HowlingAbyss);
                Directory.CreateDirectory(ShroomLocation + Utility.Map.MapType.SummonersRift);
                Directory.CreateDirectory(ShroomLocation + Utility.Map.MapType.TwistedTreeline);
                Directory.CreateDirectory(ShroomLocation + Utility.Map.MapType.Unknown);
                CreateFile();
            }
            else if (!File.Exists(xFile) || !File.Exists(zFile) || !File.Exists(yFile))
            {
                CreateFile();
            }
            else if (File.Exists(xFile) && File.Exists(zFile) && File.Exists(yFile))
            {
                ConvertToInt();
            }

            #endregion
        }

        private static void CreateFile()
        {
            #region Create File

            if (!File.Exists(xFile))
            {
                File.WriteAllText(xFile, "5020");
            }
            else if (!File.Exists(yFile))
            {
                File.WriteAllText(yFile, "8430");
            }
            else if (!File.Exists(zFile))
            {
                File.WriteAllText(zFile, "2");
            }

            DoChecks();

            #endregion
        }

        public static void GetShroomLocation()
        {
            #region Get Location

            for (var i = 0; i < xInt.Count() && i < yInt.Count() && i < zInt.Count(); i++)
            {
                Position.Add(new Vector3(xInt[i], zInt[i], yInt[i]));
                if (Essentials.Config.SubMenu("Drawing").SubMenu("debug").Item("debugpos").GetValue<bool>())
                {
                    Game.PrintChat(Position[i].ToString());
                }
            }

            #endregion
        }

        private static void ConvertToInt()
        {
            #region Convert to Int

            xString = new string[File.ReadAllLines(xFile).Count()];
            yString = new string[File.ReadAllLines(yFile).Count()];
            zString = new string[File.ReadAllLines(zFile).Count()];

            xInt = new int[File.ReadAllLines(xFile).Count()];
            yInt = new int[File.ReadAllLines(yFile).Count()];
            zInt = new int[File.ReadAllLines(zFile).Count()];

            xString = File.ReadAllLines(xFile);
            yString = File.ReadAllLines(yFile);
            zString = File.ReadAllLines(zFile);

            for (var i = 0; i < xString.Count(); i++)
            {
                xInt[i] = Convert.ToInt32(xString[i]);
            }

            for (var i = 0; i < xString.Count(); i++)
            {
                zInt[i] = Convert.ToInt32(zString[i]);
            }

            for (var i = 0; i < xString.Count(); i++)
            {
                yInt[i] = Convert.ToInt32(yString[i]);
            }

            GetShroomLocation();

            if (Essentials.Config.SubMenu("Drawing").SubMenu("debug").Item("debugpos").GetValue<bool>())
            {
                Game.PrintChat("Sucessfully Initialized FileHandler");
            }

            #endregion
        }

        #endregion
    }
}