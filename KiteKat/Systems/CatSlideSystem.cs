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
        private float slideSpeed = 150f;

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
                catSlide.Progress += slideSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                var kiteEntity = catSlide.KiteEntity;
                var kite = world.GetComponent<Kite>(kiteEntity);

                // Find the rope transform and next rope transform at the current distance.
                var currentEntity = kiteEntity;
                var nextEntity = kite.RopeEntity;
                Transform currentTransform = world.GetComponent<Transform>(currentEntity);
                Transform nextTransform = world.GetComponent<Transform>(nextEntity);
                var segmentDistance = 0f;
                var cumulativeDistance = 0f;
                while (cumulativeDistance < catSlide.Progress)
                {
                    if (nextEntity == catSlide.PlayerEntity)
                    {
                        catSlide.Progress = cumulativeDistance;
                    }
                    else
                    {
                        currentEntity = nextEntity;
                        var currentRope = world.GetComponent<Rope>(currentEntity);
                        nextEntity = currentRope.PreviousEntity;
                    }

                    currentTransform = world.GetComponent<Transform>(currentEntity);
                    nextTransform = world.GetComponent<Transform>(nextEntity);

                    segmentDistance = Vector2.Distance(currentTransform.Position, nextTransform.Position);
                    cumulativeDistance += segmentDistance;
                }

                // Move the distance back to the distance of the rope transform closer towards the kite.
                cumulativeDistance -= segmentDistance;

                if (segmentDistance > 0)
                {
                    // Determine the progress along the current segment.
                    var segmentProgress = (catSlide.Progress - cumulativeDistance) / segmentDistance;
                    transform.Position = Vector2.Lerp(currentTransform.Position, nextTransform.Position, segmentProgress);
                }
                else
                {
                    transform.Position = currentTransform.Position;
                }
            }
        }
    }
}
