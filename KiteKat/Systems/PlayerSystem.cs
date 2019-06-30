using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EcsCore;
using EcsCore.Systems;
using EcsCore.Components;
using KiteKat.Components;

namespace KiteKat.Systems
{
    class PlayerSystemFilter
    {
        public Player Player;
        public Physics Physics;
        public Transform Transform;
    }

    class PlayerSystem : EcsSystem<PlayerSystemFilter>
    {
        public PlayerSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, PlayerSystemFilter components, GameTime gameTime)
        {
            var player = components.Player;
            var physics = components.Physics;
            var transform = components.Transform;

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(player.MoveLeft))
            {
                physics.Force.X -= player.MoveForce;
            }

            if (keyboardState.IsKeyDown(player.MoveRight))
            {
                physics.Force.X += player.MoveForce;
            }

            //var kiteSpring = world.GetComponent<Spring>(player.KiteEntity);
            //if (keyboardState.IsKeyDown(player.CastOut))
            //{
            //    kiteSpring.RestDistance += player.CastSpeed;
            //    if (kiteSpring.RestDistance > player.MaxCastDistance)
            //    {
            //        kiteSpring.RestDistance = player.MaxCastDistance;
            //    }
            //}

            //if (keyboardState.IsKeyDown(player.PullIn))
            //{
            //    kiteSpring.RestDistance -= player.PullSpeed;
            //    if (kiteSpring.RestDistance < player.MinPullDistance)
            //    {
            //        kiteSpring.RestDistance = player.MinPullDistance;
            //    }
            //}

            // TODO: just make a fixed component
            var ropeTransform = world.GetComponent<Transform>(player.RopeEntity);
            ropeTransform.Position = transform.Position;
        }
    }
}
