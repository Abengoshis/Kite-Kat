using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using EcsCore.Components;

namespace KiteKat.Components
{
    class CatSlide : IEcsComponent
    {
        public int KiteEntity;
        public int PlayerEntity;
        public float Progress;
    }
}
