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
    class CatAbscondFilter
    {
        public CatAbscond CatAbscond;
        public Spring Spring;
        public Transform Transform;
    }

    class CatAbscondSystem : EcsSystem<CatAbscondFilter>
    {
        private float runSpeed = 10f;

        public CatAbscondSystem(EcsWorld world) : base(world)
        {
        }

        protected override void Process(int entity, CatAbscondFilter components, GameTime gameTime)
        {
            var transform = components.Transform;

            if (transform.Position.X < KiteKatGame.WIDTH / 2)
            {

            }
        }
    }
}
