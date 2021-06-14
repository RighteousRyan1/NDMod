using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ID;
using NDMod.Common;
using NDMod.Content.ModPlayers;
using Terraria.ModLoader;
using NDMod.Content.Buffs;
using NDMod.Content.Projectiles;
using NDMod.Common.Utilities;

namespace NDMod.Content.Disasters
{
    public class Thunderstorm : ModDisaster
    {
        public override int MaxDuration => 10500;
        public override float ChanceToOccur => 0.00001f;
        public override bool OnEnd()
        {
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            return base.OnBegin();
        }
        public override void UpdateActive()
        {
            Player player = Main.player[Main.myPlayer];
            Projectile p = default;
            if (Main.rand.NextFloat() <= 0.05f)
            {
                Vector2 position = player.Center + new Vector2(Main.rand.NextFloat(-1400, 1400), -700);
                Vector2 supposedVelocity = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(5, 15));
                p = Projectile.NewProjectileDirect(
                    position,
                    supposedVelocity,
                    ProjectileID.VortexLightning,
                    100,
                    2, Main.myPlayer, supposedVelocity.ToRotation(), Main.rand.Next(100));
                p.timeLeft = 180;
                p.extraUpdates = 5;
                Main.PlaySound(SoundID.DD2_LightningAuraZap, position);
            }
            if (p != default)
            {
                if (p.wet)
                {

                }
            }
        }
        public override string Name => "Thunderstorm";
        public override bool CanActivate => Main.raining && Main.maxRaining > 0.3f;
        public override int Cooldown => 6000;
        public override int MinDuration => 9250;
    }
}