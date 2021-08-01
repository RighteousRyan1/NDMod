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

namespace NDMod.Content.Disasters
{
    public class ColdFront : ModDisaster
    {
        public override int MaxDuration => 29000;
        public override float ChanceToOccur => 0.005f;
        public override void UpdateActive()
        {
            var p = Main.player[Main.myPlayer];
            var pl = p.GetModPlayer<ModPlayers.DisasterPlayer>();
            if (p.ZoneSnow)
            {
                if (p.wet && !p.lavaWet)
                {
                    pl.shouldLoseLife = true;
                    p.AddBuff(BuffID.Frozen, 2);
                }
                else
                    pl.shouldLoseLife = false;
            }
		}
        public override string Name => "Cold Front";
        public override bool CanActivate => true;
        public override int Cooldown => 4000;
        public override int MinDuration => 21000;
    }
}