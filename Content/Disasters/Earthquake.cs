using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace NDMod.Common.Utilities
{
    public class Earthquake : Disaster
    {
        public static float quakeSeverity;
        public override int MaxDuration => 1000;
        public override float ChanceToOccur => 0.01f;
        public override bool OnEnd()
        {
            Main.NewText("The ground has quit trembling.", Color.Orange);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("An earthquake! Get somewhere safe!", Color.BlueViolet);
            return base.OnBegin();
        }
        private float _achieve;
        public override void UpdateActive(Disaster disaster)
        {
            // Activity is handled in a ModPlayer.

            if (Main.GameUpdateCount % 300 == 0)
            {
                _achieve = Main.rand.NextFloat(-20, 20);
            }

            MathMethods.RoughStep(ref quakeSeverity, _achieve, 0.1f);
        }
        public override string Name => "Earthquake";
        public override int Cooldown => 10 * 60 * 60;
        public override void UpdateInactive(Disaster disaster)
        {
            _achieve = 0;
            MathMethods.RoughStep(ref quakeSeverity, _achieve, 0.1f);
        }
    }
}