namespace Teemo
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Color = System.Drawing.Color;

    /// Made by DBSync

    internal class Program
    {
        private const string ChampionName = "Teemo";

        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Essentials.Player.CharData.BaseSkinName != ChampionName)
            {
                return;
            }

            Essentials.Q = new Spell(SpellSlot.Q, 680);
            Essentials.W = new Spell(SpellSlot.W);
            Essentials.E = new Spell(SpellSlot.E);
            Essentials.R = new Spell(SpellSlot.R, 300);

            Essentials.Q.SetTargetted(0.5f, 1500f);
            Essentials.R.SetSkillshot(0.5f, 120f, 1000f, false, SkillshotType.SkillshotCircle);

            Essentials.Config = new Menu("Teemo", "Teemo", true);

            var orbwalking = Essentials.Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            var combo = Essentials.Config.AddSubMenu(new Menu("Combo", "Combo"));
            var harass = Essentials.Config.AddSubMenu(new Menu("Harass", "Harass"));
            var laneclear = Essentials.Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            var jungleclear = Essentials.Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            var ks = Essentials.Config.AddSubMenu(new Menu("KillSteal", "KSMenu"));
            var flee = Essentials.Config.AddSubMenu(new Menu("Flee Menu", "Flee"));
            var drawing = Essentials.Config.AddSubMenu(new Menu("Drawing", "Drawing"));
            var interrupt = Essentials.Config.AddSubMenu(new Menu("Interrupt / Gapcloser", "Interrupt"));
            var misc = Essentials.Config.AddSubMenu(new Menu("Misc", "Misc"));

            Essentials.Orbwalker = new Orbwalking.Orbwalker(orbwalking);

            combo.AddItem(new MenuItem("qcombo", "Use Q in Combo").SetValue(true));
            combo.AddItem(new MenuItem("wcombo", "Use W in Combo").SetValue(true));
            combo.AddItem(new MenuItem("rcombo", "Kite with R in Combo").SetValue(true));
            combo.AddItem(new MenuItem("useqADC", "Use Q only on ADC during Combo").SetValue(false));
            combo.AddItem(new MenuItem("wCombat", "Use W if enemy is in range only").SetValue(false));
            combo.AddItem(new MenuItem("rCharge", "Charges of R before using R").SetValue(new Slider(2, 1, 3)));
            combo.AddItem(
                new MenuItem("checkCamo", "Prevents combo being activated while stealth in brush").SetValue(false));

            harass.AddItem(new MenuItem("qharass", "Harass with Q").SetValue(true));

            laneclear.AddItem(new MenuItem("qclear", "LaneClear with Q").SetValue(true));
            laneclear.AddItem(new MenuItem("qManaManager", "Q Mana Manager").SetValue(new Slider(75)));
            laneclear.AddItem(new MenuItem("rclear", "LaneClear with R").SetValue(true));
            laneclear.AddItem(new MenuItem("userKill", "Use R only if Killable").SetValue(true));
            laneclear.AddItem(new MenuItem("minionR", "Minion for R").SetValue(new Slider(3, 1, 4)));

            jungleclear.AddItem(new MenuItem("qclear", "JungleClear with Q").SetValue(true));
            jungleclear.AddItem(new MenuItem("rclear", "JungleClear with R").SetValue(true));

            interrupt.AddItem(new MenuItem("intq", "Interrupt with Q").SetValue(true));
            interrupt.AddItem(
                new MenuItem("intChance", "Danger Level before using Q").SetValue(
                    new StringList(new[] {"High", "Medium", "Low"})));
            interrupt.AddItem(new MenuItem("gapR", "Gapclose with R").SetValue(true));

            ks.AddItem(new MenuItem("KSQ", "KillSteal with Q").SetValue(true));
            ks.AddItem(new MenuItem("KSR", "KillSteal with R").SetValue(true));

            drawing.AddItem(new MenuItem("drawQ", "Draw Q Range").SetValue(false));
            drawing.AddItem(new MenuItem("drawR", "Draw R Range").SetValue(false));
            drawing.AddItem(new MenuItem("colorBlind", "Colorblind Mode").SetValue(false));
            drawing.AddItem(new MenuItem("drawautoR", "Draw Important Shroom Areas").SetValue(true));
            drawing.AddItem(new MenuItem("DrawVision", "Shroom Vision").SetValue(new Slider(1500, 2500, 1000)));

            var debug = drawing.AddSubMenu(new Menu("Debug", "debug"));
            debug.AddItem(new MenuItem("debugdraw", "Draw Coords").SetValue(false));
            debug.AddItem(new MenuItem("x", "Where to draw X").SetValue(new Slider(500, 0, 1920)));
            debug.AddItem(new MenuItem("y", "Where to draw Y").SetValue(new Slider(500, 0, 1080)));
            debug.AddItem(new MenuItem("debugpos", "Draw Custom Shroom Locations Coordinates").SetValue(true));

            flee.AddItem(new MenuItem("fleetoggle", "Flee").SetValue(new KeyBind(65, KeyBindType.Press)));
            flee.AddItem(new MenuItem("w", "Use W while Flee").SetValue(true));
            flee.AddItem(new MenuItem("r", "Use R while Flee").SetValue(true));
            flee.AddItem(new MenuItem("rCharge", "Charges of R before using R").SetValue(new Slider(2, 1, 3)));

            misc.AddItem(new MenuItem("autoQ", "Automatic Q").SetValue(false));
            misc.AddItem(new MenuItem("autoW", "Automatic W").SetValue(false));
            misc.AddItem(new MenuItem("autoR", "Auto Place Shrooms in Important Places").SetValue(true));
            misc.AddItem(
                new MenuItem("rCharge", "Charges of R before using R in AutoShroom").SetValue(new Slider(2, 1, 3)));
            misc.AddItem(new MenuItem("autoRPanic", "Panic Key for Auto R").SetValue(new KeyBind(84, KeyBindType.Press)));
            misc.AddItem(
                new MenuItem("customLocation", "Use Custom Location for Auto Shroom (Requires Reload)").SetValue(true));
            misc.AddItem(new MenuItem("packets", "Use Packets").SetValue(false));
            misc.AddItem(new MenuItem("checkAA", "Subtract Range for Q (checkAA)").SetValue(true));
            misc.AddItem(
                new MenuItem("checkaaRange", "How many to subtract from Q Range (checkAA)").SetValue(new Slider(100, 0,
                    180)));

            Essentials.Config.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += ActiveStates.OnUpdate;
            Interrupter2.OnInterruptableTarget += Interrupter_OnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Orbwalking.AfterAttack += OrbwalkingAfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            Game.PrintChat("<font color = '#01DF3A'>Teemo v1.7.7.0 Loaded</font>");

            Essentials.FileHandler = new FileHandler();
            Essentials.ShroomPositions = new ShroomTables();
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name.ToLower() == "teemorcast")
            {
                Essentials.LastR = Environment.TickCount;
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var gapR = Essentials.Config.SubMenu("Interrupt").Item("gapR").GetValue<bool>();

            if (gapR && gapcloser.Sender.IsValidTarget() && gapcloser.Sender.IsFacing(Essentials.Player) &&
                gapcloser.Sender.IsTargetable)
            {
                Essentials.R.Cast(gapcloser.Sender.Position);
            }
        }

        private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            var useQCombo = Essentials.Config.SubMenu("Combo").Item("qcombo").GetValue<bool>();
            var useQHarass = Essentials.Config.SubMenu("Harass").Item("qharass").GetValue<bool>();
            var targetAdc = Essentials.Config.SubMenu("Combo").Item("useqADC").GetValue<bool>();
            var checkAa = Essentials.Config.SubMenu("Misc").Item("checkAA").GetValue<bool>();
            var checkaaRange = (float) Essentials.Config.SubMenu("Misc").Item("checkaaRange").GetValue<Slider>().Value;
            var t = target as Obj_AI_Hero;

            if (t != null && Essentials.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (checkAa)
                {
                    if (targetAdc)
                    {
                        if (Essentials.Marksman.Contains(t.CharData.BaseSkinName) && useQCombo && Essentials.Q.IsReady() && Essentials.Q.IsInRange(t, -checkaaRange))
                        {
                            Essentials.Q.Cast(t);
                        }
                    }
                    else
                    {
                        if (useQCombo && Essentials.Q.IsReady() && Essentials.Q.IsInRange(t, -checkaaRange))
                        {
                            Essentials.Q.Cast(t);
                        }
                    }
                }
                else
                {
                    if (targetAdc)
                    {
                        if (Essentials.Marksman.Contains(t.CharData.BaseSkinName) && useQCombo && Essentials.Q.IsReady() && Essentials.Q.IsInRange(t))
                        {
                            Essentials.Q.Cast(t);
                        }
                    }
                    else
                    {
                        if (useQCombo && Essentials.Q.IsReady() && Essentials.Q.IsInRange(t))
                        {
                            Essentials.Q.Cast(t);
                        }
                    }
                }
            }

            if (t != null && Essentials.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                if (checkAa)
                {
                    if (useQHarass && Essentials.Q.IsReady() && Essentials.Q.IsInRange(t, -checkaaRange))
                    {
                        Essentials.Q.Cast(t);
                    }
                }
                else
                {
                    if (useQHarass && Essentials.Q.IsReady() && Essentials.Q.IsInRange(t))
                    {
                        Essentials.Q.Cast(t);
                    }
                }
            }
        }

        private static void Interrupter_OnPossibleToInterrupt(
            Obj_AI_Hero sender,
            Interrupter2.InterruptableTargetEventArgs args)
        {
            var intq = Essentials.Config.SubMenu("Interrupt").Item("intq").GetValue<bool>();
            var intChance = Essentials.Config.SubMenu("Interrupt").Item("intChance").GetValue<StringList>().SelectedValue;
            if (intChance == "High" && intq && Essentials.Q.IsReady() && args.DangerLevel == Interrupter2.DangerLevel.High)
            {
                if (sender != null)
                {
                    Essentials.Q.Cast(sender);
                }
            }
            else if (intChance == "Medium" && intq && Essentials.Q.IsReady() && args.DangerLevel == Interrupter2.DangerLevel.Medium)
            {
                if (sender != null)
                {
                    Essentials.Q.Cast(sender);
                }
            }
            else if (intChance == "Low" && intq && Essentials.Q.IsReady() && args.DangerLevel == Interrupter2.DangerLevel.Low)
            {
                if (sender != null)
                {
                    Essentials.Q.Cast(sender);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Essentials.R = new Spell(SpellSlot.R, Essentials.RRange);

            if (Essentials.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                StateManager.Combo();
            }
            if (Essentials.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                StateManager.LaneClear();
                StateManager.JungleClear();
            }

            if (Essentials.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None && Essentials.Config.SubMenu("Flee").Item("fleetoggle").IsActive())
            {
                StateManager.Flee();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Essentials.Config.SubMenu("Drawing").SubMenu("debug").Item("debugdraw").GetValue<bool>())
            {
                Drawing.DrawText(Essentials.Config.SubMenu("Drawing").SubMenu("debug").Item("x").GetValue<Slider>().Value, Essentials.Config.SubMenu("Drawing").SubMenu("debug").Item("y").GetValue<Slider>().Value,
                    Color.Red, Essentials.Player.Position.ToString());
            }

            var drawQ = Essentials.Config.SubMenu("Drawing").Item("drawQ").GetValue<bool>();
            var drawR = Essentials.Config.SubMenu("Drawing").Item("drawR").GetValue<bool>();
            var colorBlind = Essentials.Config.SubMenu("Drawing").Item("colorBlind").GetValue<bool>();
            var player = ObjectManager.Player.Position;

            if (drawQ && colorBlind)
            {
                Render.Circle.DrawCircle(player, Essentials.Q.Range, Essentials.Q.IsReady() ? Color.YellowGreen : Color.Red);
            }

            if (drawQ && !colorBlind)
            {
                Render.Circle.DrawCircle(player, Essentials.Q.Range, Essentials.Q.IsReady() ? Color.LightGreen : Color.Red);
            }

            if (drawR && colorBlind)
            {
                Render.Circle.DrawCircle(player, Essentials.R.Range, Essentials.R.IsReady() ? Color.YellowGreen : Color.Red);
            }

            if (drawR && !colorBlind)
            {
                Render.Circle.DrawCircle(player, Essentials.R.Range, Essentials.R.IsReady() ? Color.LightGreen : Color.Red);
            }

            var drawautoR = Essentials.Config.SubMenu("Drawing").Item("drawautoR").GetValue<bool>();

            if (drawautoR && Utility.Map.GetMap().Type == Utility.Map.MapType.SummonersRift && Essentials.ShroomPositions.SummonersRift.Any())
            {
                foreach (
                    var place in
                        Essentials.ShroomPositions.SummonersRift.Where(
                            pos =>
                                pos.Distance(ObjectManager.Player.Position) <= Essentials.Config.SubMenu("Drawing").Item("DrawVision").GetValue<Slider>().Value))
                {
                    if (colorBlind)
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.YellowGreen);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(place, 100, Essentials.IsShroomed(place) ? Color.Red : Color.LightGreen);
                    }
                }
            }
            else if (drawautoR && Utility.Map.GetMap().Type == Utility.Map.MapType.CrystalScar && Essentials.ShroomPositions.CrystalScar.Any())
            {
                foreach (
                    var place in
                        Essentials.ShroomPositions.CrystalScar.Where(
                            pos =>
                                pos.Distance(ObjectManager.Player.Position) <= Essentials.Config.SubMenu("Drawing").Item("DrawVision").GetValue<Slider>().Value))
                {
                    if (colorBlind)
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.YellowGreen);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.LightGreen);
                    }
                }
            }
            else if (drawautoR && Utility.Map.GetMap().Type == Utility.Map.MapType.HowlingAbyss && Essentials.ShroomPositions.HowlingAbyss.Any())
            {
                foreach (
                    var place in
                        Essentials.ShroomPositions.HowlingAbyss.Where(
                            pos =>
                                pos.Distance(ObjectManager.Player.Position) <= Essentials.Config.SubMenu("Drawing").Item("DrawVision").GetValue<Slider>().Value))
                {
                    if (colorBlind)
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.YellowGreen);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.LightGreen);
                    }
                }
            }
            else if (drawautoR && Utility.Map.GetMap().Type == Utility.Map.MapType.TwistedTreeline && Essentials.ShroomPositions.TwistedTreeline.Any())
            {
                foreach (
                    var place in
                        Essentials.ShroomPositions.TwistedTreeline.Where(
                            pos =>
                                pos.Distance(ObjectManager.Player.Position) <= Essentials.Config.SubMenu("Drawing").Item("DrawVision").GetValue<Slider>().Value))
                {
                    if (colorBlind)
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.YellowGreen);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.LightGreen);
                    }
                }
            }
            else if (drawautoR && Utility.Map.GetMap().Type == Utility.Map.MapType.Unknown && Essentials.ShroomPositions.ButcherBridge.Any())
            {
                foreach (
                    var place in
                        Essentials.ShroomPositions.ButcherBridge.Where(
                            pos =>
                                pos.Distance(ObjectManager.Player.Position) <= Essentials.Config.SubMenu("Drawing").Item("DrawVision").GetValue<Slider>().Value))
                {
                    if (colorBlind)
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.YellowGreen);
                    }
                    else
                    {
                        Render.Circle.DrawCircle(place, 100,
                            Essentials.IsShroomed(place) ? Color.Red : Color.LightGreen);
                    }
                }
            }
        }
    }
}