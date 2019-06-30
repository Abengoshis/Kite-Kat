using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcsCore.Components;

namespace KiteKat.Components
{
    class Rope : IEcsComponent
    {
        public int PreviousEntity;
        public int NextEntity;
        public int Index;
    }
}
