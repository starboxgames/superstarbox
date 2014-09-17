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
    class Crane : Entity
    {
        float hookPosX;
        float hookPosY;

        enum State
        {
            Off, Lowering, LiftingUp, MovingRight, Drop
        };

        State state;

        public Crane(Vector2i pos) : base(pos)
        {
            Movable = true;
            CurrentTex = "cranebottom";
            hookPosX = 0;
            hookPosY = 0;

            DrawOnTop = true;

            state = State.Off;
        }

        RectangleShape line = new RectangleShape();

        public override void Reset()
        {
            base.Reset();

            state = State.Off;
            hookPosX = 0;
            hookPosY = 0;
        }

        public override void Draw(RenderTexture tex, Vector2f scroll, bool shadow)
        {
            base.Draw(tex, scroll, shadow);


            if (shadow)
            {


                line.Size = new Vector2f(2, hookPosY);
                line.Position = new Vector2f(hookPosX + 31 + Position.X - 2, Position.Y - 72 - 2);
                line.Position -= scroll;
                line.FillColor = new Color(0, 0, 0, 40);
                line.OutlineColor = new Color(20, 20, 20);

                tex.Draw(line);
            }

            for (int i = 0; i < 5; i++)
            {
                DrawSegment("cranesegment",
                            new Vector2f(Position.X, Position.Y - i * 16 - 16),
                            scroll,
                            tex,
                            shadow);


            }

            for (int i = 0; i < 7; i++)
            {
                DrawSegment("cranesegment",
                            new Vector2f(Position.X + i * 16 + 16, Position.Y - (16 * 5)),
                            scroll,
                            tex,
                            shadow);

            }

            DrawSegment("cranehook",
                        new Vector2f(Position.X + 32 + hookPosX, Position.Y - 64 + hookPosY),
                        scroll,
                        tex,
                        shadow);


            if (!shadow)
            {
                line.Size = new Vector2f(2, hookPosY);
                line.Position = new Vector2f(hookPosX + 31 + Position.X, Position.Y - 72);
                line.Position -= scroll;
                line.FillColor = new Color(20, 20, 20);
                line.OutlineColor = new Color(20, 20, 20);

                tex.Draw(line);
            }

        }

        Entity entToLift;

        int coolOff = 0;
        int warmUp = 0;

        int counter = new Random().Next();

        public override void Act()
        {

            if (entToLift != null && Pushed != 0 && state != State.Off && state != State.Lowering)
            {
                Push(entToLift, PushedDir, Pushed);
            }

            base.Act();

            counter++;

            

            if ((Carried || Hooked) && entToLift != null)
            {
                entToLift.Hooked = false;
                entToLift = null;
                state = State.Off;
            }

            Vector2f soundPos = new Vector2f(Position.X + 32 + hookPosX, Position.Y - 64 + hookPosY);

            if (!Carried && !Hooked)
            {
                if (state == State.Off)
                {
                    if (hookPosX > 0) hookPosX--;
                    if (hookPosY > 0) hookPosY--;

                    if (coolOff > 0) coolOff--;

                    warmUp++;

                    entToLift = EntityFinder.FindEnt(this, EntityFinder.Dir.Down, 112, hookPosX + 32, -48);

                    if (entToLift != null)
                    {
                        if (coolOff == 0 && warmUp > 80)
                        {
                            state = State.Lowering;
                        }
                    }

                }

                if (state == State.Lowering)
                {

                    if (counter % 10 == 0) SoundMan.PlaySound("drop", Position);

                    entToLift = EntityFinder.FindEnt(this, EntityFinder.Dir.Down, 112, hookPosX + 32, -48);
                    if (entToLift == null) state = State.Off;

                    if (hookPosX > 0) hookPosX--;

                    if (entToLift != null && hookPosY + Position.Y - 48 <= entToLift.Position.Y)
                    {
                        hookPosY += 0.25f;
                    }
                    else
                    {
                        if (entToLift != null)
                        {
                            SoundMan.PlaySound("pickup", soundPos);
                            BounceC = 5;
                            entToLift.BounceC = 5;
                        }
                        state = State.LiftingUp;
                    }
                }

                if (state == State.LiftingUp)
                {
                    if (counter % 5 == 0)
                    {
                        SoundMan.PlaySound("drop", soundPos);
                        
                    }

                    warmUp = 0;
                    coolOff = 100;

                    if (hookPosY > 0) hookPosY -= 0.5f;
                    else state = State.MovingRight;

                    if (entToLift != null)
                    {
                        entToLift.Hooked = true;
                    }
                    else state = State.Off;
                }
                
                {
                    if (entToLift != null && entToLift.Hooked)
                    {
                        if (state == State.LiftingUp)
                        {
                            if (entToLift.Carried)
                            {
                                entToLift.Hooked = false;
                                state = State.Off;
                            }



                            if (entToLift.Position.Y > Position.Y - 54 + hookPosY)
                            {
                                if (!Push(entToLift, EntityFinder.Dir.Up, 1f))
                                {
                                    entToLift.Hooked = false;
                                    state = State.Off;
                                }
                            }

                            if (entToLift.Position.Y < Position.Y - 54 + hookPosY)
                            {
                                if (!Push(entToLift, EntityFinder.Dir.Down, 1f))
                                {
                                    entToLift.Hooked = false;
                                    state = State.Off;

                                }
                            }
                        }

                        if (state == State.MovingRight)
                        {
                        if (entToLift.Position.X > Position.X + 32 + hookPosX)
                        {
                            if (!Push(entToLift, EntityFinder.Dir.Left, 1f))
                            {
                                entToLift.Hooked = false;
                                state = State.Off;

                            }
                        }

                        if (entToLift.Position.X < Position.X + 32 + hookPosX)
                        {
                            if (!Push(entToLift, EntityFinder.Dir.Right, 1f))
                            {
                                entToLift.Hooked = false;
                                state = State.Off;

                            }
                        }
                        }

                        if (entToLift.Hooked == false)
                        {
                            entToLift = null;
                            state = State.Off;
                            coolOff = 100;
                        }
                    }

                }

                if (state == State.MovingRight)
                {
                    if (counter % 5 == 0)
                    {
                        SoundMan.PlaySound("drop", soundPos);
                        
                    }

                    if (hookPosX < 64) hookPosX += 0.5f;
                    else
                    {
                        SoundMan.PlaySound("pickup", soundPos);

                        coolOff = 100;
                        entToLift.Hooked = false;
                        state = State.Off;
                        entToLift = null;
                    }
                }

            }
        }
    }
}
