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

    class StarBlock : Entity
    {
        public StarBlock(Vector2i _pos)
            : base(_pos)
        {
            CurrentTex = "starblock";
            Movable = true;

            StarOnTop = false;
        }

        int counter = new Random().Next();

        public bool StarOnTop { get; set; }

        public override void Act()
        {
            counter++;

            base.Act();

            if (CeilingEnt != null && CeilingEnt is Star)
            {
                StarOnTop = true;
            }
            else StarOnTop = false;

            if (StarOnTop)
            {
                if (counter % 50 == 0 || counter % 50 == 40)
                {
                    CeilingEnt.BounceC = 8;
                }


                if (counter % 10 < 5)
                {
                    CurrentTex = "starblock";
                }
                else CurrentTex = "starblock2";
            }
            else CurrentTex = "starblock";
        }
    }
}