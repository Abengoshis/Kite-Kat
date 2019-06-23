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
    class KiteFilter
    {
        public Kite Kite;
        public Transform Transform;
        public Physics Physics;
    }

    class KiteSystem : EcsSystem<KiteFilter>
    {
        public KiteSystem(EcsWorld world) : base(world)
        {

        }

        protected override void Process(int entity, KiteFilter components, GameTime gameTime)
        {
            var kite = components.Kite;
            var transform = components.Transform;

            var playerTransform = world.GetComponent<Transform>(kite.PlayerEntity);
            var lineToPlayer = playerTransform.Position - transform.Position;

            var rotationAway = Math.Atan2(lineToPlayer.X, lineToPlayer.Y);

            var rotationToward = Math.Sign(lineToPlayer.X) * Math.Atan2(lineToPlayer.Y, Math.Abs(lineToPlayer.X));

            var styleMix = (MathHelper.PiOver2 - Math.Abs(rotationAway)) / MathHelper.PiOver2;

            transform.Rotation = MathHelper.Lerp((float)rotationToward, (float)rotationAway, (float)styleMix);

            // TODO: just make a fixed component
            var ropeTransform = world.GetComponent<Transform>(kite.RopeEntity);
            ropeTransform.Position = transform.Position;

            var ropePhysics = world.GetComponent<Physics>(kite.RopeEntity);
            ropePhysics.Velocity = Vector2.Zero;
            Console.WriteLine(ropePhysics.Velocity);
        }
    }
}
