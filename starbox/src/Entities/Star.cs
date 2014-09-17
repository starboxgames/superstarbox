using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace TrainBox
{
    class Star : Entity
    {
        public Star(Vector2i pos) : base(pos)
        {
            CurrentTex = "star";
            Movable = true;
        }

        int counter = new Random().Next();

        public override void Act()
        {
            base.Act();

            counter++;

            if (counter % 20 <= 5) CurrentTex = "star";
            if (counter % 20 > 5 && counter % 20 <= 10) CurrentTex = "star2";
            if (counter % 20 > 10 && counter % 20 <= 15) CurrentTex = "star3";
            if (counter % 20 > 15) CurrentTex = "star2";
        }
    }
}
