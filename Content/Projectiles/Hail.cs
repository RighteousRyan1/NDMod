using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

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
            projectile.width = (int)(8 * r);
            projectile.height = (int)(8 * r);
            projectile.tileCollide = true;
            projectile.damage = 20;
            projectile.scale = 1 * r;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void AI()
        {
            projectile.rotation += (float)System.Math.Atan2(projectile.velocity.X, projectile.velocity.Y);

            if (projectile.wet)
                projectile.Kill();
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                var d = Dust.NewDustDirect(projectile.position, 8, 8, DustID.BubbleBurst_White, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                d.alpha += 3;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Item, projectile.position, 49);

            for (int i = 0; i < 15; i++)
            {
                var d = Dust.NewDustDirect(projectile.position, 8, 8, DustID.BubbleBurst_White, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                d.alpha += 3;
            }
            return base.OnTileCollide(oldVelocity);
        }
    }
}