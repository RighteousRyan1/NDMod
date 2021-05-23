using Terraria.ModLoader;
using NDMod.Common.Utilities;
using NDMod.Common;
using System.Collections.Generic;
using Terraria;
using System;

namespace NDMod
{
    public class NDMod : Mod
    {
        public static List<Disaster> Disasters { get; private set; } = new List<Disaster>();
        public override void PostSetupContent()
        {
            Disasters = OOPHelper.GetSubclasses<Disaster>();
        }
        public override void PostUpdateEverything()
        {
            foreach (Disaster disaster in Disasters)
            {
                if (disaster.duration > 0) 
                    disaster.duration--;
                if (Main.rand.NextFloat() <= disaster.ChanceToOccur && !disaster.Active && disaster.CanActivate) {
                    int rand = Main.rand.Next((int)(disaster.MaxDuration * 0.6f), disaster.MaxDuration);
                    disaster.duration = rand;
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