namespace TrainBox
{
    using System;
    using System.Collections.Generic;

    using SFML;
    using SFML.Graphics;
    using SFML.Window;

    class Player : Entity
    {
        Entity pickUp;

        public Player(Vector2i _pos)
        : base(_pos)
        {
            CurrentTex = "player";
            Movable = true;

            pickUp = null;
        }

        public bool DoJump { get; set; }
        public bool Climbing { get; set; }
        public bool OnLadder { get; set; }
        public bool OnTopOfLadder { get; set; }

        public override void Reset()
        {
            base.Reset();

            if (pickUp != null) pickUp.Carried = false;
            pickUp = null;
            Flipped = false;
            OnLadder = false;
            Climbing = false;
        }

        public void ClimbUp(List<Entity> entities)
        {


            //Console.WriteLine(FindEnt(entities, Dir.Down, 64).ToString());


            if (OnLadder && CeilingEnt == null)
            {
                Climbing = true;
                if (!OnTopOfLadder) Position = new Vector2f(Position.X, Position.Y - 0.5f);
            }
        }

        public void ClimbDown(List<Entity> entities)
        {
            if (OnLadder && GroundEnt == null)
            {
                Climbing = true;
                Position = new Vector2f(Position.X, Position.Y + 0.5f);
            }

        }

        int counter = 0;

        public override void Act()
        {
            counter++;

            CurrentTex = "player";

            if (Walking && GroundEnt != null && (counter % 20 < 5)) CurrentTex = "playerwalk1";
            if (Walking && GroundEnt != null && (counter % 20 >= 5 && counter % 20 < 10)) CurrentTex = "player";
            if (Walking && GroundEnt != null && (counter % 20 >= 10 && counter % 20 < 15)) CurrentTex = "playerwalk2";
            if (Walking && GroundEnt != null && (counter % 20 >= 15 && counter % 20 < 20)) CurrentTex = "player";


            if (Climbing) Speed = new Vector2f(Speed.X, 0);

            base.Act();

            if (Climbing && !DoJump)
            {
                Position = new Vector2f(Position.X, Position.Y - Speed.Y);
                if (!OnLadder) Climbing = false;
            }

            OnLadder = false;
            OnTopOfLadder = false;

            if (GroundEnt is Ladder)
            {
                Position = new Vector2f(Position.X, Position.Y);
            }

            if (GroundEnt != null || Climbing)
            {
                if (DoJump)
                {
                    BounceC = 10;

                    SoundMan.PlaySound("jump", Position);

                    Position = new Vector2f(Position.X, Position.Y - 0.5f);
                    Speed = new Vector2f(Speed.X, -2f);
                    DoJump = false;
                    Climbing = false; 
                }
            }


            if (pickUp != null)
            {
                

                pickUp.Speed = new Vector2f(0, 0);
                if (!Flipped)
                {
                    pickUp.Position = new Vector2f(Position.X + 6, Position.Y - 6);
                }
                else
                {
                    pickUp.Position = new Vector2f(Position.X - 6, Position.Y - 6);
                }

                if (pickUp.Destroyed) pickUp = null;
            }
        }

        public void PickUp()
        {
            if (pickUp == null)
            {
                

                pickUp = EntityFinder.GetEnt(Position + EntityFinder.GetDir(Flipped, 16, 0));
                
                if (pickUp != null && !pickUp.Movable) pickUp = null;

                if (pickUp != null)
                {
                    pickUp.BounceC = 5;

                    SoundMan.PlaySound("pickup", Position);
                    pickUp.Carried = true;
                }
            }
            else
            {
                if (EntityFinder.GetEnt(Position + EntityFinder.GetDir(Flipped, 16, 0)) == null)
                {
                    pickUp.Position = Position + EntityFinder.GetDir(Flipped, 16, 0);
                    pickUp.Carried = false;
                    pickUp = null;
                }
            }
        }

        public void StopWalk()
        {
            Walking = false;
        }

        public void WalkLeft()
        {
            Flipped = true;
            if (Speed.X > -1)
            {
                Speed = new Vector2f(Speed.X - 0.1f, Speed.Y);
            }

            Walking = true;
        }

        public void WalkRight()
        {
            Flipped = false;
            if (Speed.X < 1)
            {
                Speed = new Vector2f(Speed.X + 0.1f, Speed.Y);
            }

            Walking = true;
        }
    }
}