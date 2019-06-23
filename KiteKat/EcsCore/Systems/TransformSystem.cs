using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KiteKat.EcsCore;
using KiteKat.EcsCore.Components;

namespace KiteKat.EcsCore.Systems
{
    class TransformSystemFilter
    {
        public Transform Transform;
    }

    class TransformSystem : EcsSystem<TransformSystemFilter>
    {
        public TransformSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, TransformSystemFilter components, GameTime gameTime)
        {
            var transform = components.Transform;

            var parentEntity = world.GetParent(entity);
            var parentTransform = world.GetComponent<Transform>(parentEntity);

            // Calculate the global pose from the local position and rotation, and the parent's global pose.
            var translation = Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0);
            var rotation = Matrix.CreateRotationZ(transform.Rotation);
            Matrix.Multiply(ref rotation, ref translation, out transform.LocalPose);
            Matrix.Multiply(ref transform.LocalPose, ref parentTransform.GlobalPose, out transform.GlobalPose);
        }
    }
}