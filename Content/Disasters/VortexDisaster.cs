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
    public class VortexDisaster : ModDisaster
    {
        public override int MaxDuration => 8500;
        public override float ChanceToOccur => 0.002f;
        public override int MinDuration => 6500;
        public override bool OnEnd()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && (proj.type == ModContent.ProjectileType<Projectiles.VortexPurple>() || proj.type == ModContent.ProjectileType<Projectiles.VortexCyan>()))
                {
                    proj.Kill();
                }
            }
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Vector2 randSpot = new Vector2(Main.rand.NextFloat(Main.player[0].Center.X - 3000, Main.player[0].Center.X + 3000), Main.rand.NextFloat(Main.player[0].Center.Y + 25, Main.player[0].Center.Y - 200));

            var proj = Projectile.NewProjectileDirect(randSpot,
                Vector2.Zero,
                Main.rand.Next(2) == 0 ? ModContent.ProjectileType<Projectiles.VortexPurple>() : ModContent.ProjectileType<Projectiles.VortexCyan>(),
                0, 0);

            proj.timeLeft = duration;
            return base.OnBegin();
        }
        public override string Name => "Vortex";
        public override int Cooldown => 9500;
        public override bool ShouldTownNPCsGoToHomes => true;
        public override bool CanActivate => ModContent.GetInstance<DisasterConfig>().disasterEnabled_Earthquakes;
    }
}