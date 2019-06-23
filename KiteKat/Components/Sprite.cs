using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KiteKat.EcsCore.Components;

namespace KiteKat.Components
{
    class Sprite : IEcsComponent
    {
        public Texture2D Texture;
        public Rectangle? Frame;
        public Color Tint;
        public Vector2 Origin;
        public Vector2 Scale;
        public float LayerDepth;

        public Sprite(Texture2D texture, Rectangle? frame, Color tint, Vector2 origin, Vector2 scale, float layerDepth)
        {
            Texture = texture;
            Frame = frame;
            Tint = tint;
            Origin = origin;
            Scale = scale;
            LayerDepth = layerDepth;
        }

        public Sprite() : this(null, null, Color.White, Vector2.Zero, Vector2.One, 0f)
        {
        }
    }
}
