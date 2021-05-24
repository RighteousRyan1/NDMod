using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using NDMod.Common.Utilities;

namespace NDMod.Content.ModPlayers
{
    public class DisasterPlayer : ModPlayer
    {
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

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.End) && !Main.oldKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.End))
            {
                equake.ForcefullyStopDisaster();
            }
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Home) && !Main.oldKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Home))
            {
                equake.ForcefullyBeginDisaster();
            }
        }
    }
}