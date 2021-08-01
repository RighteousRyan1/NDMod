using Terraria.ModLoader;
using Terraria;

namespace NDMod.Content
{
    public class AnywhereBiome : ModBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            return true;
        }
        public override void BiomeVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("SolarFlare", true);
            player.ManageSpecialBiomeVisuals("OrangeVignette", true);
        }
        
    }
}