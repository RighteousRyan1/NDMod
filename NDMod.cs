using Terraria.ModLoader;
using NDMod.Common.Utilities;
using NDMod.Common;
using System.Collections.Generic;
using Terraria;
using System;
using Microsoft.Xna.Framework;
using NDMod.Content.ModPlayers;

namespace NDMod
{
    public class NDMod : Mod
    {
        public static List<ModDisaster> ModDisasters { get; private set; } = new List<ModDisaster>();
        public static List<ModDisaster> ModdedDisasters { get; private set; } = new List<ModDisaster>();
        public override void PostSetupContent()
        {
            ModDisasters = OOPHelper.GetSubclasses<ModDisaster>();

            foreach (ModDisaster disaster in ModDisasters)
            {
                ContentInstance.Register(disaster);
                disaster.Initialize();

                bool notBase = disaster.GetType().Namespace != "NDMod.Content.Disasters";

                if (notBase)
                {
                    ModdedDisasters.Add(disaster);
                }
            }

            On.Terraria.Main.DrawInterface_30_Hotbar += Main_DrawInterface_30_Hotbar;
        }
        public override void PreSaveAndQuit()
        {
            foreach (ModDisaster disaster in ModDisasters)
            {
                disaster.SaveAndQuit();
            }
        }

        private void Main_DrawInterface_30_Hotbar(On.Terraria.Main.orig_DrawInterface_30_Hotbar orig, Main self)
        {
            orig(self);

            if (DisasterPlayer.ViewingDisastersScroll)
            {
                DrawScrollUI();
            }
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

        private static void DrawScrollUI()
        {
            // TODO: Finish after school lul
        }
    }
}