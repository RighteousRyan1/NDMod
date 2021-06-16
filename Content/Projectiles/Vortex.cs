using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using NDMod.Common.Utilities;
using Terraria.DataStructures;
using NDMod.Content.ModPlayers;
using Microsoft.Xna.Framework.Audio;

namespace NDMod.Content.Projectiles
{
    public class VortexPurple : ModProjectile
    {
        private float _ai0Increment;
        public Rectangle AreaOfDestruction { get; set; }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex");
        }
        public override void SetDefaults()
        {
            _ai0Increment = Main.rand.NextFloat(0.01f, 0.1f);
            projectile.width = 200;
            projectile.height = 200;
            projectile.damage = 20;
            projectile.scale = 1;
			projectile.tileCollide = false;

            if (!Main.gameMenu)
            {
                ambientSound = mod.GetSound("Assets/SoundEffects/VortexAmbient");
                ambientSoundInstance = ambientSound.CreateInstance();
                ambientSoundInstance.IsLooped = true;
                ambientSoundInstance.Volume = 0f;
                ambientSoundInstance?.Play();
            }

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
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
            spriteBatch.Draw(Main.magicPixel, new Rectangle(AreaOfDestruction.X - (int)Main.screenPosition.X, AreaOfDestruction.Y - (int)Main.screenPosition.Y, projectile.width, projectile.height), Color.White * 0.5f);
            return true;
        }
        public override void AI()
        {
            projectile.ai[0] += _ai0Increment;
            projectile.rotation += 0.1f;

            projectile.velocity.Y = (float)System.Math.Cos(projectile.ai[0]) * 5;
            projectile.velocity.X = (float)System.Math.Sin(projectile.ai[0]) * 5;

            float minDist = 100f;
            float GetStrength(float fromDistance)
            {
                return 400 * fromDistance / 200;
            }
            foreach (Item item in Main.item)
            {
                if (item.active)
                {
                    float dist = projectile.Distance(item.Center);
                    if (dist < 1500f)
                    {
                        item.velocity += (projectile.Center - item.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.itemTexture[item.type].GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, item.Center, 20);
                        item.active = false;
                    }
                }
            }
            foreach (Projectile otherProj in Main.projectile)
            {
                if (otherProj.active && otherProj.type != projectile.type)
                {
                    float dist = projectile.Distance(otherProj.Center);
                    if (dist < 1500f)
                    {
                        otherProj.velocity += (projectile.Center - otherProj.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.projectileTexture[otherProj.type].GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, otherProj.Center, 20);
                        otherProj.active = false;
                    }
                }
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != projectile.type)
                {
                    float dist = projectile.Distance(npc.Center);
                    if (dist < 1500f)
                    {
                        npc.velocity += (projectile.Center - npc.Center) / GetStrength(dist);
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, npc.color);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, npc.Center, 20);
                        npc.StrikeNPC(npc.lifeMax + 10, 0, 0, false, false, true);
                    }
                }
            }
            foreach (Gore gore in Main.gore)
            {
                if (gore.active && gore.type != projectile.type)
                {
                    float dist = projectile.Distance(gore.position);
                    if (dist < 2000f)
                    {
                        gore.velocity += (projectile.Center - gore.position) / GetStrength(dist);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.goreTexture[gore.type].GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, gore.position, 20);
                        gore.active = false;
                    }
                }
            }
            foreach (Rain rain in Main.rain)
            {
                if (rain.active && rain.type != projectile.type)
                {
                    float dist = projectile.Distance(rain.position);
                    if (dist < 2000f)
                    {
                        rain.velocity += (projectile.Center - rain.position) / GetStrength(dist) / 5;
                        rain.rotation = rain.rotation.AngleLerp(rain.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.rainTexture.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Waterfall);
                        rain.active = false;
                    }
                }
            }
            if (!Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().isImmuneToVortex)
            {
                foreach (Player player in Main.player)
                {
                    if (player.active)
                    {
                        float dist = projectile.Distance(player.Center);
                        if (dist <= 1000f && !player.dead)
                        {
                            player.GetModPlayer<DisasterPlayer>().isBeingSucked = true;
                            player.velocity += (projectile.Center - player.Center) / GetStrength(dist);
                            if (Main.GameUpdateCount % 5 == 0)
                                player.statLife -= 4 - (int)System.Math.Round(dist) / 250;
                            // Main.NewText(10f - (int)System.Math.Round(dist) / 100);
                        }
                        if (dist > 1000f)
                            player.GetModPlayer<DisasterPlayer>().isBeingSucked = false;
                    }
                    if (player.statLife < 1)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.BubbleBlock, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, player.skinColor);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, player.Center, 20);
                        player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} " + CommonUtils.Pick($"was brutally ripped apart by gravity.",
                            "let the stress of gravity overcome them.", "was ripped limb by limb by the tides of gravity.")), player.statLife + 25, 0);
                    }
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (Main.rand.Next(50) == 0)
                {
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Clentaminator_Purple, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                    d.alpha += 3;
                    d.scale = 0.4f;
                    d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 800; i++)
            {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Clentaminator_Purple, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                Main.PlaySound(SoundID.Item, projectile.Center, 84);
                d.alpha += 3;
                d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
            }
            ambientSoundInstance?.Stop();
            Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().isBeingSucked = false;
            Main.player[Main.myPlayer].fullRotation = 0f;
        }
        public SoundEffect ambientSound;
        public SoundEffectInstance ambientSoundInstance;
        public override void PostAI()
        {
            ambientSoundInstance.Volume = projectile.Center.GetVolumeFromPosition() * Main.soundVolume;
            ambientSoundInstance.Pan = projectile.Center.GetPanFromPosition();
        }
    }
    public class VortexCyan : ModProjectile
    {
        private float _ai0Increment;
        public Rectangle AreaOfDestruction { get; set; }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex");
        }
        public override void SetDefaults()
        {
            _ai0Increment = Main.rand.NextFloat(0.01f, 0.1f);
            projectile.width = 200;
            projectile.height = 200;
            projectile.damage = 20;
            projectile.scale = 1;
            projectile.tileCollide = false;

            if (!Main.gameMenu)
            {
                ambientSound = mod.GetSound("Assets/SoundEffects/VortexAmbient");
                ambientSoundInstance = ambientSound.CreateInstance();
                ambientSoundInstance.IsLooped = true;
                ambientSoundInstance.Volume = 0f;
                ambientSoundInstance?.Play();
            }
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
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
            spriteBatch.Draw(Main.magicPixel, new Rectangle(AreaOfDestruction.X - (int)Main.screenPosition.X, AreaOfDestruction.Y - (int)Main.screenPosition.Y, projectile.width, projectile.height), Color.White * 0.5f);
            return true;
        }
        public override void AI()
        {
            projectile.ai[0] += _ai0Increment;
            projectile.rotation += 0.1f;

            projectile.velocity.Y = (float)System.Math.Cos(projectile.ai[0]) * 5;
            projectile.velocity.X = (float)System.Math.Sin(projectile.ai[0]) * 5;

            float minDist = 100f;
            float GetStrength(float fromDistance)
            {
                return 400 * fromDistance / 200;
            }
            foreach (Item item in Main.item)
            {
                if (item.active)
                {
                    float dist = projectile.Distance(item.Center);
                    if (dist < 1500f)
                    {
                        item.velocity += (projectile.Center - item.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.itemTexture[item.type].GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, item.Center, 20);
                        item.active = false;
                    }
                }
            }
            foreach (Projectile otherProj in Main.projectile)
            {
                if (otherProj.active && otherProj.type != projectile.type)
                {
                    float dist = projectile.Distance(otherProj.Center);
                    if (dist < 1500f)
                    {
                        otherProj.velocity += (projectile.Center - otherProj.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.projectileTexture[otherProj.type].GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, otherProj.Center, 20);
                        otherProj.active = false;
                    }
                }
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != projectile.type)
                {
                    float dist = projectile.Distance(npc.Center);
                    if (dist < 1500f)
                    {
                        npc.velocity += (projectile.Center - npc.Center) / GetStrength(dist);
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, npc.color);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, npc.Center, 20);
                        npc.StrikeNPC(npc.lifeMax + 10, 0, 0, false, false, true);
                    }
                }
            }
            foreach (Gore gore in Main.gore)
            {
                if (gore.active && gore.type != projectile.type)
                {
                    float dist = projectile.Distance(gore.position);
                    if (dist < 2000f)
                    {
                        gore.velocity += (projectile.Center - gore.position) / GetStrength(dist);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.goreTexture[gore.type].GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, gore.position, 20);
                        gore.active = false;
                    }
                }
            }
            foreach (Rain rain in Main.rain)
            {
                if (rain.active && rain.type != projectile.type)
                {
                    float dist = projectile.Distance(rain.position);
                    if (dist < 2000f)
                    {
                        rain.velocity += (projectile.Center - rain.position) / GetStrength(dist) / 5;
                        rain.rotation = rain.rotation.AngleLerp(rain.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, Main.rainTexture.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Waterfall);
                        rain.active = false;
                    }
                }
            }
            if (!Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().isImmuneToVortex)
            {
                foreach (Player player in Main.player)
                {
                    if (player.active)
                    {
                        float dist = projectile.Distance(player.Center);
                        if (dist <= 1000f && !player.dead)
                        {
                            player.GetModPlayer<DisasterPlayer>().isBeingSucked = true;
                            player.velocity += (projectile.Center - player.Center) / GetStrength(dist);
                            if (Main.GameUpdateCount % 5 == 0)
                                player.statLife -= 4 - (int)System.Math.Round(dist) / 250;
                        }
                        if (dist > 1000f)
                            player.GetModPlayer<DisasterPlayer>().isBeingSucked = false;
                    }
                    if (player.statLife < 1)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.BubbleBlock, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, player.skinColor);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        Main.PlaySound(SoundID.Item, player.Center, 20);
                        player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} " + CommonUtils.Pick($"was brutally ripped apart by gravity.",
                            "let the stress of gravity overcome them.", "was ripped limb by limb by the tides of gravity.")), player.statLife + 25, 0);
                    }
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (Main.rand.Next(50) == 0)
                {
                    var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Clentaminator_Cyan, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                    d.alpha += 3;
                    d.scale = 0.4f;
                    d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 800; i++)
            {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Clentaminator_Purple, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                Main.PlaySound(SoundID.Item, projectile.Center, 84);
                d.alpha += 3;
                d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
            }
            ambientSoundInstance?.Stop();
            Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().isBeingSucked = false;
            Main.player[Main.myPlayer].fullRotation = 0f;
        }
        public SoundEffect ambientSound;
        public SoundEffectInstance ambientSoundInstance;
        public override void PostAI()
        {
            ambientSoundInstance.Volume = projectile.Center.GetVolumeFromPosition() * Main.soundVolume;
            ambientSoundInstance.Pan = projectile.Center.GetPanFromPosition();
        }
    }
}