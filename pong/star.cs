using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace YourGameNamespace
{
    public class Star
    {
        public Vector2 Position { get; private set; }
        public float Radius { get; private set; }
        public Color Color { get; private set; }

        public Star(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight, Random rand)
        {
            Position = new Vector2(rand.Next(screenWidth), rand.Next(screenHeight));
            Radius = (float)(rand.NextDouble() * 2 + 1); // 1~3 像素的星星
            byte brightness = (byte)rand.Next(150, 256); // 比较亮
            Color = new Color(brightness, brightness, brightness);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0f,
                new Vector2(texture.Width / 2f, texture.Height / 2f),
                Radius / (texture.Width / 2f), SpriteEffects.None, 0f);
        }
    }
}
