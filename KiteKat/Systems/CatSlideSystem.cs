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
    class CatSlideFilter
    {
        public CatSlide CatSlide;
        public Transform Transform;
    }

    class CatSlideSystem : EcsSystem<CatSlideFilter>
    {
        private float slideSpeed = 10f;

        public CatSlideSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, CatSlideFilter components, GameTime gameTime)
        {
            var catSlide = components.CatSlide;
            var transform = components.Transform;

            var playerTransform = world.GetComponent<Transform>(catSlide.PlayerEntity);
            if (transform.Position.Y >= playerTransform.Position.Y)
            {
                // If the cat is below the player, stop sliding.
                world.RemoveComponent<CatSlide>(entity);
                world.AddComponent<CatAbscond>(entity);
            }
            else
            {
                catSlide.SegmentProgress += slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Advance the current segment down the rope.
                while (catSlide.SegmentProgress >= 1f)
                {
                    catSlide.ConnectedEntity = world.GetComponent<Rope>(catSlide.ConnectedEntity).PreviousEntity;

                    // If the next rope entity is the player's rope, stop sliding.
                    var player = world.GetComponent<Player>(catSlide.PlayerEntity);
                    if (catSlide.ConnectedEntity == player.RopeEntity)
                    {
                        catSlide.SegmentProgress = 0f;
                    }
                    else
                    {
                        --catSlide.SegmentProgress;
                    }
                }

                var rope = world.GetComponent<Rope>(catSlide.ConnectedEntity);

                var ropeTransform = world.GetComponent<Transform>(catSlide.ConnectedEntity);

                var lastRopeTransform = world.GetComponent<Transform>(rope.NextEntity);

                Physics ropePhysics;
                if (world.TryGetComponent<Physics>(catSlide.ConnectedEntity, out ropePhysics))
                {
                    ropePhysics.Force += Vector2.UnitY * 300f;
                }

                transform.Position = Vector2.Lerp(lastRopeTransform.Position, ropeTransform.Position, catSlide.SegmentProgress);
            }
        }
    }
}
