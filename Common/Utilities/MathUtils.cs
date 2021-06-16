using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace NDMod.Common.Utilities
{
    public static class MathUtils
    {
		public static float RoughStep(ref float value, float goal, float step)
		{
			if (value < goal)
			{
				value += step;

				if (value > goal)
				{
					return goal;
				}
			}
			else if (value > goal)
			{
				value -= step;

				if (value < goal)
				{
					return goal;
				}
			}

			return value;
		}
		public static Color ColorStep(ref Color value, Color goal, byte step)
        {
			if (value.R > goal.R)
				value.R -= step;
			else
				value.R += step;

			if (value.G > goal.G)
				value.G -= step;
			else
				value.G += step;

			if (value.B > goal.B)
				value.B -= step;
			else
				value.B += step;

			return value;
		}
		public static Color GetAverageColor(this Texture2D texture)
		{
			try
			{
				Color[] colorBuffer = texture.GetColors();
				int avgR = 0;
				int avgG = 0;
				int avgB = 0;
				int cLen = colorBuffer.Length;

				for (int i = 0; i < colorBuffer.Length; i++)
				{
					var c = colorBuffer[i];
					if (c.A != 255)
					{
						cLen--;
						continue;
					}

					avgR += c.R;
					avgG += c.G;
					avgB += c.B;
				}
				return new Color((byte)(avgR / cLen), (byte)(avgG / cLen), (byte)(avgB / cLen));
			}
			catch(DivideByZeroException dbze)
            {
				var mod = Terraria.ModLoader.ModContent.GetInstance<NDMod>();

				mod.Logger.Error($"{dbze.GetType().Name} thrown in NDMod.Common.Utilities.MathUtils.GetAverageColor() -> \n{dbze.Message}\n{dbze.StackTrace}");
            }
			return Color.White;
		}
		public static Color[] GetColors(this Texture2D tex)
		{
			Color[] colorBuffer = new Color[tex.Width * tex.Height];
			tex.GetData(colorBuffer);
			return colorBuffer;
		}
	}
}