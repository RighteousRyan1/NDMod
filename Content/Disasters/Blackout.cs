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

namespace NDMod.Content.Disasters
{
    public class Blackout : ModDisaster
    {
        public override int MaxDuration => 16000;
        public override float ChanceToOccur => 0.0001f;

        public static int bgStopModifyTimer;
        public override void UpdateActive()
        {
            bgStopModifyTimer = 33;
        }
        public override void UpdateInactive()
        {
            bgStopModifyTimer--;
        }
        public override string Name => "Blackout";
        public override bool CanActivate => !Main.dayTime;
        public override int Cooldown => 9000;
        public override int MinDuration => 12000;
    }
}