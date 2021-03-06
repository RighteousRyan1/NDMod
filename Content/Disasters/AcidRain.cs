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
using NDMod.Common.Utilities;
using Terraria.GameContent;

namespace NDMod.Content.Disasters
{
    public class AcidRain : ModDisaster
    {
        public static SoundEffect SFXSizzle;
        public static SoundEffectInstance SFXISizzle;
        public override int MaxDuration => 12000;
        public override float ChanceToOccur => 0.000015f;
        public override void UpdateActive()
        {
            var rainTexture = TextureAssets.Rain.Value;
            // def = 0.16f
            Main.maxRaining = 0.6f;
            Texture2D acidRain = Mod.GetTexture("Assets/Textures/AcidRain");
            if (rainTexture == rainTex)
                rainTexture = acidRain;
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
        public override void UpdateInactive()
        {
            var rainTexture = TextureAssets.Rain.Value;
            Texture2D acidRain = Mod.GetTexture("Assets/Textures/AcidRain");
            if (rainTexture == acidRain)
                rainTexture = rainTex;
        }
        public override string Name => "Acid Rain";
        public override bool CanActivate => Main.raining && ModContent.GetInstance<DisasterConfig>().disasterEnabled_AcidRain && !ModContent.GetInstance<Hailstorm>().Active && base.CanActivate;
        public override int Cooldown => 32000;
        public override int MinDuration => 8000;
        public static Texture2D rainTex;
        public override void Initialize()
        {
            Mod mod = ModContent.GetInstance<NDMod>();
            if (!Main.dedServ)
            {
                rainTex = TextureAssets.Rain.Value;

                SFXSizzle = mod.GetSound("Assets/SoundEffects/Sizzle");
                SFXISizzle = SFXSizzle.CreateInstance();
                SFXISizzle.IsLooped = true;
                SFXISizzle.Play();
                SFXISizzle.Volume = 0f;
            }
        }
        public override void SaveAndQuit()
        {
            var rainTexture = TextureAssets.Rain.Value;
            rainTexture = rainTex;
            SFXISizzle.Volume = 0f;
        }
        public override bool ShouldTownNPCsGoToHomes => true;
    }
}