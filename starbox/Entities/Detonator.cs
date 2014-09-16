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

    class Detonator : Entity
    {
        public Detonator(Vector2i _pos)
            : base(_pos)
        {
            CurrentTex = "detonator";
            Movable = true;
        }

        bool activated = false;

        public override void Draw(RenderTexture tex, Vector2f scroll, bool shadow)
        {
            if (!activated) DrawSegment("detonatorhandle", new Vector2f(Position.X, Position.Y - 2), scroll, tex, shadow);
            else DrawSegment("detonatorhandle", new Vector2f(Position.X, Position.Y), scroll, tex, shadow);

            base.Draw(tex, scroll, shadow);

            
        }

        public override void Reset()
        {
            base.Reset();
            activated = false;
        }

        public override void Act()
        {
            base.Act();

            if (CeilingEnt != null && CeilingEnt.Movable && !activated && !Carried && !Hooked)
            {
                activated = true;

                SoundMan.PlaySound("pickup", Position);

                bool playSound = true;

                while (true)
                {

                    Entity a = EntityFinder.FindOfType(typeof(Dynamite));

                    if (a != null)
                    {
                        

                        Dynamite d = (Dynamite)a;

                        BounceC = 20;

                        if (playSound) SoundMan.PlaySound("explode", d.Position);
                        playSound = false;

                        d.BlowUp();
                        d.BlowUp();
                    }
                    else break;
                }
            }
        }
    }
}