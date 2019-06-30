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
    class CatInterestFilter
    {
        public CatInterest CatInterest;
        public Transform Transform;
    }

    class CatInterestSystem : EcsSystem<CatInterestFilter>
    {
        // How close to a cat must a kite be to pull aggro?
        private float interestRadius = 120f;

        public CatInterestSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, CatInterestFilter components, GameTime gameTime)
        {
            var transform = components.Transform;
            var catInterest = components.CatInterest;

            // TODO: See TODO in EcsSystem regarding multiple filters per system, primary filters and secondary filters.
            var kiteEntities = world.GetEntities<Kite>().ToList();
            int closestKiteEntity = -1;
            Transform closestKiteTransform = null;
            var closestKiteDistanceSquared = interestRadius * interestRadius;
            foreach (var kiteEntity in kiteEntities)
            {
                var kiteTransform = world.GetComponent<Transform>(kiteEntity);
                var kiteDistanceSquared = Vector2.DistanceSquared(kiteTransform.Position, transform.Position);
                if (kiteDistanceSquared < closestKiteDistanceSquared)
                {
                    closestKiteDistanceSquared = kiteDistanceSquared;
                    closestKiteTransform = kiteTransform;
                    closestKiteEntity = kiteEntity;
                }
            }

            if (closestKiteEntity >= 0f)
            {
                catInterest.Interest += catInterest.InterestRate * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Start pouncing when the cat is fully interested.
                if (catInterest.Interest >= 1f)
                {
                    var catPounce = world.AddComponent<CatPounce>(entity);
                    catPounce.Start = transform.Position;
                    catPounce.KiteEntity = closestKiteEntity;
                    catPounce.Height = 40f;

                    // No longer need to track interest.
                    world.RemoveComponent<CatInterest>(entity);
                }
            }
            else
            {
                catInterest.Interest -= catInterest.DisinterestRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (catInterest.Interest < 0f)
                {
                    catInterest.Interest = 0f;
                }
            }
        }

    }
}
