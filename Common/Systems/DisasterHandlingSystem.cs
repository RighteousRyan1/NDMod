using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NDMod.Common.Utilities;
using NDMod.Content.Disasters;
using NDMod.Content.ModPlayers;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace NDMod.Common.Systems
{
    public class DisasterHandlingSystem : ModSystem
    {
        private Color darkeningColor;
        private Color _solarFlareBGColor;

        private ScreenShaderData _dataWaveSolar;
        private ScreenShaderData _dataOrangeVignette;
        public override void OnModLoad()
        {
            darkeningColor = new Color(33, 33, 33);
            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> WaveSolarFlare = new(Mod.GetEffect("Effects/SolarFlare"));
                _dataWaveSolar = new ScreenShaderData(WaveSolarFlare, "SolarFlare");
                Filters.Scene["SolarFlare"] = new Filter(_dataWaveSolar, EffectPriority.VeryHigh);
                Ref<Effect> OrangeVignette = new(Mod.GetEffect("Effects/OrangeVignette"));
                _dataOrangeVignette = new ScreenShaderData(OrangeVignette, "OrangeVignette");
                Filters.Scene["OrangeVignette"] = new Filter(_dataOrangeVignette, EffectPriority.High);
            }
        }
        public override void ModifyLightingBrightness(ref float scale)
        {
            if (ModContent.GetInstance<Blackout>().Active)
                scale = 0.85f;
        }
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            _solarFlareBGColor = Color.Orange;
            if (ModContent.GetInstance<SolarFlare>().Active)
            {
                backgroundColor = _solarFlareBGColor;
                tileColor = _solarFlareBGColor;
            }
            else
            {
                _solarFlareBGColor = backgroundColor;
            }
            if (ModContent.GetInstance<Blackout>().Active)
            {
                backgroundColor = darkeningColor;
                tileColor = darkeningColor;

                if (darkeningColor.R > 0)
                {
                    darkeningColor.R--;
                    darkeningColor.G--;
                    darkeningColor.B--;
                }
            }
            if (Blackout.bgStopModifyTimer > 0 && !ModContent.GetInstance<Blackout>().Active)
            {
                tileColor = darkeningColor;
                backgroundColor = darkeningColor;
                if (darkeningColor.R < 150)
                {
                    darkeningColor.R++;
                    darkeningColor.G++;
                    darkeningColor.B++;
                }
            }
        }
        public override void PreSaveAndQuit()
        {
            Earthquake.SFXICrumble.Volume = 0f;
            AcidRain.SFXISizzle.Volume = 0f;
            CancerPlayer.shadersIntensity = 0f;
            DisasterPlayer.ViewingDisastersScroll = false;
            foreach (ModDisaster disaster in NDMod.ModDisasters)
            {
                disaster.SaveAndQuit();
            }
        }
        public override void PostUpdateEverything()
        {
            if (Main.keyState.KeyJustPressed(Keys.P))
            {
                // Main.NewText("Getting liquid pool...");
                // WorldUtils.GetLiquidPool((Main.MouseWorld / 16).ToPoint(), new Point(20, 20), true);
            }
            _dataWaveSolar?.UseIntensity(CancerPlayer.shadersIntensity);
            _dataOrangeVignette?.UseIntensity(CancerPlayer.shadersIntensity);
            foreach (ModDisaster disaster in NDMod.ModDisasters)
            {
                disaster.Update();

                if (disaster.Active)
                    disaster.UpdateActive();
                else
                    disaster.UpdateInactive();
            }
        }
    }
}