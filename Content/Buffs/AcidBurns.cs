using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace NDMod.Content.Buffs
{
    public class AcidBurns : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acidic Burns");
            Description.SetDefault("Your flesh is screaming in pain");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            CanBeCleared = false;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;
            player.lifeRegen -= 24;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;
            npc.lifeRegen -= 36;
        }
    }
}