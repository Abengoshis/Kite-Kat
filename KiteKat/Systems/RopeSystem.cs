using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using KiteKat.EcsCore;
using KiteKat.EcsCore.Systems;
using KiteKat.EcsCore.Components;
using KiteKat.Components;

namespace KiteKat.Systems
{
    class RopeFilter
    {
        public Rope Rope;
        public Transform Transform;
        public Sprite Sprite;
    }

    class RopeSystem : EcsSystem<RopeFilter>
    {

        public RopeSystem(EcsWorld world) : base(world)
        {

        }

        protected override void Process(int entity, RopeFilter components, GameTime gameTime)
        {
            var rope = components.Rope;
            var transform = components.Transform;
            var sprite = components.Sprite;

            var attachedTransform = world.GetComponent<Transform>(rope.PreviousEntity);
            var line = attachedTransform.Position - transform.Position;
            var distance = line.Length() / sprite.Texture.Width;

            if (!world.HasComponent<Rope>(rope.PreviousEntity)) {
                distance = 0;
            }

            sprite.Scale = new Vector2(distance, sprite.Scale.Y);
            transform.Rotation = (float)Math.Atan2(-line.Y, -line.X);
        }
    }
}
