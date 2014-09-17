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

    class Exit : Entity
    {
        public bool Open { get; set; }

        public Exit(Vector2i _pos)
            : base(_pos)
        {
            CurrentTex = "exit";
            Movable = false;
            Open = false;
        }


        public override void Act()
        {
            if (Open)
            {
                CurrentTex = "exit1";
            }
            else CurrentTex = "exit";

            base.Act();
        }
    }
}