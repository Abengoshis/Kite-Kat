using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KiteKat.Debugging
{
    static class DebugDrawing
    {
        private static Texture2D pixel;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
        }

        public static void DrawLine(SpriteBatch spritebatch, Vector2 start, Vector2 end, Color tint, float thickness = 1)
        {
            Vector2 delta = end - start;
            float angle = (float)Math.Atan2(delta.Y, delta.X);
            spritebatch.Draw(pixel, start, null, tint, angle, new Vector2(0f, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);
        }
    }
}
