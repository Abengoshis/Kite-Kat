using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using EcsCore.Components;

namespace KiteKat.Components
{
    class Drift : IEcsComponent
    {
        public Vector2 Blowability;

        public Drift(Vector2 blowability)
        {
            Blowability = blowability;
        }

        public Drift() : this(Vector2.One)
        {
        }
    }
}
