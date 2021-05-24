using Terraria.ModLoader;
using NDMod.Common.Utilities;
using NDMod.Common;
using System.Collections.Generic;
using Terraria;
using System;
using Microsoft.Xna.Framework;

namespace NDMod
{
    public class NDMod : Mod
    {
        public static List<Disaster> Disasters { get; private set; } = new List<Disaster>();
        public override void PostSetupContent()
        {
            Disasters = OOPHelper.GetSubclasses<Disaster>();

            foreach (Disaster dis in Disasters)
            {
                ContentInstance.Register(dis);
            }

            On.Terraria.Rain.MakeRain += Rain_MakeRain;
        }

        private void Rain_MakeRain(On.Terraria.Rain.orig_MakeRain orig)
        {
            orig();
        }

        public override void PostUpdateEverything()
        {
            foreach (Disaster disaster in Disasters)
            {
                if (Main.rand.NextFloat() <= disaster.ChanceToOccur && !disaster.Active && disaster.CanActivate) {
                    disaster.ForcefullyBeginDisaster();
                }
                if (disaster.GetBegin())
                    disaster.OnBegin();

                if (disaster.GetEnd())
                    disaster.OnEnd();

                if (disaster.Active)
                    disaster.UpdateActive(disaster);
                else
                    disaster.UpdateInactive(disaster);
            }
        }
    }
}