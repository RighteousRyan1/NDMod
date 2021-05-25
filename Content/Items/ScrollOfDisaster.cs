using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NDMod.Content.ModPlayers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NDMod.Content.Items
{
    public abstract class ScrollOfDisaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scroll of Disasters");
            Tooltip.SetDefault("Use this to read all about normal disasters.");
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useAnimation = 20;
            item.useTime = 20;
            item.UseSound = SoundID.Item77;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }
        public override bool UseItem(Player player)
        {
            if (player.itemAnimation == 1)
            {
                DisasterPlayer.ViewingDisastersScroll = !DisasterPlayer.ViewingDisastersScroll;
            }
            return base.UseItem(player);
        }
    }
}