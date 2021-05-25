using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using NDMod.Common.Utilities;
using NDMod.Content.Disasters;
using NDMod.Content.Buffs;
using Microsoft.Xna.Framework.Input;

namespace NDMod.Content.ModPlayers
{
    public class DisasterPlayer : ModPlayer
    {
        public static bool ViewingDisastersScroll { get; set; }
        public override void ModifyScreenPosition()
        {
            var equake = ModContent.GetInstance<Earthquake>();
            var fld = ModContent.GetInstance<Flood>();
            var aRain = ModContent.GetInstance<AcidRain>();


            float rand1 = Main.rand.NextFloat(0, Earthquake.quakeSeverity);
            float rand2 = Main.rand.NextFloat(0, Earthquake.quakeSeverity);
            float d = Vector2.Distance(Main.screenPosition, Main.screenPosition + new Vector2(rand1, rand2));
            int choice = Main.rand.Next(0, 2);
            if (equake.Active)
            {
                if (choice == 0)
                    Main.screenPosition += new Vector2(rand1, rand2);
                else
                    Main.screenPosition -= new Vector2(rand1, rand2);
            }
            else if (d > 1.25f)
                Main.screenPosition += new Vector2(Main.rand.NextFloat(0, Earthquake.quakeSeverity), Main.rand.NextFloat(0, Earthquake.quakeSeverity));
        }
        public override void PostUpdate()
        {
            var equake = ModContent.GetInstance<Earthquake>();
            var fld = ModContent.GetInstance<Flood>();
            var aRain = ModContent.GetInstance<AcidRain>();
            if (Main.keyState.OnKeyPressed(Keys.End))
            {
                aRain.ForcefullyStopDisaster();
                // equake.ForcefullyStopDisaster();
                // fld.ForcefullyStopDisaster();
            }
            if (Main.keyState.OnKeyPressed(Keys.Home))
            {
                aRain.ForcefullyBeginDisaster();
                // equake.ForcefullyBeginDisaster();
                //fld.ForcefullyBeginDisaster();
            }
        }
        public override void PostUpdateBuffs()
        {
        }
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (player.HasBuff(ModContent.BuffType<AcidBurns>()))
            {
                r = 0.6f;
                g = 1f;
                b = 0.6f;
            }
        }
    }
}