using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KiteKat.EcsCore;
using KiteKat.EcsCore.Systems;
using KiteKat.EcsCore.Components;
using KiteKat.Components;

namespace KiteKat.Systems
{
    class SpriteSystemFilter
    {
        public Sprite Sprite;
        public Transform Transform;
    }

    class SpriteSystem : EcsSystem<SpriteSystemFilter>
    {
        private SpriteBatch spriteBatch;

        public SpriteSystem(EcsWorld world, SpriteBatch spriteBatch) : base(world)
        {
            this.spriteBatch = spriteBatch;
        }

        protected override void Process(int entity, SpriteSystemFilter components, GameTime gameTime)
        {
            var sprite = components.Sprite;
            var transform = components.Transform;

            // Decompose the global pose into usable values. (Maybe put these in the transform for use?)
            var globalPosition = new Vector2(transform.GlobalPose.Translation.X, transform.GlobalPose.Translation.Y);
            var globalRight = Vector2.Transform(Vector2.UnitX, transform.GlobalPose.Rotation);
            var globalRotation = (float)Math.Atan2(globalRight.Y, globalRight.X);

            spriteBatch.Draw(
                sprite.Texture,
                globalPosition,
                sprite.Frame,
                sprite.Tint,
                globalRotation,
                sprite.Origin,
                sprite.Scale,
                SpriteEffects.None,
                sprite.LayerDepth);
        }
    }
}
