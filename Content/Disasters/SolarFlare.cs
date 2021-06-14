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
    public class SolarFlare : ModDisaster
    {
        public override bool ShouldTownNPCsGoToHomes => true;
        public override int MaxDuration => 17250;
        public override float ChanceToOccur => 0.000025f;
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
            var player = Main.player[Main.myPlayer].GetModPlayer<CancerPlayer>().player;
            var cancerPlayer = Main.player[Main.myPlayer].GetModPlayer<CancerPlayer>();

            var bodyTile = Main.tile[(int)player.Top.X / 16 + 1, (int)player.Top.Y / 16 + 1];
            Item item = player.HeldItem;
            bool heldItemIsNotTorch = item.createTile != TileID.Torches;

            bool dry = false;
            bool wet = false;
            bool glowStick = false;
            if (item.modItem != null)
            {
                ItemLoader.AutoLightSelect(item, ref dry, ref wet, ref glowStick);

                if (dry || wet || glowStick)
                {
                    heldItemIsNotTorch = false;
                }
            }
            // If the lighting is greater than 15% on the player's center and the player center's tile's wall is 0 (not there), increase
            if (heldItemIsNotTorch && Lighting.GetSubLight(player.Center).X < 0.15f)
            {
                if (CancerPlayer.shadersIntensity > 0.02f)
                    CancerPlayer.shadersIntensity -= 0.005f;
                if (CancerPlayer.shadersIntensity < 0.02f)
                    CancerPlayer.shadersIntensity = 0;
            }
            if ((Lighting.GetSubLight(player.Center).X >= 0.15f && bodyTile.wall <= 0) && player.ZoneOverworldHeight)
            {

                if (CancerPlayer.shadersIntensity < 0.98f)
                    CancerPlayer.shadersIntensity += 0.005f;
                if (CancerPlayer.shadersIntensity > 0.98f)
                    CancerPlayer.shadersIntensity = 1f;
            }
            else
            {
                if (CancerPlayer.shadersIntensity > 0.02f)
                    CancerPlayer.shadersIntensity -= 0.005f;
                if (CancerPlayer.shadersIntensity < 0.02f)
                    CancerPlayer.shadersIntensity = 0;
            }
            cancerPlayer.solarFlareExposure += CancerPlayer.shadersIntensity;
        }
        public override void UpdateInactive()
        {
            var player = Main.player[Main.myPlayer].GetModPlayer<CancerPlayer>().player;
            var cancerPlayer = Main.player[Main.myPlayer].GetModPlayer<CancerPlayer>();

            if (CancerPlayer.shadersIntensity > 0.02f)
                CancerPlayer.shadersIntensity -= 0.005f;
            if (CancerPlayer.shadersIntensity < 0.02f)
                CancerPlayer.shadersIntensity = 0;
            if (cancerPlayer.solarFlareExposure > 0)
                cancerPlayer.solarFlareExposure -= 2;
        }
        public override string Name => "Solar Flare";
        public override bool CanActivate => true;
        public override int Cooldown => 15000;
        public override int MinDuration => 12500;
    }
}