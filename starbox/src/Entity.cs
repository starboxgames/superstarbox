namespace TrainBox
{
    using System;
    using System.Collections.Generic;

    using SFML;
    using SFML.Graphics;
    using SFML.Window;


    class Entity
    {

        protected Sprite Spr { get; set; }

        public Vector2i OrigPos { get; set; }

        public Entity(Vector2i pos)
        {
            OrigPos = pos;

            Position = new Vector2f(pos.X * 16 + 8, pos.Y * 16);
            Speed = new Vector2f(0, 0);

            Spr = new Sprite();

            Destroyed = false;
            Movable = false;
            Hooked = false;
            Flipped = false;
            Carried = false;
            Walking = false;
            DrawOnTop = false;
            ScreenShake = false;
        }

        protected string CurrentTex { get; set; }

        public Vector2f Position { get; set; }
        public Vector2f Speed { get; set; }
        public float Pushed { get; set; }
        public EntityFinder.Dir PushedDir { get; set; }

        public Entity GroundEnt { get; set; }
        public Entity CeilingEnt { get; set; }
        public Entity BlockedLeftEnt { get; set; }
        public Entity BlockedRightEnt { get; set; }

        public bool Flipped { get; set; }
        public bool Movable { get; set; }
        public bool Walking { get; set; }        
        public bool Hooked { get; set; }
        public bool Destroyed { get; set; }
        public bool Carried { get; set; }
        public bool DrawOnTop { get; set; }
        public bool ScreenShake { get; set; }

        public virtual void Reset()
        {
            BounceC = 0;

            Position = new Vector2f(OrigPos.X * 16 + 8, OrigPos.Y * 16);
            Destroyed = false;
            Carried = false;
            Hooked = false;
            Speed = new Vector2f(0, 0);
        }

        protected void DrawSegment(string name, Vector2f pos, Vector2f scroll, RenderTexture tex, bool shadow)
        {
            Spr = TextureMan.GetSprite(name, shadow);
       
            Spr.Position = pos;
            if (shadow) Spr.Position = new Vector2f(pos.X - 2, pos.Y - 2);
            Spr.Position -= scroll;

            Spr.Origin = new Vector2f(8, 8);
            Spr.Scale = new Vector2f(1 * scaleX, 1 * scaleY);

            tex.Draw(Spr);
        }

        int bounceC = 0;

        float scaleX = 1;
        float scaleY = 1;

        public int BounceC
        {
            get { return bounceC; }
            set { bounceC = value; }
        }

        public virtual void Act()
        {
            Pushed = 0;

            if (bounceC > 0)
            {
                bounceC--;

                scaleX = 1 + (float)bounceC / 30f;
                scaleY = 1 - (float)bounceC / 50f;
            }

            if (bounceC == 0)
            {
                scaleX = 1;
                scaleY = 1;
            }

            if (BlockedRightEnt != null && Movable)
            {
 
                Position = new Vector2f((int)Position.X, Position.Y);

                if (Speed.X > 0)
                    if (BlockedRightEnt.Hooked || !Push(BlockedRightEnt, EntityFinder.Dir.Right, 1))
                    {
                        
                        Speed = new Vector2f(0, Speed.Y);
                    }

            }

            if (BlockedLeftEnt != null && Movable)
            {

                Position = new Vector2f((int)(Position.X + 0.9f), Position.Y);

                if (Speed.X < 0)
                    if (BlockedLeftEnt.Hooked || !Push(BlockedLeftEnt, EntityFinder.Dir.Left, 1))
                    {
                        
                        Speed = new Vector2f(0, Speed.Y);                        
                    }
            }

            if (GroundEnt == null && Movable)
            {
                Speed += new Vector2f(0, 0.1f);

                if (!Walking)
                {
                    Speed = new Vector2f(Speed.X * 0.9f, Speed.Y);
                    if (Math.Abs(Speed.X) < 0.1)
                    {
                        Speed = new Vector2f(0, Speed.Y);
                    }
                }

            }

            if (GroundEnt != null && Movable && !GroundEnt.Carried)
            {
                if (Speed.Y > 0)
                {

                    if (GroundEnt.Hooked == false)
                    {
                        SoundMan.PlaySound("drop", Position);

                        bounceC = 10;
                        if (!GroundEnt.Movable) GroundEnt.BounceC = 3;
                        else
                        {
                            GroundEnt.BounceC = 7;

                            if (GroundEnt.GroundEnt != null && GroundEnt.GroundEnt.Movable)
                            {
                                GroundEnt.GroundEnt.BounceC = 3;
                            }
                        }
                    }
                    if (!Hooked) Position = new Vector2f(Position.X, GroundEnt.Position.Y - 16);
                }

                

                if (!Walking)
                {
                    if (!Carried && !Hooked) Position = new Vector2f(Position.X, GroundEnt.Position.Y - 16);  

                    Speed = new Vector2f(Speed.X * 0.5f, Speed.Y);
                    if (Math.Abs(Speed.X) < 0.1)
                    {
                        Speed = new Vector2f(0, Speed.Y);
                    }
                }

                Speed = new Vector2f(Speed.X, 0);
            }

            if (CeilingEnt != null && Movable)
            {
                //Position = new Vector2f(Position.X, CeilingEnt.Position.Y+15);

                if (Speed.Y < 0)
                {
                    Speed = new Vector2f(Speed.X, 0);
                    if (!CeilingEnt.Movable) CeilingEnt.BounceC = 3;
                    else CeilingEnt.BounceC = 7;
                    bounceC = 10;
                }
            }

            if (Speed.Y > 1)
            {
                Speed = new Vector2f(Speed.X, 1f);
            }

            
            if (!Hooked)
            {
                Position += Speed;
            }
            else Speed = new Vector2f(Speed.X, 0);
        }

        public virtual void Draw(RenderTexture tex, Vector2f scroll, bool shadow)
        {
            Spr = TextureMan.GetSprite(CurrentTex, shadow);

            Spr.Position = Position - scroll;

            if (shadow)
            {
                Spr.Position = new Vector2f(Spr.Position.X - 2, Spr.Position.Y - 2);
            }

            Spr.Origin = new Vector2f(8, 8 - bounceC / 3);

            if (Flipped)
            {
                Spr.Scale = new Vector2f(-1 * scaleX, 1 * scaleY);
            }
            else
            {
                Spr.Scale = new Vector2f(1 * scaleX, 1 * scaleY);
            }

            tex.Draw(Spr);
        }

        protected bool Push(Entity ent, EntityFinder.Dir dir, float speed = 1)
        {
            if (ent.Movable == false)
            {
                return false;
            }
            else
            {
                if (dir == EntityFinder.Dir.Right && ent.BlockedRightEnt == null)
                {
                    ent.Position = new Vector2f(ent.Position.X + speed, ent.Position.Y);
                    ent.Pushed = speed;
                    ent.PushedDir = dir;

                    if (ent.CeilingEnt != null) Push(ent.CeilingEnt, dir, speed);
                    return true;
                }

                if (dir == EntityFinder.Dir.Left && ent.BlockedLeftEnt == null)
                {
                    ent.Position = new Vector2f(ent.Position.X - speed, ent.Position.Y);
                    ent.Pushed = speed;
                    ent.PushedDir = dir;

                    if (ent.CeilingEnt != null) Push(ent.CeilingEnt, dir, speed);
                    return true;
                }
                if (dir == EntityFinder.Dir.Up)
                {
                    bool doPush = true;

                    if (ent.CeilingEnt != null)
                        if (!ent.CeilingEnt.Movable) doPush = false;

                    if (doPush)
                    {
                        ent.Speed = new Vector2f(ent.Speed.X, 0);
                        ent.Position = new Vector2f(ent.Position.X, ent.Position.Y - speed);
                        ent.Pushed = speed;
                        ent.PushedDir = dir;

                        if (ent.CeilingEnt != null) if (!Push(ent.CeilingEnt, dir, speed)) return false;
                        return true;
                    }
                    else return false;
                    
                }
                if (dir == EntityFinder.Dir.Down)
                {
                    bool doPush = true;

                    if (ent.GroundEnt != null)
                        if (!ent.GroundEnt.Movable) doPush = false;

                    if (doPush)
                    {
                        ent.Speed = new Vector2f(ent.Speed.X, 0);
                        ent.Position = new Vector2f(ent.Position.X, ent.Position.Y + speed);
                        ent.Pushed = speed;
                        ent.PushedDir = dir;

                        if (ent.GroundEnt != null) if (!Push(ent.GroundEnt, dir, speed)) return false;
                        return true;
                    }
                    else return false;
                }
       
            }

            return false;
        }

    }
}