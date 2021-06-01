using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using NDMod.Common.Utilities;
using NDMod.Content.Disasters;
using NDMod.Content.Buffs;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Terraria.DataStructures;
using NDMod.Content.Items;
using NDMod.Common.Systems;
using NDMod.Core;

namespace NDMod.Content.ModPlayers
{
    public class DisasterPlayer : ModPlayer
    {
        public static bool ViewingDisastersScroll { get; set; }
        public bool shouldLoseLife;
        public bool nearChests;

        public int joiningWorldTimer;
        public override bool PreItemCheck()
        {
            var ingameUIPath = "Assets/Textures/UI/IngameUI";
            var scrollBody = mod.GetTexture($"{ingameUIPath}/ScrollBody");
            (int, int) bounds = CommonUtils.GetScreenCenter();
            Rectangle scrollBounds = new Rectangle(bounds.Item1 - scrollBody.Width / 2, bounds.Item2 - scrollBody.Height / 2, scrollBody.Width, scrollBody.Height);
            return ViewingDisastersScroll && scrollBounds.Contains(Main.MouseScreen.ToPoint()) ? false : true;
        }
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
        public override void OnEnterWorld(Player player)
        {
            joiningWorldTimer = 50;
            var x = ModContent.GetInstance<DisasterIO>();
            foreach (string str in x.nameDurations.Keys)
            {
                foreach (int num in x.nameDurations.Values)
                {
                    Main.NewText($"{str}: {num}");
                }
            }
        }
        public override void PostUpdate()
        {
            if (!player.HasItem(ModContent.ItemType<ScrollOfDisaster>()))
                ViewingDisastersScroll = false;
            if (Main.GameUpdateCount % 30 == 0)
            {
                for (int x = (int)player.Center.X / 16 - 70; x < (int)player.Center.X / 16 + 70; x++)
                {
                    for (int y = (int)player.Center.Y / 16 - 40; y < (int)player.Center.Y / 16 + 40; y++)
                    {
                        if (WorldGen.InWorld(x, y))
                        {
                            Tile t = Framing.GetTileSafely(x, y);

                            if (t.type == TileID.Containers || t.type == TileID.Containers2)
                            {
                                nearChests = true;
                            }
                        }
                    }
                }
            }
        }
        public override void ResetEffects()
        {
            if (Main.GameUpdateCount % 30 == 0)
            {
                nearChests = false;
            }
        }
        public override void PostUpdateBuffs()
        {
            if (joiningWorldTimer > 0)
                joiningWorldTimer--;
            bool HB(int type)
            {
                return player.HasBuff(type);
            }
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
            if (player.HasBuff(ModContent.BuffType<ExtremeChills>())) // change to extreme chills
            {
                r = 0.6f;
                g = 0.6f;
                b = 1f;
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            // Main.NewText($"MRS: {player.maxRunSpeed} | RA: {player.runAcceleration} | ARS: {player.accRunSpeed}");
            var cFront = ModContent.GetInstance<ColdFront>();
            if (player.ZoneSnow)
            {
                if (cFront.Active)
                {
                    player.AddBuff(ModContent.BuffType<ExtremeChills>(), 2);
                }
            }
        }
    }
}