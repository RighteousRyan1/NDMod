using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using NDMod.Common.Utilities;
using NDMod.Content.Disasters;
using NDMod.Content.Buffs;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Terraria.DataStructures;

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
                equake.ForcefullyStopDisaster();
                // equake.ForcefullyStopDisaster();
                // fld.ForcefullyStopDisaster();
            }
            if (Main.keyState.OnKeyPressed(Keys.Home))
            {
                equake.ForcefullyBeginDisaster();
                // equake.ForcefullyBeginDisaster();
                //fld.ForcefullyBeginDisaster();
            }
        }
        public override void PostUpdateBuffs()
        {
            bool HB(int type)
            {
                return player.HasBuff(type);
            }
            var vol = AcidRain.SFXISizzle.Volume;
            if (HB(ModContent.BuffType<AcidBurns>()))
            {
                AcidRain.SFXISizzle.Volume += 0.001f;
                if (AcidRain.SFXISizzle.Volume > 0.075f)
                    AcidRain.SFXISizzle.Volume = 0.075f;
            }
            else
            {
                AcidRain.SFXISizzle.Volume = 0f;
            }

        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (player.HasBuff(ModContent.BuffType<AcidBurns>()))
            {
                string pick = CommonUtils.Pick($"{player.name} rotted away from acid.", $"{player.name} couldn't handle the acid burn.", $"{player.name} let themselves rot out.");
                damageSource.SourceCustomReason = pick;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
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