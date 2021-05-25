using Terraria.ModLoader;
using NDMod.Common.Utilities;
using NDMod.Common;
using System.Collections.Generic;
using Terraria;
using System;
using Microsoft.Xna.Framework;
using NDMod.Content.ModPlayers;
using MonoMod.Cil;
using Mono.Cecil.Cil;

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
            IL.Terraria.NPC.AI_007_TownEntities += GoToHomes;
            On.Terraria.Main.DrawInterface_30_Hotbar += Main_DrawInterface_30_Hotbar;
        }

        private void GoToHomes(ILContext il)
        {
            foreach (ModDisaster disaster in ModDisasters)
            {
                // Here you go lolxd
                if (disaster.ShouldTownNPCsGoToHomes && disaster.Active)
                {
                    var c = new ILCursor(il);

                    /*
                     * Header Size: 12 bytes
                     * Code Size: 22134 (0x5676) bytes
                     * LocalVarSig Token: 0x1100017D RID: 381
                     * .maxstack 11
                     * .locals init (
                     * [0] int32 maxValue,
                     * [1] bool flag,
                     */
                    bool canGoto = c.TryGotoNext(x => x.MatchLdloc(1)); // match flag, the boolean tracking movements back to houses.
                    if (canGoto)
                    {
                        var op = OpCodes.Pop;

                        // c.EmitDelegate<Func<bool>>(() => 
                    }
                    else
                        return;
                }
            }
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