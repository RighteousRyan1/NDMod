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
    public class DisasterRadar : ModItem
    {
        public override string Texture => "NDMod/Assets/Textures/Placeholder/MissingTexture";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Disaster Radar");
            Tooltip.SetDefault("While in your inventory, displays each and every disaster\nSupports other modded disasters too\n");
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Pink;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (ModDisaster disaster in NDMod.ModDisasters)
            {
                bool isSinkhole = disaster.Name.Equals("Sinkhole");
                tooltips.Add(new(Mod, "AllDisasters", $"{disaster.Name}: " + (!isSinkhole ? (disaster.Active ? "Active" : "Inactive") : $"Last occurred {Disasters.Sinkhole.timeType}"))
                {
                    overrideColor = disaster.Active ? Color.LimeGreen : Main.errorColor
                });
            }
        }
    }
}