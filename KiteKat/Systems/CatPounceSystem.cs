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
using KiteKat.Helpers;

namespace KiteKat.Systems
{
    class CatPounceFilter
    {
        public CatPounce CatPounce;
        public Transform Transform;
    }

    class CatPounceSystem : EcsSystem<CatPounceFilter>
    {
        private float pounceSpeed = 2f;

        public CatPounceSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, CatPounceFilter components, GameTime gameTime)
        {
            var catPounce = components.CatPounce;
            var transform = components.Transform;

            catPounce.Progress += pounceSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // If the cat has finished pouncing, start sliding.
            if (catPounce.Progress >= 1f)
            {
                var kiteEntity = catPounce.KiteEntity;
                var kite = world.GetComponent<Kite>(kiteEntity);

                var catSlide = world.AddComponent<CatSlide>(entity);
                catSlide.KiteEntity = kiteEntity;
                catSlide.PlayerEntity = kite.PlayerEntity;
                catSlide.ConnectedEntity = kite.RopeEntity;

                // Make sure the cat doesn't overshoot the kite.
                catPounce.Progress = 1f;

                // Stop pouncing.
                world.RemoveComponent<CatPounce>(entity);
            }

            // Interpolate the position with a vertical curve.
            var endTransform = world.GetComponent<Transform>(catPounce.KiteEntity);
            var endPosition = endTransform.Position;
            transform.Position = new Vector2(
                MathHelper.Lerp(catPounce.Start.X, endPosition.X, Ease.SineInOut(catPounce.Progress)),
                MathHelper.Lerp(catPounce.Start.Y, endPosition.Y, catPounce.Progress) - catPounce.Height * (float)Math.Sin(catPounce.Progress * Math.PI)
            );
        }
    }
}
