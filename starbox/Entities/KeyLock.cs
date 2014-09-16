using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace TrainBox
{
    class KeyLock : Entity
    {
        public KeyLock(Vector2i pos)
            : base(pos)
        {
            CurrentTex = "keylock";
            Movable = false;
        }
    }
}
