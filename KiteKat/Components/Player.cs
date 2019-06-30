using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EcsCore.Components;

namespace KiteKat.Components
{
    class Player : IEcsComponent
    {
        public int RopeEntity;
        public int KiteEntity;

        public float MoveForce;
        public Keys MoveLeft;
        public Keys MoveRight;

        public float MaxCastDistance;
        public float CastSpeed;
        public Keys CastOut;

        public float MinPullDistance;
        public float PullSpeed;
        public Keys PullIn;

        public Player()
        {
        }
    }
}
