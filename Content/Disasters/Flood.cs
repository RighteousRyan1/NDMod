using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ID;
using NDMod.Common;

namespace NDMod.Content.Disasters
{
    public class Flood : ModDisaster
    {
        public override int MaxDuration => 6000;
        public override float ChanceToOccur => 0.0005f;
        public override bool OnEnd()
        {
            Main.NewText("The ground has stopped flooding.", Color.LightSkyBlue);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("Oh no! It's starting to flood!", Color.LightBlue);
            return base.OnBegin();
        }
        private bool wasActive;
        private bool isActive;
        public override void UpdateActive(ModDisaster disaster)
        {
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
                        var t = Main.tile[tposx, tposy - 2];
                        var tileBelow = Framing.GetTileSafely(tposx, tposy + 1);

                        isActive = tileBelow.active() && tileBelow.collisionType == 1;

                        if (isActive && !wasActive)
                        {
                            Main.PlaySound(SoundID.PlayerHit, pos);

                            p.Center = new Vector2(posx, posy);
                            Main.NewText($"Coords: {pos} | TileCoords: {new Vector2(tposx, tposy)}");
                            t.liquidType(0);
                            t.liquid = 1;
                            WorldGen.SquareTileFrame(tposx, tposy - 2);
                            WorldGen.DiamondTileFrame(tposx, tposy - 2);
                            if (Main.dedServ)
                            {
                                NetMessage.sendWater(tposx, tposy);
                            }
                            else
                            {
                                Liquid.AddWater(tposx, tposy);
                            }
                        }

                        wasActive = isActive;
                    }
                }
            }
		}
        public override string Name => "Flooding";
        public override bool CanActivate => false;// Main.raining;
        public override int Cooldown => 1000;
    }
}