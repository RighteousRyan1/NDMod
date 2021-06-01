using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ID;
using NDMod.Common;
using NDMod.Content.ModPlayers;
using Terraria.ModLoader;
using NDMod.Content.Buffs;
using NDMod.Content.Projectiles;
using NDMod.Common.Utilities;

namespace NDMod.Content.Disasters
{
    public class Hurricane : ModDisaster
    {
        public override int MaxDuration => 7500;
        public override float ChanceToOccur => 0.00001f;

        private static float _windSpeedApproachable;
        private static int _timerWindSpeedChange;
        public override bool OnEnd()
        {
            _timerWindSpeedChange = 300;
            _windSpeedApproachable = Main.rand.NextFloat(-0.5f, 0.5f);
            Main.NewText("The wind is getting lighter.", Color.DarkSlateBlue);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            _windSpeedApproachable = Main.rand.NextFloat(-5f, 5f);

            if (_windSpeedApproachable > -2f && _windSpeedApproachable <= 0)
                _windSpeedApproachable = -4f;
            else if (_windSpeedApproachable > 0)
                _windSpeedApproachable = 2f;
            Main.NewText("The wind is starting to speed!", Color.DarkBlue);
            return base.OnBegin();
        }
        public override void UpdateActive(ModDisaster disaster)
        {
            Main.maxRaining = 1f;
            Main.numClouds = 30;
            // Main.NewText($"{Main.windSpeed} : {_windSpeedApproachable}");
            for (int i = 0; i < Main.ActivePlayersCount; i++)
            {
                var player = Main.player[i];

                player.AddBuff(BuffID.WindPushed, 2, true);
            }

            MathMethods.RoughStep(ref Main.windSpeed, _windSpeedApproachable, 0.0025f);
        }
        public override void UpdateInactive(ModDisaster disaster)
        {
            if (_timerWindSpeedChange > 0)
            {
                _timerWindSpeedChange--;
                MathMethods.RoughStep(ref Main.windSpeed, _windSpeedApproachable, 0.075f);
            }
        }
        public override string Name => "Hurricane";
        public override bool CanActivate => Main.raining;
        public override int Cooldown => 6000;
        public override int MinDuration => 4250;
    }
}