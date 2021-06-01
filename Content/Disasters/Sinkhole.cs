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
using NDMod.Content.ModPlayers;
using NDMod.Common.Utilities;

namespace NDMod.Content.Disasters
{
    public class Sinkhole : ModDisaster
    {
        public static int lastOcurrance;
        internal static string timeType;
        public override int MaxDuration => 2;
        public override float ChanceToOccur => 0.0005f;
        public override void UpdateActive(ModDisaster disaster)
        {
            if (Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().nearChests)
            {
                mod.Logger.Debug("Failed to place sinkhole. No suitable place found.");
                return;
            }
            WorldGenUtils.TryGenerateSinkholeNatural(out bool sunk);

            if (sunk)
            {
                mod.Logger.Debug("Found a place for a sinkhole!");
                lastOcurrance = 0;
            }
        }
        public override void UpdateAlways()
        {
            lastOcurrance++;


            if (lastOcurrance <= 60 * 60)
            {
                timeType = "very recently";
            }
            if (lastOcurrance > 60 * 60 && lastOcurrance <= 60 * 60 * 5)
            {
                timeType = "recently";
            }
            if (lastOcurrance > 60 * 60 * 5 && lastOcurrance <= 60 * 60 * 15)
            {
                timeType = "a while ago";
            }
        }
        public override string Name => "Sinkhole";
        public override bool CanActivate => true;
        public override int Cooldown => 14000;
    }
}