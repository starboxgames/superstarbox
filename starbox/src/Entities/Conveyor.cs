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
    class Conveyor : Entity
    {
        public Conveyor(Vector2i pos) : base(pos)
        {
            Movable = false;
            CurrentTex = "conv1";
            Flipped = false;
        }

        int c = 0;

        public override void Act()
        {
            Entity ent;

            //ent = GetEnt(entities, new Vector2f(Position.X, Position.Y - 2));

            ent = CeilingEnt;

            if (ent != null)
            {
                if (!Flipped) Push(ent, EntityFinder.Dir.Left, 0.25f);
                else Push(ent, EntityFinder.Dir.Right, 0.25f);
            }

            base.Act();

            c++;

            if (c % 10 < 5) CurrentTex = "conv2";
            else CurrentTex = "conv1";
        }

        public override void Draw(RenderTexture tex, Vector2f scroll, bool shadow)
        {
            base.Draw(tex, scroll, shadow);

            DrawSegment("convrail", new Vector2f(Position.X, Position.Y - 16), scroll, tex, shadow);
        }
    }
}
