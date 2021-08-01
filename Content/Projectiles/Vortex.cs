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
using Terraria.Audio;
using Terraria.GameContent;

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
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.damage = 20;
            Projectile.scale = 1;
			Projectile.tileCollide = false;

            if (!Main.gameMenu)
            {
                ambientSound = Mod.GetSound("Assets/SoundEffects/VortexAmbient");
                ambientSoundInstance = ambientSound.CreateInstance();
                ambientSoundInstance.IsLooped = true;
                ambientSoundInstance.Volume = 0f;
                ambientSoundInstance?.Play();
            }

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
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
            // spriteBatch.Draw(Main.magicPixel, new Rectangle(AreaOfDestruction.X - (int)Main.screenPosition.X, AreaOfDestruction.Y - (int)Main.screenPosition.Y, Projectile.width, Projectile.height), Color.White * 0.5f);
            return true;
        }
        public override void AI()
        {
            Projectile.ai[0] += _ai0Increment;
            Projectile.rotation += 0.1f;

            Projectile.velocity.Y = (float)System.Math.Cos(Projectile.ai[0]) * 5;
            Projectile.velocity.X = (float)System.Math.Sin(Projectile.ai[0]) * 5;

            float minDist = 100f;
            float GetStrength(float fromDistance)
            {
                return 400 * fromDistance / 200;
            }
            foreach (Item item in Main.item)
            {
                if (item.active)
                {
                    float dist = Projectile.Distance(item.Center);
                    if (dist < 1500f)
                    {
                        item.velocity += (Projectile.Center - item.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Item[item.type].Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, item.Center, 20);
                        item.active = false;
                    }
                }
            }
            foreach (Projectile otherProj in Main.projectile)
            {
                if (otherProj.active && otherProj.type != Projectile.type)
                {
                    float dist = Projectile.Distance(otherProj.Center);
                    if (dist < 1500f)
                    {
                        otherProj.velocity += (Projectile.Center - otherProj.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Projectile[otherProj.type].Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, otherProj.Center, 20);
                        otherProj.active = false;
                    }
                }
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != Projectile.type)
                {
                    float dist = Projectile.Distance(npc.Center);
                    if (dist < 1500f)
                    {
                        npc.velocity += (Projectile.Center - npc.Center) / GetStrength(dist);
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, npc.color);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, npc.Center, 20);
                        npc.StrikeNPC(npc.lifeMax + 10, 0, 0, false, false, true);
                    }
                }
            }
            foreach (Gore gore in Main.gore)
            {
                if (gore.active && gore.type != Projectile.type)
                {
                    float dist = Projectile.Distance(gore.position);
                    if (dist < 2000f)
                    {
                        gore.velocity += (Projectile.Center - gore.position) / GetStrength(dist);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Gore[gore.type].Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, gore.position, 20);
                        gore.active = false;
                    }
                }
            }
            foreach (Rain rain in Main.rain)
            {
                if (rain.active && rain.type != Projectile.type)
                {
                    float dist = Projectile.Distance(rain.position);
                    if (dist < 2000f)
                    {
                        rain.velocity += (Projectile.Center - rain.position) / GetStrength(dist) / 5;
                        rain.rotation = rain.rotation.AngleLerp(rain.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Rain.Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Waterfall);
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
                        float dist = Projectile.Distance(player.Center);
                        if (dist <= 1000f && !player.dead)
                        {
                            player.GetModPlayer<DisasterPlayer>().isBeingSucked = true;
                            player.velocity += (Projectile.Center - player.Center) / GetStrength(dist);
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
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBlock, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, player.skinColor);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, player.Center, 20);
                        player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} " + CommonUtils.Pick($"was brutally ripped apart by gravity.",
                            "let the stress of gravity overcome them.", "was ripped limb by limb by the tides of gravity.")), player.statLife + 25, 0);
                    }
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (Main.rand.Next(50) == 0)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
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
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                SoundEngine.PlaySound(SoundID.Item, Projectile.Center, 84);
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
            ambientSoundInstance.Volume = Projectile.Center.GetVolumeFromPosition() * Main.soundVolume;
            ambientSoundInstance.Pan = Projectile.Center.GetPanFromPosition();
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
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.damage = 20;
            Projectile.scale = 1;
            Projectile.tileCollide = false;

            if (!Main.gameMenu)
            {
                ambientSound = Mod.GetSound("Assets/SoundEffects/VortexAmbient");
                ambientSoundInstance = ambientSound.CreateInstance();
                ambientSoundInstance.IsLooped = true;
                ambientSoundInstance.Volume = 0f;
                ambientSoundInstance?.Play();
            }
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
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
            // Main.spriteBatch.Draw(Main.magicPixel, new Rectangle(AreaOfDestruction.X - (int)Main.screenPosition.X, AreaOfDestruction.Y - (int)Main.screenPosition.Y, Projectile.width, Projectile.height), Color.White * 0.5f);
            return true;
        }
        public override void AI()
        {
            Projectile.ai[0] += _ai0Increment;
            Projectile.rotation += 0.1f;

            Projectile.velocity.Y = (float)System.Math.Cos(Projectile.ai[0]) * 5;
            Projectile.velocity.X = (float)System.Math.Sin(Projectile.ai[0]) * 5;

            float minDist = 100f;
            static float GetStrength(float fromDistance)
            {
                return 400 * fromDistance / 200;
            }
            foreach (Item item in Main.item)
            {
                if (item.active)
                {
                    float dist = Projectile.Distance(item.Center);
                    if (dist < 1500f)
                    {
                        item.velocity += (Projectile.Center - item.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Item[item.type].Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, item.Center, 20);
                        item.active = false;
                    }
                }
            }
            foreach (Projectile otherProj in Main.projectile)
            {
                if (otherProj.active && otherProj.type != Projectile.type)
                {
                    float dist = Projectile.Distance(otherProj.Center);
                    if (dist < 1500f)
                    {
                        otherProj.velocity += (Projectile.Center - otherProj.Center) / GetStrength(dist);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Projectile[otherProj.type].Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, otherProj.Center, 20);
                        otherProj.active = false;
                    }
                }
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != Projectile.type)
                {
                    float dist = Projectile.Distance(npc.Center);
                    if (dist < 1500f)
                    {
                        npc.velocity += (Projectile.Center - npc.Center) / GetStrength(dist);
                        npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, npc.color);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, npc.Center, 20);
                        npc.StrikeNPC(npc.lifeMax + 10, 0, 0, false, false, true);
                    }
                }
            }
            foreach (Gore gore in Main.gore)
            {
                if (gore.active && gore.type != Projectile.type)
                {
                    float dist = Projectile.Distance(gore.position);
                    if (dist < 2000f)
                    {
                        gore.velocity += (Projectile.Center - gore.position) / GetStrength(dist);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Gore[gore.type].Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, gore.position, 20);
                        gore.active = false;
                    }
                }
            }
            foreach (Rain rain in Main.rain)
            {
                if (rain.active && rain.type != Projectile.type)
                {
                    float dist = Projectile.Distance(rain.position);
                    if (dist < 2000f)
                    {
                        rain.velocity += (Projectile.Center - rain.position) / GetStrength(dist) / 5;
                        rain.rotation = rain.rotation.AngleLerp(rain.velocity.ToRotation() + MathHelper.PiOver2, 0.5f);
                    }
                    if (dist < minDist / 2)
                    {
                        for (int m = 0; m < 20; m++)
                        {
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, TextureAssets.Rain.Value.GetAverageColor());
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Waterfall);
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
                        float dist = Projectile.Distance(player.Center);
                        if (dist <= 1000f && !player.dead)
                        {
                            player.GetModPlayer<DisasterPlayer>().isBeingSucked = true;
                            player.velocity += (Projectile.Center - player.Center) / GetStrength(dist);
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
                            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBlock, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f), 0, player.skinColor);
                            d.alpha += 3;
                            d.color = new Color(Main.rand.Next(0, 256), Main.rand.Next(0, 256), Main.rand.Next(0, 256));
                        }
                        SoundEngine.PlaySound(SoundID.Item, player.Center, 20);
                        player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} " + CommonUtils.Pick($"was brutally ripped apart by gravity.",
                            "let the stress of gravity overcome them.", "was ripped limb by limb by the tides of gravity.")), player.statLife + 25, 0);
                    }
                }
            }
            for (int i = 0; i < 200; i++)
            {
                if (Main.rand.Next(50) == 0)
                {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
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
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-0.25f, -0.5f));
                SoundEngine.PlaySound(SoundID.Item, Projectile.Center, 84);
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
            ambientSoundInstance.Volume = Projectile.Center.GetVolumeFromPosition() * Main.soundVolume;
            ambientSoundInstance.Pan = Projectile.Center.GetPanFromPosition();
        }
    }
}