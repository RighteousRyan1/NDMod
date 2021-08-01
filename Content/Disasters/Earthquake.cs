using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Terraria.ModLoader;
using NDMod.Content.ModPlayers;
using NDMod.Common;
using NDMod.Common.Utilities;

namespace NDMod.Content.Disasters
{
    public class Earthquake : ModDisaster
    {
        public static SoundEffect SFXCrumble;
        public static SoundEffectInstance SFXICrumble;
        public static float quakeSeverity;
        public override int MaxDuration => 5500;
        public override float ChanceToOccur => 0.001f;
        public override int MinDuration => 4500;
        /// <summary>
        /// What the quake scale attempts to crawl to via steps.
        /// </summary>
        private float _achieve;
        public override void UpdateActive()
        {
            // Main.NewText(duration);
            Player player = Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().Player;
            // Activity is handled in a ModPlayer.

            bool getUnusuallyHighQuakeScale = Main.rand.NextFloat() < 0.175f;
            if (Main.GameUpdateCount % 150 == 0)
            {
                if (getUnusuallyHighQuakeScale) {
                    _achieve = Main.rand.NextFloat(10, 20);
                    //Main.NewText("Warning, Going to " + _achieve);
                }
                else {
                    _achieve = Main.rand.NextFloat(0, 10);
                    //Main.NewText("Safe, Going to " + _achieve);
                }
            }
            MathUtils.RoughStep(ref quakeSeverity, _achieve, 0.05f);
            // Main.NewText($"Severe: {quakeSeverity}");
            if (Math.Abs(quakeSeverity) >= 15)
            {
                float randF = Main.rand.NextFloat();
                if (randF <= 0.025)
                {
                    for (int x = (int)player.Center.X / 16 - 40; x < (int)player.Center.X / 16 + 40; x++)
                    {
                        for (int y = (int)player.Center.Y / 16 - 40; y < (int)player.Center.Y / 16 + 40; y++)
                        {
                            bool chooseBreak = Main.rand.NextFloat() < (0.035f * (quakeSeverity / 8));

                            if (chooseBreak)
                            {
                                player.PickTile(x, y, (int)Math.Round(quakeSeverity) * 5);
                            }
                        }
                    }
                }
            }
        }
        public override string Name => "Earthquake";
        public override int Cooldown => 1500;
        public override void UpdateInactive()
        {
            _achieve = 0;
            MathUtils.RoughStep(ref quakeSeverity, _achieve, 0.05f);
        }
        public override void UpdateAlways()
        {
            float rand1 = Main.rand.NextFloat(0, quakeSeverity);
            float rand2 = Main.rand.NextFloat(0, quakeSeverity);
            float d = Vector2.Distance(Main.screenPosition, Main.screenPosition + new Vector2(rand1, rand2));
            SFXICrumble.Volume = Math.Abs(quakeSeverity / 20f) * Main.soundVolume;
            if (!Active && SFXICrumble.Volume < 0.03f)
                SFXICrumble.Volume = 0f;
            SFXICrumble.Volume = MathHelper.Clamp(SFXICrumble.Volume, 0f, 1f);
        }
        public override void Initialize()
        {
            var mod = ModContent.GetInstance<NDMod>();
            if (!Main.dedServ)
            {
                SFXCrumble = mod.GetSound("Assets/SoundEffects/Quake");
                SFXICrumble = SFXCrumble.CreateInstance();
                SFXICrumble.IsLooped = true;
                SFXICrumble.Play();
                SFXICrumble.Volume = 0f;
            }
        }
        public override bool ShouldTownNPCsGoToHomes => true;
        public override bool CanActivate => ModContent.GetInstance<DisasterConfig>().disasterEnabled_Earthquakes;
    }
}