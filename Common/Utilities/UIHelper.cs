using System;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.ModLoader;

namespace NDMod.Common.Utilities
{
    public class UIHelper
    {
        public static UIHelper GetInstance()
        {
            return ModContent.GetInstance<UIHelper>();
        }
        public Rectangle MakeUIButton(
            Vector2 position,
            string text,
            Action onLeftClick,
            ref float useScale,
            bool shouldScale = true,
            Color colorWhenHovered = default,
            float scaleMultiplier = 0.015f,
            Action hoveringAction = null,
            Color nonHoverColor = default)
        {
            useScale = MathHelper.Clamp(useScale, 0.65f, 0.85f);
            if (colorWhenHovered == default)
                colorWhenHovered = Main.highVersionColor;
            if (nonHoverColor == default)
                nonHoverColor = Color.White;
            var bounds = Main.fontDeathText.MeasureString(text);
            var rectHoverable = new Rectangle((int)position.X - (int)(bounds.X / 2 * useScale * 0.6f), (int)(position.Y - bounds.Y / 2 + 20), (int)(bounds.X * useScale * 0.6f), (int)bounds.Y - 50);
            bool hovering = rectHoverable.Contains(Main.MouseScreen.ToPoint());
            // Utils.DrawBorderString(Main.spriteBatch, Main.fontDeathText, text, position, hovering ? colorWhenHovered * alpha : nonHoverColor * alpha, 0f, bounds / 2, new Vector2(useScale), -1, 2);
            Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, text, position, hovering ? colorWhenHovered : nonHoverColor, 0f, Main.fontDeathText.MeasureString(text) / 2, new Vector2(shouldScale ? useScale * 0.4f : 0.4f), -1, 1);
            // Main.spriteBatch.Draw(mod.GetTexture("Assets/Debug/WhitePixel"), rectHoverable, Color.White * 0.1f);
            if (hoveringAction != null)
            {
                if (hovering)
                {
                    hoveringAction();
                }
            }
            if (onLeftClick != null)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    if (hovering)
                        onLeftClick();
                }
            }
            useScale += hovering ? scaleMultiplier : -scaleMultiplier;

            // hoverOld = hovering;
            // hoverOldOld = hoverNewNew;
            return rectHoverable;
        }
    }
}