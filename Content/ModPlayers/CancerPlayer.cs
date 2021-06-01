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
using Terraria.ModLoader.IO;

namespace NDMod.Content.ModPlayers
{
    public class CancerPlayer : ModPlayer
    {
        public bool hasCancer;
        public float solarFlareExposure;

        public static float shadersIntensity;
        public override void PostUpdate()
        {
            const float twentyTwoPointThree = 60 * 22.3f;
            if (hasCancer)
                player.AddBuff(ModContent.BuffType<Cancer>(), 2, false);
            if (solarFlareExposure >= twentyTwoPointThree)
                hasCancer = true;
            // Main.NewText($"{solarFlareExposure / 60} -> {twentyTwoPointThree / 60}");

            var pt = (Main.MouseWorld / 16).ToPoint();

            var tList = WorldGenUtils.GetTileSquareCoordinates(pt.X, pt.Y, 20, 20);

            if (Main.keyState.OnKeyPressed(Keys.L))
            {
                Main.NewText(TileID.Search.GetName(Main.MouseWorld.ToTile().type));
                foreach (Point p in tList)
                {
                    // WorldGen.TileFrame(p.X, p.Y);
                }
            }
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            solarFlareExposure = 0;
            hasCancer = false;
        }
        public override void UpdateBiomeVisuals()
        {
            player.ManageSpecialBiomeVisuals("SolarFlare", true);
            player.ManageSpecialBiomeVisuals("OrangeVignette", true);
        }
        public override void NaturalLifeRegen(ref float regen)
        {
            regen = 0;
        }
        public override TagCompound Save()
        {
            return new TagCompound()
            {
                { "hasCancer", hasCancer }
            };
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Load(TagCompound tag)
        {
            hasCancer = tag.GetBool("hasCancer");
        }
    }
}