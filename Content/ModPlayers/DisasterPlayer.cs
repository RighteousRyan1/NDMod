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
using Terraria.Audio;

namespace NDMod.Content.ModPlayers
{
    public class DisasterPlayer : ModPlayer
    {
        public static bool ViewingDisastersScroll { get; set; }
        public bool shouldLoseLife;
        public bool nearChests;
        public bool isBeingSucked;

        public int joiningWorldTimer;
        internal bool isImmuneToVortex;
        public override bool PreItemCheck()
        {
            var ingameUIPath = "Assets/Textures/UI/IngameUI";
            var scrollBody = Mod.GetTexture($"{ingameUIPath}/ScrollBody");
            (int, int) bounds = CommonUtils.GetScreenCenter();
            Rectangle scrollBounds = new(bounds.Item1 - scrollBody.Width / 2, bounds.Item2 - scrollBody.Height / 2, scrollBody.Width, scrollBody.Height + 50);
            return ViewingDisastersScroll && scrollBounds.Contains(Main.MouseScreen.ToPoint()) ? false : true;
        }

        private bool _oldVortexAffect;
        private bool _newVortexAffect;
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            _newVortexAffect = isBeingSucked;
            Player.fullRotationOrigin = Player.Hitbox.Size() / 2;
            if (isBeingSucked)
            {
                Player.fullRotation = Player.fullRotation.AngleLerp(Player.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                Player.bodyFrame.Y = 280;
                Player.legFrame.Y = 280;
            }

            if (_oldVortexAffect && !_newVortexAffect)
                Player.fullRotation = 0f;

            _oldVortexAffect = _newVortexAffect;
        }
        public override void ModifyScreenPosition()
        {
            var equake = ModContent.GetInstance<Earthquake>();
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
            if (!Player.HasItem(ModContent.ItemType<ScrollOfDisaster>()))
                ViewingDisastersScroll = false;
            if (Main.GameUpdateCount % 30 == 0)
            {
                for (int x = (int)Player.Center.X / 16 - 70; x < (int)Player.Center.X / 16 + 70; x++)
                {
                    for (int y = (int)Player.Center.Y / 16 - 40; y < (int)Player.Center.Y / 16 + 40; y++)
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


            if (Main.keyState.AreKeysPressed(Keys.Divide, Keys.Multiply))
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                isImmuneToVortex = true;
            }
            if (Main.keyState.AreKeysPressed(Keys.Add, Keys.Subtract))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                isImmuneToVortex = false;
            }
        }
        public override void ResetEffects()
        {
            if (Main.GameUpdateCount % 30 == 0)
            {
                nearChests = false;
            }
            isBeingSucked = false;
        }
        public override void PostUpdateBuffs()
        {
            if (joiningWorldTimer > 0)
                joiningWorldTimer--;
            bool HB(int type)
            {
                return Player.HasBuff(type);
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
            if (Player.HasBuff(ModContent.BuffType<AcidBurns>()))
            {
                string pick = CommonUtils.Pick($"{Player.name} rotted away from acid.", $"{Player.name} couldn't handle the acid burn.", $"{Player.name} let themselves rot out.");
                damageSource.SourceCustomReason = pick;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            isBeingSucked = false;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Player.HasBuff(ModContent.BuffType<AcidBurns>()))
            {
                r = 0.6f;
                g = 1f;
                b = 0.6f;
            }
            if (Player.HasBuff(ModContent.BuffType<ExtremeChills>())) // change to extreme chills
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
            if (Player.ZoneSnow)
            {
                if (cFront.Active)
                {
                    Player.AddBuff(ModContent.BuffType<ExtremeChills>(), 2);
                }
            }
        }
    }
}