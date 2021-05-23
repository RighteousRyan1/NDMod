using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;

namespace NDMod.Common.Utilities
{
    /*public class Earthquake : Disaster
    {
        public override int MaxDuration => 300;
        public override float ChanceToOccur => 0.004f;

        public override void UpdateInactive(Disaster disaster)
        {
        }
        public override bool OnEnd()
        {
            Main.NewText("EARTH OVER");
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("EARTH");
            return base.OnBegin();
        }
        public override void UpdateActive(Disaster disaster)
        {
            Player player = Main.player[Main.myPlayer];
            int dust = Dust.NewDust(player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), 1, 1, Terraria.ID.DustID.Fire, 0, -5);
        }
        public override string Name => "Earthquake";
    }*/
}