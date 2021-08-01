using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using NDMod.Common;
using Microsoft.Xna.Framework.Audio;

namespace NDMod.Content.Disasters
{
    public class LavaRain : ModDisaster
    {
        public override int MaxDuration => 6500;
        public override float ChanceToOccur => 0.001f;
        public override int MinDuration => 5500;
        public override bool OnEnd()
        {
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            return base.OnBegin();
        }
        public override void UpdateActive()
        {
            var player = Main.player[Main.myPlayer];
            Vector2 worldCoords = new(player.Center.X + Main.rand.NextFloat(3000, -3000), player.Center.Y - Main.rand.NextFloat(1100, 1450));
            if (Main.rand.NextFloat() <= 0.05f && WorldGen.InWorld((int)(worldCoords / 16).X, (int)(worldCoords / 16).Y))
            {
                Point tileCoords = (worldCoords / 16).ToPoint();


                var tile = Main.tile[tileCoords.X, tileCoords.Y];


                //Main.NewText("Hit! Spawned at " + tileCoords);
                tile.LiquidType = 1;
                tile.LiquidAmount = 1;
                WorldGen.SquareTileFrame(tileCoords.X, tileCoords.Y);
            }
        }
        public override string Name => "Lava Rain";
        public override int Cooldown => 12000;
        public override bool ShouldTownNPCsGoToHomes => true;
        public override bool CanActivate => ModContent.GetInstance<DisasterConfig>().disasterEnabled_LavaRain;
    }
}