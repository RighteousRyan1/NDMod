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

namespace NDMod.Content.Disasters
{
    public class Hailstorm : ModDisaster
    {
        public override int MaxDuration => 11000;
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
            foreach (Rain rain in Main.rain)
            {
                if (rain.active)
                {
                    if (Main.rand.NextBool(5))
                    {
                        var pr = Projectile.NewProjectileDirect(rain.position, rain.velocity, ModContent.ProjectileType<Hail>(), 30, 2);
                        pr.hostile = true;
                        pr.friendly = true;
                    }
                    rain.active = false;
                }
            }
        }
        public override string Name => "Hailstorm";
        public override bool CanActivate => Main.raining && !ModContent.GetInstance<AcidRain>().Active;
        public override int Cooldown => 6000;
        public override int MinDuration => 8250;
    }
}