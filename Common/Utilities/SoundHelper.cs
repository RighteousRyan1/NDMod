using System;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using NDMod.Content.ModPlayers;

namespace NDMod.Common.Utilities
{
	public static class SoundHelper
	{
		public static float GetPanFromPosition(this Vector2 position)
		{
			try
			{
				bool onScreen = false;
				float panFromVector = 0f;
				if (position.X == -1 || position.Y == -1)
				{
					onScreen = true;
				}
				else
				{
					Rectangle screenBounds = new Rectangle((int)(Main.screenPosition.X - (Main.screenWidth * 2)), (int)(Main.screenPosition.Y - (Main.screenHeight * 2)), Main.screenWidth * 5, Main.screenHeight * 5);
					Rectangle rectFromVect = new Rectangle((int)position.X, (int)position.Y, 1, 1);
					Vector2 midScreen = new Vector2(Main.screenPosition.X + Main.screenWidth / 2, Main.screenPosition.Y + Main.screenHeight / 2);
					if (rectFromVect.Intersects(screenBounds))
						onScreen = true;
					if (onScreen)
					{
						panFromVector = (position.X - midScreen.X) / (Main.screenWidth * 0.5f);
						float absPanX = Math.Abs(position.X - midScreen.X);
						float absPanY = Math.Abs(position.Y - midScreen.Y);
						float panSQRT = (float)Math.Sqrt(absPanX * absPanX + absPanY * absPanY);
					}
				}
				if (panFromVector < -1f)
				{
					panFromVector = -1f;
				}
				if (panFromVector > 1f)
				{
					panFromVector = 1f;
				}
				return panFromVector;
			}
			catch(Exception e)
            {
				ModContent.GetInstance<NDMod>().Logger.Error($"{e.Message}\n{e.StackTrace}");
            }
			return 0f;
		}
		public static float GetVolumeFromPosition(this Vector2 position)
		{
			try
			{
				bool onScreen = false;
				float volumeFromVector = 1f;
				if (position.X == -1 || position.Y == -1)
				{
					onScreen = true;
				}
				else
				{
					Rectangle screenBounds = new Rectangle((int)(Main.screenPosition.X - (Main.screenWidth * 2)), (int)(Main.screenPosition.Y - (Main.screenHeight * 2)), Main.screenWidth * 5, Main.screenHeight * 5);
					Rectangle rectFromVect = new Rectangle((int)position.X, (int)position.Y, 1, 1);
					Vector2 midScreen = new Vector2(Main.screenPosition.X + Main.screenWidth / 2, Main.screenPosition.Y + Main.screenHeight / 2);
					if (rectFromVect.Intersects(screenBounds))
						onScreen = true;
					if (onScreen)
					{
						float num4 = Math.Abs(position.X - midScreen.X);
						float num5 = Math.Abs(position.Y - midScreen.Y);
						float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
						volumeFromVector = 1f - num6 / ((float)Main.screenWidth * 1.5f);
					}
				}
				if (volumeFromVector > 1f)
				{
					volumeFromVector = 1f;
				}
				if (volumeFromVector < 0f)
				{
					volumeFromVector = 0f;
				}
				if (Vector2.Distance(Main.screenPosition, position) > 3850)
					volumeFromVector = 0f;
				return volumeFromVector;
			}
			catch (Exception e)
			{
				ModContent.GetInstance<NDMod>().Logger.Error($"{e.Message}\n{e.StackTrace}");
			}
			return 0f;
		}
	}
}