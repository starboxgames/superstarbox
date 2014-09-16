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

    class InfoBox : Entity
    {
        public InfoBox(Vector2i _pos)
            : base(_pos)
        {
            CurrentTex = "infobox";
            Movable = false;

            InfoString = "info";

            DrawOnTop = true;
        }

        public string InfoString { get; set; }

        Text text = new Text();


        public override void Reset()
        {
            base.Reset();

            active = false;
        }

        public override void Draw(RenderTexture tex, Vector2f scroll, bool shadow)
        {
            base.Draw(tex, scroll, shadow);

            if (active)
            {
                text.Font = TextureMan.Font;
                text.CharacterSize = 8;
                text.Position = new Vector2f(320 / 2.0f, 20);

                FloatRect textRect = text.GetLocalBounds();
                text.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f,
                                            textRect.Top + textRect.Height / 2.0f);
                
                text.DisplayedString = InfoString;

                if (shadow)
                {
                    text.Color = new Color(20, 20, 20, 40);
                    text.Position = new Vector2f(text.Position.X - 1, text.Position.Y - 1);
                }
                else
                {
                    int d;

                    if (c * 10 > 255) d = 255;
                    else d = c * 10;

                    text.Color = new Color(255, 255, 255, (byte)(d));
                }

                tex.Draw(text);
            }
        }

        bool active = false;

        int c = 0;

        public override void Act()
        {
            base.Act();

           // active = false;

            c++;

            if (CeilingEnt != null && CeilingEnt is Player)
            {

                if (!active)
                {
                    CeilingEnt.BounceC = 3;
                    BounceC = 15;
                    c = 0;
                }
                active = true;
            }
            else active = false;
        }
    }

}