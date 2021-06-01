using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;

namespace NDMod.Common.Utilities
{
    public static class EntityHelper
    {
        public static void SetRunSpeeds(this Player player, float moveSpeed, float runAccel, float accRunSpeed)
        {
            // player.maxRunSpeed
            // player.runAcceleration
            // player.accRunSpeed
            player.maxRunSpeed = moveSpeed == -1 ? player.moveSpeed : moveSpeed;
            player.runAcceleration = runAccel == -1 ? player.runAcceleration : runAccel;
            player.accRunSpeed = accRunSpeed == -1 ? player.accRunSpeed : accRunSpeed;

            /*bool IsInPostUpdateRunSpeeds()
            {
                typeof(NDMod).Assembly.GetType("").GetMethod("").;
                return true;
            }
            return IsInPostUpdateRunSpeeds();*/
        }
    }
}