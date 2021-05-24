using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ID;

namespace NDMod.Common.Utilities
{
    /*public class Flood : Disaster
    {
        public override int MaxDuration => 6000;
        public override float ChanceToOccur => 0.01f;
        public override bool OnEnd()
        {
            Main.NewText("The ground has stopped flooding.", Color.Orange);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("Oh no! It's starting to flood!", Color.BlueViolet);
            return base.OnBegin();
        }
        public override void UpdateActive(Disaster disaster)
        {
            var p = Main.player[Main.myPlayer];
            if (Main.rand.NextFloat() < 0.025f)
            {
                var pos = p.Center - new Vector2(600, 800) + new Vector2(Main.rand.Next(0, 2500), 0);
                var tile = pos.ToTile();
                int posx = (int)pos.X;
                int posy = (int)pos.X;
                int tposx = posx / 16;
                int tposy = posy / 16;
                Main.PlaySound(SoundID.PlayerHit, pos);

                Main.NewText($"Coords: {pos} | TileCoords: {new Vector2(tposx, tposy)}");
                // Liquid.AddWater(tposx, tposy);
				int num221 = Framing.GetTileSafely(tposx, tposy).liquidType();
				int num222 = 0;
				for (int num223 = tposx - 1; num223 <= tposx + 1; num223++)
				{
					for (int num224 = tposy - 1; num224 <= tposy + 1; num224++)
					{
						if (Framing.GetTileSafely(num223, num224).liquidType() == num221)
						{
							num222 += Framing.GetTileSafely(num223, num224).liquid;
						}
					}
				}
				if (Main.tile[tposx, tposy].liquid > 0 && (num222 > 100))
				{
					int liquidType = Framing.GetTileSafely(tposx, tposy).liquidType();
					int num225 = Framing.GetTileSafely(tposx, tposy).liquid;
					Framing.GetTileSafely(tposx, tposy).liquid = 0;
					Framing.GetTileSafely(tposx, tposy).lava(lava: false);
					Framing.GetTileSafely(tposx, tposy).honey(honey: false);
					WorldGen.SquareTileFrame(tposx, tposy, resetFrame: false);
					if (Main.netMode == 1)
					{
						NetMessage.sendWater(tposx, tposy);
					}
					else
					{
						Liquid.AddWater(tposx, tposy);
					}
				}
			}
		}
        public override string Name => "Flooding";
        public override bool CanActivate => Main.raining;
        public override int Cooldown => 1000;
    }*/
}