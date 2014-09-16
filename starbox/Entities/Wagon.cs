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

    class Wagon : Entity
    {
        public Wagon(Vector2i _pos)
            : base(_pos)
        {
            CurrentTex = "wagon";
            Movable = true;
        }

        bool activated = false;

        int counter;

        public override void Reset()
        {
            base.Reset();
            activated = false;
        }

        public override void Act()
        {
            base.Act();

            counter++;

            //Entity ent = GetEnt(entities, new Vector2f(Position.X, Position.Y - 16));
            Entity ent = CeilingEnt;

            if (ent != null && !ent.Movable) ent = null;

            if (ent != null && !Flipped && BlockedRightEnt == null) activated = true;
            if (ent != null && Flipped && BlockedLeftEnt == null) activated = true;

            if (ent == null && ((BlockedLeftEnt != null && Flipped) || 
                                (BlockedRightEnt != null && !Flipped))) activated = false;

            if (Carried || Hooked) activated = false;

            if (activated)
            {

                if (counter % 7 == 0) SoundMan.PlaySound("drop", Position);

                if (!Flipped) Push(this, EntityFinder.Dir.Right, 0.4f);
                if (Flipped) Push(this, EntityFinder.Dir.Left, 0.4f);                
            }

        }
    }
}