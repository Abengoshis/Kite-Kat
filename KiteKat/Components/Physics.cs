using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using KiteKat.EcsCore.Components;

namespace KiteKat.Components
{
    class Physics : IEcsComponent
    {
        private float mass;
        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                if (value != 0f)
                {
                    mass = value;
                    InverseMass = 1f / value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Physical entities must have mass (or anti-mass).");
                }
            }
        }

        public float InverseMass
        {
            get; private set;
        }

        public Vector2 Velocity;
        public Vector2 Force;
        public Vector2 ConstantForce;
        public Vector2 Drag;
        public Vector2 Multiplier;

        public Physics(float mass)
        {
            Mass = mass;
            Multiplier = Vector2.One;
        }

        public Physics() : this(1f)
        {
        }
    }
}
