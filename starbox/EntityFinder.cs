using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;

namespace TrainBox
{
    static class EntityFinder
    {
        static List<Entity> entities;

        static public List<Entity> Entities { get { return entities; } set { entities = value; } }

        static public Entity FindOfType(Type t)
        {
            foreach (Entity ent in entities)
            {
                if (ent.GetType() == t && ent.Destroyed == false)
                {
                    return ent;
                }
            }

            return null;
        }

        static public Entity GetEnt(Vector2f pos)
        {
            foreach (Entity ent in entities)
            {

                if (ent.Position.X + 7 > pos.X - 8 && ent.Position.X - 8 < pos.X + 7)
                    if (ent.Position.Y + 7 > pos.Y - 8 && ent.Position.Y - 8 < pos.Y + 7)
                    {
                        if (!ent.Carried && !ent.Destroyed) return ent;

                    }
            }

            return null;
        }

        static public Vector2f GetDir(bool flipped, float x = 1, float y = 1)
        {
            if (!flipped) return new Vector2f(1 * x, 0);
            if (flipped) return new Vector2f(-1 * x, 0);

            return new Vector2f(0, 0);
        }

        static public Vector2f GetDir(Dir dir, float x = 1, float y = 1)
        {
            if (dir == Dir.Down) return new Vector2f(0, 1 * x);
            if (dir == Dir.Up) return new Vector2f(0, -1 * x);
            if (dir == Dir.Left) return new Vector2f(-1 * y, 0);
            if (dir == Dir.Right) return new Vector2f(1 * y, 0);

            return new Vector2f(0, 0);
        }

        public enum Dir
        {
            Left, Right, Up, Down
        }

        static public Entity FindEnt(Entity thisent, Dir dir, int tries = 200, float xo = 0, float yo = 0)
        {
            Entity found = null;

            Vector2f pos = thisent.Position;

            pos.X += xo;
            pos.Y += yo;

            while (found == null)
            {
                found = GetEnt(pos);

                pos += GetDir(dir);

                if (tries > 0) tries--;
                if (tries == 0) break;

                if (found != null)
                {

                    if (found.Carried || found == thisent || found.Destroyed)
                    {
                        found = null;
                        continue;
                    }

                    if (!found.Movable)
                    {
                        found = null;
                        break;
                    }
                }

            }

            return found;
        }
    }
}
