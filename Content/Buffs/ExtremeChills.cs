using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace NDMod.Content.Buffs
{
    public class ExtremeChills : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Extreme Chills");
            Description.SetDefault("You feel pieces of your skin freezing...\nExtremely reduced movement");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            CanBeCleared = false;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ZoneRockLayerHeight)
            {
                player.maxRunSpeed *= 0.3f;
                player.accRunSpeed *= 0.3f;
            }
            else
            {
                player.maxRunSpeed *= 0.4f;
                player.accRunSpeed *= 0.4f;
            }
            if (player.GetModPlayer<ModPlayers.DisasterPlayer>().shouldLoseLife)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
                player.lifeRegen -= 32;
            }
        }
    }
}