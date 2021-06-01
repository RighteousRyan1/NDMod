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

namespace NDMod.Content.Disasters
{
    public class Flood : ModDisaster
    {
        public override int MaxDuration => 9000;
        public override float ChanceToOccur => 0.0001f;
        public override bool OnEnd()
        {
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            return base.OnBegin();
        }
        private bool wasActive;
        private bool isActive;
        public override void UpdateActive(ModDisaster disaster)
        {
            if (!ModContent.GetInstance<AcidRain>().Active)
            {
                Main.maxRaining = 0.8f;
            }
            var p = Main.player[Main.myPlayer];

            foreach (Rain rain in Main.rain)
            {
                if (rain.active)
                {
                    rain.velocity *= 1.025f;
                    if (Main.rand.NextFloat() < 0.01f)
                    {
                        var pos = rain.position;
                        int posx = (int)pos.X;
                        int posy = (int)pos.Y;
                        int tposx = posx / 16;
                        int tposy = posy / 16;
                        var t = Main.tile[tposx, tposy - 1];
                        var tileBelow = Framing.GetTileSafely(tposx, tposy + 1);

                        isActive = (tileBelow.active() && tileBelow.collisionType == 1) || tileBelow.liquid > 0;

                        if (isActive && !wasActive)
                        {
                            t.liquidType(0);
                            t.liquid = 255;
                            WorldGen.SquareTileFrame(tposx, tposy - 1);
                            WorldGen.DiamondTileFrame(tposx, tposy - 1);
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                NetMessage.sendWater(tposx, tposy - 1);
                            }
                            else
                            {
                                Liquid.AddWater(tposx, tposy - 1);
                            }
                            Liquid.UpdateLiquid();
                        }

                        wasActive = isActive;
                    }
                }
            }
        }
        public override string Name => "Flooding";
        public override bool CanActivate => Main.raining;
        public override int Cooldown => 3000;
    }
}