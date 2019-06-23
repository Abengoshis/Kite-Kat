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
    class SpringSystemFilter
    {
        public Spring Spring;
        public Physics Physics;
        public Transform Transform;
    }

    class SpringSystem : EcsSystem<SpringSystemFilter>
    {
        public SpringSystem(EcsWorld world) : base(world) 
        {
        }

        protected override void Process(int entity, SpringSystemFilter components, GameTime gameTime)
        {
            var spring = components.Spring;
            var physics = components.Physics;
            var transform = components.Transform;
            var connectedTransform = world.GetComponent<Transform>(spring.ConnectedEntity);

            // Calculate the displacement from the other transform.
            var displacement = transform.Position - connectedTransform.Position;

            // If the entity's rotation is fixed on the spring, rotate it through the displacement.
            if (spring.FixedRotation != null)
            {
                transform.Rotation = spring.FixedRotation.Value + (float)Math.Atan2(-displacement.X, displacement.Y);
            }

            // Add the rest origin.
            displacement.X += spring.RestOrigin.X * -(float)Math.Cos(transform.Rotation) + spring.RestOrigin.Y * (float)Math.Sin(transform.Rotation);
            displacement.Y += spring.RestOrigin.Y * -(float)Math.Cos(transform.Rotation) - spring.RestOrigin.X * (float)Math.Sin(transform.Rotation);

            // Add the required resting vector.
            if (spring.RestDistance != 0)
            {
                Vector2 restDirection;
                if (spring.RestAngle != null)
                {
                    float localRestAngle = connectedTransform.Rotation + spring.RestAngle.Value;
                    restDirection.X = (float)Math.Sin(localRestAngle);
                    restDirection.Y = (float)-Math.Cos(localRestAngle);
                }
                else if (displacement.LengthSquared() != 0)
                {
                    Vector2.Normalize(ref displacement, out restDirection);
                    Vector2.Negate(ref restDirection, out restDirection);
                }
                else
                {
                    // Prevent sticking or crashing when the distance is 0.
                    restDirection = -Vector2.UnitY;
                }
                displacement += restDirection * spring.RestDistance;
            }

            // Hooke's Law.
            var force = -spring.Stiffness * displacement;
            if (spring.MaxForce != null && force.LengthSquared() > spring.MaxForce.Value * spring.MaxForce.Value)
            {
                Vector2.Normalize(ref force, out force);
                Vector2.Multiply(ref force, spring.MaxForce.Value, out force);
            }

            physics.Force += force - spring.Damping * physics.Velocity;

            // Act on the other entity if it also has physics.
            Physics connectedPhysics;
            if (world.TryGetComponent(spring.ConnectedEntity, out connectedPhysics))
            {
                connectedPhysics.Force -= force - spring.Damping * connectedPhysics.Velocity;
            }
        }
    }
}
