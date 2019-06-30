using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using EcsCore.Components;

namespace KiteKat.Components
{
    class CatPounce : IEcsComponent
    {
        // Where did the cat pounce from?
        public Vector2 Start;

        // Which kite is the cat pouncing to?
        public int KiteEntity;

        // How far has the cat's pounce progressed?
        public float Progress;

        // How steep is the pounce incline?
        public float Height;
    }
}
