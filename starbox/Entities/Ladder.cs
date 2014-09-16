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
    class Ladder : Entity
    {
        public Ladder(Vector2i pos) : base(pos)
        {
            CurrentTex = "ladderbottom";
            Movable = true;

            DrawOnTop = true;
        }

        public override void Draw(RenderTexture tex, Vector2f scroll, bool shadow)
        {
            base.Draw(tex, scroll, shadow);

            for (int i = 0; i < 3; i++)
            {
                DrawSegment("laddersegment", new Vector2f(Position.X, Position.Y - 16 - i * 16), scroll, tex, shadow);

            }

        }

        public override void Act()
        {
            base.Act();

            for (int i = 0; i < 3; i++)
            {

                Entity ent = EntityFinder.GetEnt(new Vector2f(Position.X, Position.Y - 14 - i * 16));

                if (ent is Player)
                {
                    Player player;
                    player = (Player)ent;

                    player.OnLadder = true;
                }
            }

            Entity ent2 = EntityFinder.GetEnt(new Vector2f(Position.X, Position.Y - 16 - 34));
            {
                if (ent2 is Player)
                {
                    Player player;
                    player = (Player)ent2;

                    player.OnLadder = true;

                    if (player.Position.Y <= Position.Y - 16 - 48)
                    {
                        player.OnTopOfLadder = true;
                    }
                }
            }

        }
    }
}
