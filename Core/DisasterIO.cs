using System;
using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using NDMod.Common;

namespace NDMod.Core
{
    public class DisasterIO : ModWorld
    {
        public List<ModDisaster> activeDisasters = new List<ModDisaster>();
        /*public override TagCompound Save()
        {
            foreach (Disaster disaster in NDMod.Disasters)
            {
                if (disaster.Active)
                {
                    activeDisasters.Add(disaster);
                }
            }

            return new TagCompound()
            {
                { "activeDisasters", activeDisasters}
            };
        }
        public override void Initialize()
        {
            activeDisasters = new List<Disaster>();
        }
        public override void Load(TagCompound tag)
        {
            tag.Get<List<Disaster>>("activeDisasters");
        }*/
    }
}