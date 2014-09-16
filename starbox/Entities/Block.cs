namespace TrainBox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using SFML;
    using SFML.Graphics;
    using SFML.Window;

    class Block : Entity
    {
        public Block(Vector2i _pos)
        : base(_pos)
        {
            CurrentTex = "block";
            Movable = false;
        }
    }
}