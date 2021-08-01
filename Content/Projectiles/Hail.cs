using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;

namespace NDMod.Content.Projectiles
{
    public class Hail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hail");

        }
        public override void SetDefaults()
        {
            float r = Main.rand.NextFloat() * 1.5f;
            Projectile.width = (int)(8 * r);
            Projectile.height = (int)(8 * r);
            Projectile.tileCollide = true;
            Projectile.damage = 20;
            Projectile.scale = 1 * r;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void AI()
        {
            Projectile.rotation += (float)System.Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y);

            if (Projectile.wet)
                Projectile.Kill();
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, 8, 8, DustID.BubbleBlock, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                d.alpha += 3;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Item, Projectile.position, 49);

            for (int i = 0; i < 15; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, 8, 8, DustID.BubbleBlock, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                d.alpha += 3;
            }
            return base.OnTileCollide(oldVelocity);
        }
    }
}