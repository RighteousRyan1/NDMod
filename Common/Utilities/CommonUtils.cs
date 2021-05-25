using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace NDMod.Common.Utilities
{
    public static class CommonUtils
    {
        public struct ChancesChart
        {
            public const float UltraCommon = 0.05f;
            public const float VeryCommon = 0.025f;
            public const float DecentlyCommon = 0.01f;
            public const float Common = 0.005f;
            public const float Rare = 0.0025f;
            public const float VeryRare = 0.001f;
            public const float UltraRare = 0.00075f;
        }
        public static bool OnKeyPressed(this KeyboardState state, Keys key)
        {
            return state.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
        }
        public static bool OnKeyRelease(this KeyboardState state, Keys key)
        {
            return !state.IsKeyDown(key) && Main.oldKeyState.IsKeyDown(key);
        }
        public static bool AreKeysPressed(this KeyboardState state, params Keys[] keys)
        {
            bool shouldReturnTrue = keys.All(k => state.IsKeyDown(k));
            return shouldReturnTrue;
        }

        public static T Pick<T>(params T[] values)
        {
            return values[Main.rand.Next(values.Length)];
        }
    }
}