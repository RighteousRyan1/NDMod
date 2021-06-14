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
        public override int MaxDuration => 15500;
        public override float ChanceToOccur => 0.00001f;

        private static float _windSpeedApproachable;
        private static int _timerWindSpeedChange;
        public override bool OnEnd()
        {
            _timerWindSpeedChange = 300;
            _windSpeedApproachable = Main.rand.NextFloat(-0.5f, 0.5f);
            return base.OnEnd();
        }
        public override bool OnBegin()
        {
            _windSpeedApproachable = Main.rand.NextFloat(-3f, 3f);

            if (_windSpeedApproachable > -2f && _windSpeedApproachable <= 0)
                _windSpeedApproachable = -4f;
            else if (_windSpeedApproachable > 0)
                _windSpeedApproachable = 2f;
            return base.OnBegin();
        }
        public override void UpdateActive()
        {
            Main.maxRaining = 1f;
            Main.numClouds = 60;
            for (int i = 0; i < Main.ActivePlayersCount; i++)
            {
                var player = Main.player[i];
                if (!player.behindBackWall && player.ZoneOverworldHeight)
                    player.AddBuff(BuffID.WindPushed, 2, true);
            }

            MathMethods.RoughStep(ref Main.windSpeed, _windSpeedApproachable, 0.0025f);
        }
        public override void UpdateInactive()
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
        public override int MinDuration => 11250;
    }
}