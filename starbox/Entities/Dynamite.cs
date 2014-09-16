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

    class Dynamite : Entity
    {
        public Dynamite(Vector2i _pos)
            : base(_pos)
        {
            CurrentTex = "dynamite";
            Movable = true;
        }

        public void BlowUp()
        {

            ScreenShake = true;

            Entity a = EntityFinder.GetEnt(new Vector2f(Position.X - 16, Position.Y));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X + 16, Position.Y));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X, Position.Y - 16));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X, Position.Y + 16));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X - 16, Position.Y + 16));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X - 16, Position.Y - 16));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X + 16, Position.Y - 16));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;

            a = EntityFinder.GetEnt(new Vector2f(Position.X + 16, Position.Y + 16));
            if (a != null && !(a is Dynamite)) a.Destroyed = true;


            Destroyed = true;
        }
    }
}