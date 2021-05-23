using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;

namespace NDMod.Common.Utilities
{
    public class Flood : Disaster
    {
        public override int MaxDuration => 3000;
        public override float ChanceToOccur => 0.001f;

        public override void UpdateInactive(Disaster disaster)
        {
        }
        public override bool OnEnd()
        {
            Main.NewText("Whew! The flood is over!", Color.Orange);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            Main.NewText("Oh no! It's starting to flood!", Color.BlueViolet);
            return base.OnBegin();
        }
        private bool _rainActiveNew;
        private bool _rainActiveOld;
        public override void UpdateActive(Disaster disaster)
        {
            foreach (Rain rain in Main.rain)
            {
                _rainActiveNew = rain.active;

                if (!_rainActiveNew && _rainActiveOld)
                {
                    if (Main.rand.NextFloat() < 0.025f)
                    {
                        var tile = rain.position.ToTile();

                        tile.liquid = 1;
                    }
                }

                _rainActiveOld = _rainActiveNew;
            }
        }
        public override string Name => "Flooding";
        public override bool CanActivate => Main.raining;
    }
}