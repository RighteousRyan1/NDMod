using System;
using System.Linq;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna;
using System.Collections.Generic;
using System.Reflection;

namespace NDMod.Common.Utilities
{
    public static class MathMethods
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
	}
}