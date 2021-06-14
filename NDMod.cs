using Terraria.ModLoader;
using NDMod.Common.Utilities;
using NDMod.Common;
using System.Collections.Generic;
using Terraria;
using System;
using Microsoft.Xna.Framework;
using NDMod.Content.ModPlayers;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework.Input;
using NDMod.Common.Enums;
using System.Linq;
using NDMod.Content.Disasters;
using Terraria.ID;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using Terraria.GameContent;

namespace NDMod
{
    public class NDMod : Mod
    {
        public static ModHotKey GoBackInScrollUI;
        public static List<ModDisaster> ModDisasters { get; private set; } = new List<ModDisaster>();
        public static List<ModDisaster> ModdedDisasters { get; private set; } = new List<ModDisaster>();

        public static UIHelper UIUtils => UIHelper.GetInstance();

        private ScreenShaderData _dataWaveSolar;
        private ScreenShaderData _dataOrangeVignette;
        public override void Load()
        {
            GoBackInScrollUI = RegisterHotKey("Return (Scroll UI)", "Escape");

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> WaveSolarFlare = new Ref<Effect>(GetEffect("Effects/SolarFlare"));
                _dataWaveSolar = new ScreenShaderData(WaveSolarFlare, "SolarFlare");
                Filters.Scene["SolarFlare"] = new Filter(_dataWaveSolar, EffectPriority.VeryHigh);

                Ref<Effect> OrangeVignette = new Ref<Effect>(GetEffect("Effects/OrangeVignette"));
                _dataOrangeVignette = new ScreenShaderData(OrangeVignette, "OrangeVignette");
                Filters.Scene["OrangeVignette"] = new Filter(_dataOrangeVignette, EffectPriority.High);
            }
            darkeningColor = new Color(33, 33, 33);

        }

        private Color darkeningColor;
        private Color _solarFlareBGColor;
        public override void ModifyLightingBrightness(ref float scale)
        {
            scale = 0.85f;
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            _solarFlareBGColor = Color.Orange;
            if (ModContent.GetInstance<SolarFlare>().Active)
            {
                backgroundColor = _solarFlareBGColor;
                tileColor = _solarFlareBGColor;
            }
            else
            {
                _solarFlareBGColor = backgroundColor;
            }
            if (ModContent.GetInstance<Blackout>().Active)
            {
                backgroundColor = darkeningColor;
                tileColor = darkeningColor;

                if (darkeningColor.R > 0)
                {
                    darkeningColor.R--;
                    darkeningColor.G--;
                    darkeningColor.B--;
                }
            }
            if (Blackout.bgStopModifyTimer > 0 && !ModContent.GetInstance<Blackout>().Active)
            {
                tileColor = darkeningColor;
                backgroundColor = darkeningColor;
                if (darkeningColor.R < 150)
                {
                    darkeningColor.R++;
                    darkeningColor.G++;
                    darkeningColor.B++;
                }
            }
        }
        public override void PostSetupContent()
        {
            ContentInstance.Register(new UIHelper());
            ModDisasters = OOPHelper.GetSubclasses<ModDisaster>();
            foreach (ModDisaster disaster in ModDisasters)
            {
                ContentInstance.Register(disaster);
                disaster.Initialize();

                bool notBase = disaster.GetType().Namespace != "NDMod.Content.Disasters";

                if (notBase)
                {
                    ModdedDisasters.Add(disaster);
                }
            }
            IL.Terraria.NPC.AI_007_TownEntities += GoToHomes;
            On.Terraria.Rain.Update += Rain_Update;
            On.Terraria.Main.DrawInterface_30_Hotbar += Main_DrawInterface_30_Hotbar;
        }
        private void Rain_Update(On.Terraria.Rain.orig_Update orig, Rain self)
        {
            self.position += self.velocity;
            if (!Collision.SolidCollision(self.position, 2, 2) && !(self.position.Y > Main.screenPosition.Y + (float)Main.screenHeight + 100f) && !Collision.WetCollision(self.position, 2, 2))
            {
                return;
            }
            self.active = false;
            if (Main.rand.Next(100) < Main.gfxQuality * 100f)
            {
                int num = 154;
                if (self.type == 3 || self.type == 4 || self.type == 5)
                {
                    num = 218;
                }
                var dust = Dust.NewDustDirect(self.position - self.velocity, 2, 2, ModContent.GetInstance<AcidRain>().Active ? DustID.BubbleBurst_Green : num);

                dust.position.X -= 2f;
                dust.alpha = 38;
                dust.velocity *= 0.1f;
                dust.velocity += -self.velocity * 0.025f;
                dust.scale = 0.75f;
                dust.color = Color.Green;
                dust.noLight = true;
            }
        }
        public override void Unload()
        {
            IL.Terraria.NPC.AI_007_TownEntities -= new ILContext.Manipulator(GoToHomes);

            GoBackInScrollUI = null;
        }

