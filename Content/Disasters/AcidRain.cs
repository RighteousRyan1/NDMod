using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ID;
using NDMod.Common;
using Terraria.ModLoader;
using NDMod.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace NDMod.Content.Disasters
{
    public class AcidRain : ModDisaster
    {
        public static SoundEffect SFXSizzle;
        public static SoundEffectInstance SFXISizzle;
        public override int MaxDuration => 6000;
        public override float ChanceToOccur => 0.0001f;
        public override bool OnEnd()
        {
            Main.NewText("The skies no longer pour down acid.", Color.DarkGreen);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("The skies are pouring down acid!", Color.Chartreuse);
            return base.OnBegin();
        }
        public override void UpdateActive(ModDisaster disaster)
        {
            // def = 0.16f
            Main.maxRaining = 1f;
            Texture2D acidRain = ModContent.GetInstance<NDMod>().GetTexture("Assets/Textures/AcidRain");
            if (Main.rainTexture == rainTex)
                Main.rainTexture = acidRain;
            var p = Main.player[Main.myPlayer];

            foreach (Rain rain in Main.rain)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active)
                    {
                        if (rain.active)
                        {
                            if (p.Hitbox.Contains(rain.position.ToPoint()))
                            {
                                p.AddBuff(ModContent.BuffType<AcidBurns>(), 90, false);
                            }
                            if (npc.Hitbox.Contains(rain.position.ToPoint()))
                            {
                                npc.AddBuff(ModContent.BuffType<AcidBurns>(), 90);
                            }
                        }
                    }
                }
            }
		}
        public override void UpdateInactive(ModDisaster disaster)
        {
            Texture2D acidRain = ModContent.GetInstance<NDMod>().GetTexture("Assets/Textures/AcidRain");
            if (Main.rainTexture == acidRain)
            {
                Main.rainTexture = rainTex;
            }
        }
        public override string Name => "Acid Rain";
        public override bool CanActivate => Main.raining && ModContent.GetInstance<DisasterConfig>().disasterEnabled_AcidRain;
        public override int Cooldown => 64000;
        public override int MinDuration => 3000;
        public override void UpdateAlways()
        {
            if (!Main.raining)
                ForcefullyStopDisaster();
        }
        public static Texture2D rainTex;
        public override void Initialize()
        {
            Mod mod = ModContent.GetInstance<NDMod>();
            rainTex = Main.rainTexture;

            SFXSizzle = mod.GetSound("Assets/SoundEffects/Sizzle");
            SFXISizzle = SFXSizzle.CreateInstance();
            SFXISizzle.IsLooped = true;
            SFXISizzle.Play();
            SFXISizzle.Volume = 0f;
        }
        public override void SaveAndQuit()
        {
            Main.rainTexture = rainTex;
        }
        public override bool ShouldTownNPCsGoToHomes => true;
    }
}