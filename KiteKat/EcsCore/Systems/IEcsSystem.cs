using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KiteKat.EcsCore.Systems
{
    public interface IEcsSystem
    {
        void Process(GameTime gameTime);
    }
}
