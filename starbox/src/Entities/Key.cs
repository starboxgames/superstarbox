using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace TrainBox
{
    class Key : Entity
    {
        public Key(Vector2i pos) : base(pos)
        {
            CurrentTex = "key";
            Movable = true;
        }

        public override void Act()
        {
            base.Act();

            if (   BlockedLeftEnt is KeyLock 
                || BlockedRightEnt is KeyLock
                || GroundEnt is KeyLock
                || CeilingEnt is KeyLock)
            {
                SoundMan.PlaySound("open", Position);

                if (BlockedLeftEnt is KeyLock) BlockedLeftEnt.Destroyed = true;
                if (BlockedRightEnt is KeyLock) BlockedRightEnt.Destroyed = true;
                if (GroundEnt is KeyLock) GroundEnt.Destroyed = true;
                if (CeilingEnt is KeyLock) CeilingEnt.Destroyed = true;
                

                while (true)
                {
                    Entity a = EntityFinder.FindOfType(typeof(KeyBlock));

                    if (a != null) a.Destroyed = true;
                    else break;
                }

                Destroyed = true;

            }
        }
    }
}
