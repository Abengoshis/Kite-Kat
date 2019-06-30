using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using EcsCore;
using EcsCore.Systems;
using EcsCore.Components;
using KiteKat.Components;

namespace KiteKat.Systems
{
    class PhysicsSystemFilter
    {
        public Physics Physics;
        public Transform Transform;
    }

    class PhysicsSystem : EcsSystem<PhysicsSystemFilter>
    {
        private float forceMultiplier = 1000f; // 1kN

        public PhysicsSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, PhysicsSystemFilter components, GameTime gameTime)
        {
            var physics = components.Physics;
            var transform = components.Transform;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 acceleration = (physics.Force * deltaTime - 0.5f * physics.Drag * physics.Velocity) * physics.InverseMass * forceMultiplier;
            Vector2 position = transform.Position + (physics.Velocity + acceleration * deltaTime * deltaTime) * physics.Multiplier;
            physics.Velocity = position - transform.Position;
            transform.Position = position;

            // Clear impulse forces.
            physics.Force = physics.ConstantForce;
        }
    }
}
