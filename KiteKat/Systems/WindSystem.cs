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
    class WindSystemFilter
    {
        public Drift Drift;
        public Physics Physics;
    }

    class WindSystem : EcsSystem<WindSystemFilter>
    {
        public Vector2 Strength;

        public WindSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, WindSystemFilter components, GameTime gameTime)
        {
            var drift = components.Drift;
            var physics = components.Physics;
            physics.Force += drift.Blowability * Strength;
        }
    }
}