        private void GoToHomes(ILContext il)
        {
            /*
                * Header Size: 12 bytes
                * Code Size: 22134 (0x5676) bytes
                * LocalVarSig Token: 0x1100017D RID: 381
                * .maxstack 11
                * .locals init (
                * [0] int32 maxValue,
                * [1] bool flag,
                */

            // bool flag = Main.raining;
            //IL_0006: ldsfld bool Terraria.Main::raining // push Main.raining
            //IL_000b: stloc.1 // assign
            // if (!Main.dayTime) // other checks
            //IL_000c: ldsfld bool Terraria.Main::dayTime
            //IL_0011: brtrue.s IL_0015
            // ...

            ILCursor c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchStloc(1))) // match the 'bool flag = Main.raining' (the assignment)
            {
                // IL failed
                Logger.Warn("IL Patch at Terraria.NPC.AI_007_TownEntities failed.");
                return;
            }

            // the boolean of Main.raining is still on the stack
            c.EmitDelegate<Func<bool, bool>>(raining =>
            {
                return raining || AnyDisasterReturnsNPCHome();
            });
            Logger.Info("IL Patch at Terraria.NPC.AI_007_TownEntities success!");
            // now it would act like 'bool flag = Main.raining || AnyDisasterReturnsNPCHome()'
        }

        private static bool AnyDisasterReturnsNPCHome()
        {
            foreach (ModDisaster disaster in ModDisasters)
            {
                if (disaster.Active && disaster.ShouldTownNPCsGoToHomes)
                {
                    return true;
                }
            }
            return false;
        }

        public override void PreSaveAndQuit()
        {
            Earthquake.SFXICrumble.Volume = 0f;
            AcidRain.SFXISizzle.Volume = 0f;
            CancerPlayer.shadersIntensity = 0f;
            DisasterPlayer.ViewingDisastersScroll = false;
            foreach (ModDisaster disaster in ModDisasters)
            {
                disaster.SaveAndQuit();
            }
        }
        internal int mode;
        private void Main_DrawInterface_30_Hotbar(On.Terraria.Main.orig_DrawInterface_30_Hotbar orig, Main self)
        {
            if (Main.keyState.KeyJustPressed(Keys.P))
            {
                var m = Projectile.NewProjectileDirect(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.VortexPurple>(), 50, 0, Main.myPlayer);

                m.timeLeft = 300;
            }
            orig(self);
            if (Main.mouseRight && Main.mouseRightRelease)
                Projectile.NewProjectileDirect(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.Hail>(), 30, 2);
            List<ModDisaster> disasters = new List<ModDisaster>();
            foreach (ModDisaster d in ModDisasters)
                disasters.Add(d);
            if (Main.keyState.KeyJustPressed(Keys.Up) && mode < disasters.Count - 1)
            {
                Main.PlaySound(SoundID.MenuTick);
                mode++;
            }
            if (Main.keyState.KeyJustPressed(Keys.Down) && mode > 0)
            {
                Main.PlaySound(SoundID.MenuTick);
                mode--;
            }
            if (Main.keyState.KeyJustPressed(Keys.End))
                disasters[mode].End();
            if (Main.keyState.KeyJustPressed(Keys.Home))
                disasters[mode].TryBeginRestartCooldown();

            Main.spriteBatch.DrawString(Main.fontMouseText, $"{disasters[mode].Name} ({disasters[mode].GetType().Name})", new Vector2(8, Main.screenHeight - 40), Color.White);
            if (DisasterPlayer.ViewingDisastersScroll)
            {
                DrawScrollUI();
                DrawChildren_ScrollUI();
                Main.isMouseLeftConsumedByUI = true;
            }
        }

        public override void PostUpdateEverything()
        {
            if (Main.keyState.KeyJustPressed(Keys.P))
            {
                // Main.NewText("Getting liquid pool...");
                // WorldUtils.GetLiquidPool((Main.MouseWorld / 16).ToPoint(), new Point(20, 20), true);
            }
            _dataWaveSolar?.UseIntensity(CancerPlayer.shadersIntensity);
            _dataOrangeVignette?.UseIntensity(CancerPlayer.shadersIntensity);
            foreach (ModDisaster disaster in ModDisasters)
            {
                disaster.Update();

                if (disaster.Active)
                    disaster.UpdateActive();
                else
                    disaster.UpdateInactive();
            }
        }

        private static readonly float[] buttonScales = new float[30];
        internal static ScrollUIMode ScrollUI;
        private static void DrawScrollUI(bool beginAndEnd = false)
        {
            var mod = ModContent.GetInstance<NDMod>();
            var ingameUIPath = "Assets/Textures/UI/IngameUI";
            var scrollBody = mod.GetTexture($"{ingameUIPath}/ScrollBody");
            var scrollLeft = mod.GetTexture($"{ingameUIPath}/ScrollLeft");
            var scrollRight = mod.GetTexture($"{ingameUIPath}/ScrollRight");
            Rectangle body = new Rectangle(CommonUtils.GetScreenCenter().Item1, CommonUtils.GetScreenCenter().Item2, scrollBody.Width, scrollBody.Height); ;
            // TODO: Finish after school lul
            Player player = Main.LocalPlayer;

            var sb = Main.spriteBatch;

            string[] disasterNamesRow1 =
            {
                "Earthquakes",
                "Acid Rain",
                "Cold Fronts",
                "Floods",
                "Solar Flare",
                "Tornadoes",
                "Hurricanes",
                "Thunderstorms",
                "Vortexes",
                "Lava Rain",
                "Sinkholes",
            };
            string[] disasterNamesRow2 =
            {
                "Blackout",
                "Hailstorms",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
                "Coming Soon",
            };

            if (beginAndEnd) sb.Begin();
            sb.Draw(scrollBody, body, null, Color.White, 0f, scrollBody.Size() / 2, default, 0f);

            sb.Draw(scrollRight, new Vector2(body.X + body.Width - scrollBody.Size().X / 2 - 18, body.Y - scrollBody.Size().Y / 2 - 32), null, Color.White, 0f, Vector2.Zero, 1f, default, 0f);
            sb.Draw(scrollLeft, new Vector2(body.X - 78 - scrollBody.Size().X / 2, body.Y - scrollBody.Size().Y / 2 - 32), null, Color.White, 0f, Vector2.Zero, 1f, default, 0f);
            if (ScrollUI == ScrollUIMode.Main)
            {
                int i = 0;
                Color coolColor = new Color();
                foreach (string str in disasterNamesRow1)
                {
                    i++;
                    switch (i)
                    {
                        case 1:
                            coolColor = Color.Green;
                            break;
                        case 2:
                            coolColor = Color.Lime;
                            break;
                        case 3:
                            coolColor = Color.LightSkyBlue;
                            break;
                        case 4:
                            coolColor = Color.DodgerBlue;
                            break;
                        case 5:
                            coolColor = Color.DarkOrange;
                            break;
                        case 6:
                            coolColor = Color.Gray;
                            break;
                        case 7:
                            coolColor = Color.LightBlue;
                            break;
                        case 8:
                            coolColor = Color.Yellow;
                            break;
                        case 9:
                            coolColor = Color.MediumPurple;
                            break;
                        case 10:
                            coolColor = Color.Orange;
                            break;
                        case 11:
                            coolColor = new Color(Color.SaddleBrown.R + 25, Color.SaddleBrown.G + 25, Color.SaddleBrown.B + 25);
                            break;
                    }
                    var buttons = UIUtils.MakeUIButton(new Vector2(body.X - 75, body.Y - 125 + (i - 1) * 25), str,
                        delegate
                        {
                            string adjust = str;
                            if (str != "Tornadoes")
                            {
                                adjust = str.Remove(str.Length - 1, str[str.Length - 1] == 's' ? 1 : 0).Replace(" ", string.Empty); ;
                            }
                            if (str == "Tornadoes")
                                adjust = "Tornado";
                            if (str == "Vortexes")
                                adjust = "Vortex";
                            // Main.NewText(Enum.Parse(typeof(ScrollUIMode), adjust));
                            ScrollUI = (ScrollUIMode)Enum.Parse(typeof(ScrollUIMode), adjust);
                        }, ref buttonScales[i], false, coolColor, 0, null, new Color(coolColor.R - 40, coolColor.G - 40, coolColor.B - 40));
                }
                int j = 0;
                foreach (string str in disasterNamesRow2)
                {
                    j++;
                    switch (j)
                    {
                        case 1:
                            coolColor = Color.DarkGray;
                            break;
                        case 2:
                            coolColor = Color.White;
                            break;
                        case 3:
                            coolColor = Color.White;
                            break;
                        case 4:
                            coolColor = Color.White;
                            break;
                        case 5:
                            coolColor = Color.White;
                            break;
                        case 6:
                            coolColor = Color.White;
                            break;
                        case 7:
                            coolColor = Color.White;
                            break;
                        case 8:
                            coolColor = Color.White;
                            break;
                        case 9:
                            coolColor = Color.White;
                            break;
                        case 10:
                            coolColor = Color.White;
                            break;
                        case 11:
                            coolColor = Color.White;
                            break;
                    }
                    var buttons = UIUtils.MakeUIButton(new Vector2(body.X + 75, body.Y - 125 + (j - 1) * 25), str,
                    delegate
                    {
                        string adjust = str;
                        adjust = str.Remove(str.Length - 1, str[str.Length - 1] == 's' ? 1 : 0).Replace(" ", string.Empty);
                        ScrollUI = Enum.TryParse(adjust, out ScrollUIMode changeTo) ? changeTo : ScrollUIMode.Main;
                        if (str == "Coming Soon")
                            Main.PlaySound(SoundID.Unlock);
                    }, ref buttonScales[i], false, coolColor, 0, null, new Color(coolColor.R - 40, coolColor.G - 40, coolColor.B - 40));
                }
            }
            if (ScrollUI == ScrollUIMode.Main)
            {
                float p = 1f;
                var rarityIndex = UIUtils.MakeUIButton(new Vector2(body.X, body.Y + 155), "Index",
                        delegate
                        {
                            ScrollUI = ScrollUIMode.Index;
                        }, ref p, false);
                if (beginAndEnd) sb.End();
            }
        }
        private static void DrawChildren_ScrollUI(bool beginAndEnd = false)
        {
            string warnings = "";
            if (GoBackInScrollUI.JustPressed)
            {
                ScrollUI = ScrollUIMode.Main;
            }
            (int, int) center = CommonUtils.GetScreenCenter();
            if (beginAndEnd) Main.spriteBatch.Begin();
            #region Explain Strings
            string[] explainEquakes =
            {
                "Earthquakes are all about shaking the earth.",
                "If one occurs, you might want to take cover.",
                "If underground, rocks start falling from above.",
                "Be aware, if the earthquake gets too powerful,",
                "blocks around you will start breaking!",
                "The chance an earthquake will break anything",
                "is rare, but not impossible.",
                "Earthquakes themselves are quite uncommon,",
                "but when they do happen, be careful."
            };
            string[] explainARain =
            {
                "Acid Rain is a very dangerous event.",
                "Anything living will suffer.",
                "If you step out into the rain, your skin will",
                "start burning off, and you will lose life quickly.",
                "The only way to avoid being hurt is to take cover",
                "under any structure.",
                "Be sure that your Town NPCs are somewhere safe",
                "otherwise they will die quickly and most likely",
                "be many casualties.",
            };
            string[] explainCFronts =
            {
                "Cold Fronts are very lengthy events which",
                "chill the player to the bone. Monsters are",
                "immune to these freezing temperatures.",
                "While in a snowy area during these fronts,",
                "you will be slowed down by a very noticeable",
                "amount. This makes you more vulnerable and",
                "become an easy target, making you an easy kill.",
                "During these times, try to avoid the snow."
            };
            string[] explainFloods =
            {
                "Floods are a non-dangerous but very annoying",
                "disaster. As rain comes down, water starts to",
                "build up in the sorrounding area.",
                "When it's flooding, you should be fine if you",
                "have the right accessories, but NPCs may drown",
                "if you don't have your doors shut. Be wary of your",
                "town NPCs otherwise they may die!",
            };
            string[] explainSFlares =
            {
                "Solar Flares in the Terraria world are extremely",
                "deadly anomaly. You should try to steer clear of",
                "them, otherwise you will get easily overwhelmed.",
                "Magic Mirror-like items will not work, and wiring",
                "will fail to work during this time. If you spend",
                "too much time outside, you may get cancer and",
                "lose health indefinitely until you die."
            };
            string[] explainTStorms =
            {
                "Thunderstorms are moderatrely dangerous and",
                "will damage any living thing it touches.",
                "Lightning will occasionally strike down in places",
                "near you. If you have good enough armor, you can",
                "explore a little bit or a lot if you wish.",
                "If not geared sufficiently, try to not go out."
            };
            string[] explainENight =
            {
                ChildSafety.Disabled ? "Damn! Who turned the lights off?" : "Darn! Who turned the lights off?",
                "During these very dark times, it is dangerous",
                "to wander about, as light is minimal and monsters",
                "lurk in the darkness, awating your arival.",
                "In the early game, it is even MORE dangerous.",
                "Try to stay inside or near light."
            };
            string[] explainHStorms =
            {
                "Hailstorms are a very painful storm.",
                "While these storms are ocurring, you must",
                "try to stay inside, as the hail is very large,",
                "not to mention that it hurts!",
                "Depending on your armor, you may want to",
                "stay indoors for the time being."
            };
            string[] explainSinkholes =
            {
                "Sinkholes are a very environmentally damaging",
                "and annoying disaster to deal with",
                "when they happen in the world.",
                "There is no real way to prevent these from",
                "affecting the world. They will never affect",
                "chests or any containers.",
            };

            string[] indexes =
            {
                "Rarities: Very Common, Common, Unusual,",
                "Rare, Very Rare"
            };
            string[] severities =
            {
                "Severity: None, Minimal, Moderate,",
                "Severe, Very Severe"
            };
            string[] durations =
            {
                "Duration: Instant, Very Short, Short,",
                "Medium, Long, Very Long"
            };
            #endregion
            #region Drawing Explain Strings
            switch (ScrollUI)
            {
                case ScrollUIMode.Main:
                    return;
                case ScrollUIMode.Earthquake:
                    int i = 0;
                    warnings = "R: Rare | S: Severe | D: Short"; 
                    foreach (string strArr in explainEquakes)
                    {
                        i++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (i - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.AcidRain:
                    int j = 0;
                    warnings = "R: Rare | S: Severe | D: Medium";
                    foreach (string strArr in explainARain)
                    {
                        j++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (j - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.ColdFront:
                    warnings = "R: Unusual | S: Damaging | D: Very Long";
                    int b = 0;
                    foreach (string strArr in explainCFronts)
                    {
                        b++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (b - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Flood:
                    warnings = "R: Unusual | S: Minimal | D: Long";
                    int z = 0;
                    foreach (string strArr in explainFloods)
                    {
                        z++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (z - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Vortex:
                    break;
                case ScrollUIMode.SolarFlare:
                    int s = 0;
                    warnings = "R: Very Rare | S: Very Severe | D: Long";
                    foreach (string strArr in explainSFlares)
                    {
                        s++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (s - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Thunderstorm:
                    int t = 0;
                    warnings = "R: Unusual | S: Moderate | D: Medium";
                    foreach (string strArr in explainTStorms)
                    {
                        t++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (t   - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Blackout:
                    int g = 0;
                    warnings = "R: Very Rare | S: None | D: Long - Very Long";
                    foreach (string strArr in explainENight)
                    {
                        g++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (g - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Hailstorm:
                    int h = 0;
                    warnings = "R: Unusual | S: Minimal | D: Short";
                    foreach (string strArr in explainHStorms)
                    {
                        h++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (h - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Sinkhole:
                    int v = 0;
                    warnings = "R: Very Rare | S: Very Severe | D: Instant";
                    foreach (string strArr in explainSinkholes)
                    {
                        v++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (v - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
                case ScrollUIMode.Index:
                    int m = 0;
                    foreach (string strArr in indexes)
                    {
                        m++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 125 + (m - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    int n = 0;
                    foreach (string strArr in severities)
                    {
                        n++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 75 + (n - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    int p = 0;
                    foreach (string strArr in durations)
                    {
                        p++;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, strArr, new Vector2(center.Item1, center.Item2 - 25 + (p - 1) * 25), Color.White, 0f, Main.fontDeathText.MeasureString(strArr) / 2, new Vector2(0.3f), -1, 1);
                    }
                    break;
            }
            if (ScrollUI != ScrollUIMode.Main)
            {
                if (GoBackInScrollUI.GetAssignedKeys().Count == 0)
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, $"Please assign a hotkey to return.", new Vector2(center.Item1, center.Item2 + 150), Color.White, 0f, Main.fontDeathText.MeasureString($"Please assign a hotkey to return.") / 2, new Vector2(0.3f), -1, 1);
                else
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, $"Press {GoBackInScrollUI.GetAssignedKeys()[0]} to return", new Vector2(center.Item1, center.Item2 + 150), Color.White, 0f, Main.fontDeathText.MeasureString($"Press {GoBackInScrollUI.GetAssignedKeys()[0]} to return") / 2, new Vector2(0.3f), -1, 1);
                string ind2 = "'R:' indicates rarity\n'S:' indicates severity\n'D:' indicates duration";
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, warnings, new Vector2(center.Item1, center.Item2 + 125), Color.White, 0f, Main.fontDeathText.MeasureString(warnings) / 2, new Vector2(0.3f), -1, 1);
                if (ScrollUI == ScrollUIMode.Index) ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, ind2, new Vector2(center.Item1, center.Item2 + 75), Color.White, 0f, Main.fontDeathText.MeasureString(ind2) / 2, new Vector2(0.3f), -1, 1);
            }
            #endregion
            if (beginAndEnd) Main.spriteBatch.End();
        }
    }
}