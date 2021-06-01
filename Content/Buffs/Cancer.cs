using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace NDMod.Content.Buffs
{
    public class Cancer : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cancer");
            Description.SetDefault("You are permanently dying");
            Main.buffNoTimeDisplay[Type] = true;
            Main.persistentBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            canBeCleared = false;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;
            player.lifeRegen -= 1 + (int)(player.GetModPlayer<ModPlayers.CancerPlayer>().solarFlareExposure / 1000);
        }
    }
}