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
        public static List<ModDisaster> ModDisasters { get; private set; } = new List<ModDisaster>();
        public override void PostSetupContent()
        {
            ModDisasters = OOPHelper.GetSubclasses<ModDisaster>();

            foreach (ModDisaster dis in ModDisasters)
            {
                ContentInstance.Register(dis);
                dis.Initialize();
            }

            On.Terraria.Rain.MakeRain += Rain_MakeRain;
        }

        private void Rain_MakeRain(On.Terraria.Rain.orig_MakeRain orig)
        {
            orig();
        }

        public override void PostUpdateEverything()
        {
            foreach (ModDisaster disaster in ModDisasters)
            {
                disaster.Update();

                if (disaster.Active)
                    disaster.UpdateActive(disaster);
                else
                    disaster.UpdateInactive(disaster);
            }
        }
    }
}