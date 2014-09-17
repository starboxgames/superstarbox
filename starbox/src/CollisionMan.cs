using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Window;

using System.Diagnostics;

namespace TrainBox
{
    static class CollisionMan
    {
        static List<Entity> entities;

        static List<Entity>[,] collisionCell;

        static public List<Entity> Entities { get { return entities; } set { entities = value; } }

        const int cellNum = 60;
        const int cellSize = 32;

        static CollisionMan()
        {

            collisionCell = new List<Entity>[cellNum,cellNum];

            for (int i = 0; i < cellNum; i++)
                for (int j = 0; j < cellNum; j++ )
                {
                    collisionCell[i,j] = new List<Entity>();
                }
        }


        static public void PopulateCells()
        {
            for (int i = 0; i < cellNum; i++)
                for (int j = 0; j < cellNum; j++ )
                {
                    collisionCell[i, j].Clear();
                }

            foreach (Entity ent in entities)
            {                

                int x = (int)(ent.Position.X / cellSize) + 1;
                int y = (int)(ent.Position.Y / cellSize) + (cellNum - 2) + 1;

                if (x < 1) x = 1;
                if (y < 1) y = 1;

                if (x >= cellNum - 2) x = cellNum - 2;
                if (y >= cellNum - 2) y = cellNum - 2;

                collisionCell[x, y].Add(ent);

                if (ent.Position.X % cellSize <= 16) collisionCell[x - 1, y].Add(ent);
                if (ent.Position.X % cellSize >= cellSize - 16) collisionCell[x + 1, y].Add(ent);

                if (ent.Position.Y % cellSize <= 16) collisionCell[x, y - 1].Add(ent);
                if (ent.Position.Y % cellSize >= cellSize - 16) collisionCell[x, y + 1].Add(ent);
            }

        }

        static public void CheckCollision(Entity thisent, Entity ent)
        {
            if (thisent.Carried) thisent.Hooked = false;

            if (ent != thisent)
            {
                if (ent.Carried == false && ent.Destroyed == false)
                {

                    if (StandingOn(thisent, ent))
                    {
                        thisent.GroundEnt = ent;
                    }

                    if (BlockedRight(thisent, ent))
                    {
                        thisent.BlockedRightEnt = ent;

                    }
                    if (BlockedLeft(thisent, ent))
                    {
                        thisent.BlockedLeftEnt = ent;

                    }

                    if (Ceiling(thisent, ent))
                    {
                        thisent.CeilingEnt = ent;
                    }
                }
            }
        }

        static public void CheckCollisionCells()
        {
            foreach (Entity ent in entities)
            {
                Entity thisent = ent;
                thisent.GroundEnt = null;
                thisent.BlockedRightEnt = null;
                thisent.BlockedLeftEnt = null;
                thisent.CeilingEnt = null;
            }

            for (int i = 0; i < cellNum; i++)
                for (int j = 0; j < cellNum; j++)
                    if (collisionCell[i, j].Count > 0)
                    {
                        foreach (Entity thisent in collisionCell[i, j])
                            foreach (Entity otherent in collisionCell[i, j])
                            {

                                if (!(thisent is Block))
                                {
                                    CheckCollision(thisent, otherent);
                                    
                                }

                                
                            }
                    }

        }
        
        static public bool BlockedLeft(Entity thisent, Entity ent)
        {
            if (thisent.Position.X - 16 <= ent.Position.X && thisent.Position.X - 16 >= ent.Position.X - 8)
                if (thisent.Position.Y + 7 >= ent.Position.Y - 8 && thisent.Position.Y + 7 <= ent.Position.Y + 22)
                {
                    return true;
                }

            return false;
        }

        static public bool BlockedRight(Entity thisent, Entity ent)
        {
            if (thisent.Position.X + 16 >= ent.Position.X && thisent.Position.X + 16 <= ent.Position.X + 8)
                if (thisent.Position.Y + 7 >= ent.Position.Y - 8 && thisent.Position.Y + 7 <= ent.Position.Y + 22)
                {
                    return true;
                }

            return false;
        }

        static public bool Ceiling(Entity thisent, Entity ent)
        {
            if (thisent.Position.Y - 16 <= ent.Position.Y && thisent.Position.Y - 16 >= ent.Position.Y - 8)
                if (thisent.Position.X + 7 >= ent.Position.X - 8 && thisent.Position.X - 8 <= ent.Position.X + 7)
                {
                    return true;
                }
            return false;
        }
        
        static public bool StandingOn(Entity thisent, Entity ent)
        {
            if (thisent.Position.Y + 16 >= ent.Position.Y && thisent.Position.Y + 16 <= ent.Position.Y + 8)
                if (thisent.Position.X + 7 >= ent.Position.X - 8 && thisent.Position.X - 8 <= ent.Position.X + 7)
                {
                    return true;
                }
            return false;
        } 


    }
}
