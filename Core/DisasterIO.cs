using System;
using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using NDMod.Common;
using System.Linq;

namespace NDMod.Core
{
    public class DisasterIO : ModWorld
    {
        public Dictionary<string, int> nameDurations = new Dictionary<string, int>();
        public override TagCompound Save()
        {
            nameDurations.Clear();
            foreach (ModDisaster disaster in NDMod.ModDisasters)
            {
                if (disaster.Active)
                {
                    nameDurations.Add(disaster.Name, disaster.duration);
                }

                disaster.duration = 0;
            }

            return new TagCompound()
            {
                { "dName", nameDurations.Keys.ToList() },
                { "dDuration", nameDurations.Values.ToList() }
            };
        }
        public override void Initialize()
        {
            nameDurations.Clear();
        }
        public override void Load(TagCompound tag)
        {
            var names = tag.Get<List<string>>("dName");
            var values = tag.Get<List<int>>("dDuration");
            for (int i = 0; i < names.Count; i++)
            {
                var disaster = NDMod.ModDisasters.FirstOrDefault(d => d.Name == names[i]);
                if (disaster != default)
                    disaster.duration = values[i];
            }
        }
    }
}