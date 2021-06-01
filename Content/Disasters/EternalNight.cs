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
    public class EternalNight : ModDisaster
    {
        public override int MaxDuration => 11000;
        public override float ChanceToOccur => 0.0001f;

        public static int bgStopModifyTimer;
        public override bool OnEnd()
        {
            Main.NewText("The world lightens...", Color.White);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("The world darkens...", Color.DarkGray);
            return base.OnBegin();
        }
        public override void UpdateActive(ModDisaster disaster)
        {
            bgStopModifyTimer = 33;
            var player = Main.player[Main.myPlayer].GetModPlayer<CancerPlayer>().player;
        }
        public override void UpdateInactive(ModDisaster disaster)
        {
            bgStopModifyTimer--;
            var player = Main.player[Main.myPlayer].GetModPlayer<CancerPlayer>().player;
        }
        public override string Name => "Eternal Darkness";
        public override bool CanActivate => !Main.dayTime;
        public override int Cooldown => 9000;
        public override int MinDuration => 9000;
    }
}