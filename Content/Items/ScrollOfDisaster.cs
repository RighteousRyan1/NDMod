using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NDMod.Common;
using NDMod.Content.ModPlayers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace NDMod.Content.Items
{
    public class ScrollOfDisaster : ModItem
    {
        private SoundEffect _pageFlip;
        private SoundEffectInstance _iPageFlip;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scroll of Disasters");
            Tooltip.SetDefault("Use this to read all about normal disasters.");
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Quest;
        }
        public override void UpdateInventory(Player player)
        {
            var sfx = Mod.Assets.Request<SoundEffect>("Assets/SoundEffects/PageOpen", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            if (_pageFlip != sfx)
            {
                _pageFlip = sfx;
                _iPageFlip = _pageFlip.CreateInstance();
            }
        }
        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation == 1)
            {
                DisasterPlayer.ViewingDisastersScroll = !DisasterPlayer.ViewingDisastersScroll;
                _iPageFlip.Volume = Main.soundVolume;
                _iPageFlip.Play();
            }
            return base.UseItem(player);
        }
        public override bool? CanBurnInLava()
        {
            return true;
        }
    }
}