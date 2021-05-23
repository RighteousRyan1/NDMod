using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;

namespace NDMod.Common.Utilities
{
    public static class ArrayHelper
    {
        public static object[,] To2DArray(this Vector2 input)
        {
            var twoD = new object[,]
            {
                { (int)input.X, (int)input.Y }
            };
            return twoD;
        }
        public static Tile ToTile(this Vector2 input)
        {
            Tile tile = Main.tile[(int)input.X / 16, (int)input.Y / 16];
            return tile;
        }
    }
}