using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace TrainBox
{
    class KeyBlock : Entity
    {
        public KeyBlock(Vector2i pos)
            : base(pos)
        {
            CurrentTex = "keyblock";
            Movable = false;
        }
    }
}
