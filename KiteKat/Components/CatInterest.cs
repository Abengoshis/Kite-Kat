using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcsCore.Components;

namespace KiteKat.Components
{
    class CatInterest : IEcsComponent
    {
        // How quickly does the cat gain interest?
        public float InterestRate;

        // How quickly does the cat lose interest?
        public float DisinterestRate;

        // How interested is the cat?
        public float Interest;
    }
}
