using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.Window;

using System.Xml;
using System.Diagnostics;

using System.IO;
using System.Reflection;

namespace TrainBox
{
    class Level
    {
        List<Entity> entities;

        public Stopwatch Timer = new Stopwatch();

        static List<Type> entityTypes = new List<Type>();

        public static List<Type> EntityTypes { get { return entityTypes; } }

        Sprite background;
        Texture backgroundTexture;

        Vector2f scroll = new Vector2f(0, 0);

        public Vector2f Scroll { get { return scroll; } set { scroll = value; } }

        int maxScrollX, minScrollX;
        int maxScrollY, minScrollY;

        public static void PrepareEntityTypes()
        {
            entityTypes.Add(typeof(Block));
            entityTypes.Add(typeof(Box));
            entityTypes.Add(typeof(Conveyor));
            entityTypes.Add(typeof(Crane));
            entityTypes.Add(typeof(Key));
            entityTypes.Add(typeof(KeyLock));
            entityTypes.Add(typeof(KeyBlock));
            entityTypes.Add(typeof(Ladder));
            entityTypes.Add(typeof(Player));
            entityTypes.Add(typeof(Wagon));
            entityTypes.Add(typeof(Dynamite));
            entityTypes.Add(typeof(Detonator));
            entityTypes.Add(typeof(InfoBox));
            entityTypes.Add(typeof(Star));
            entityTypes.Add(typeof(StarBlock));
            entityTypes.Add(typeof(Exit));
        }

        public List<Entity> GetEntities()
        {
            return entities;
        }


        public bool SaveLevel(string name)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = Environment.NewLine;

            XmlWriter writer = XmlWriter.Create("lvl/" + name + ".xml", settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Level");

            foreach (Entity ent in entities)
            {
                writer.WriteStartElement("Entity");

                writer.WriteAttributeString("Name", ent.GetType().Name);
                writer.WriteAttributeString("X", ent.OrigPos.X.ToString());
                writer.WriteAttributeString("Y", ent.OrigPos.Y.ToString());
                writer.WriteAttributeString("Flipped", ent.Flipped.ToString());

                if (ent is InfoBox)
                {
                    InfoBox infoBox = (InfoBox)ent;

                    writer.WriteAttributeString("InfoString", infoBox.InfoString);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Dispose();

            return true;
        }

        public void DrawLevel(RenderTexture buffer)
        {
            DrawBackground(buffer);

            foreach (Entity ent in entities)
            {
                if (!ent.Destroyed) ent.Draw(buffer, scroll, true);
            }

            foreach (Entity ent in entities)
            {
                if (!ent.Destroyed && !ent.DrawOnTop) ent.Draw(buffer, scroll, false);
            }

            foreach (Entity ent in entities)
            {
                if (!ent.Destroyed && ent.DrawOnTop) ent.Draw(buffer, scroll, false);
            }
        }

        void DrawBackground(RenderTexture buffer)
        {
            background.Position = new Vector2f(-scroll.X % 320, -scroll.Y % 120);
            buffer.Draw(background);

            background.Position = new Vector2f(background.Position.X + 320, background.Position.Y);
            buffer.Draw(background);

            background.Position = new Vector2f(background.Position.X - 640, background.Position.Y);
            buffer.Draw(background);

            background.Position = new Vector2f(-scroll.X % 320, -scroll.Y % 120 + 120);
            buffer.Draw(background);

            background.Position = new Vector2f(background.Position.X + 320, background.Position.Y);
            buffer.Draw(background);

            background.Position = new Vector2f(background.Position.X - 640, background.Position.Y);
            buffer.Draw(background);

            background.Position = new Vector2f(-scroll.X % 320, -scroll.Y % 120 - 120);
            buffer.Draw(background);

            background.Position = new Vector2f(background.Position.X + 320, background.Position.Y);
            buffer.Draw(background);

            background.Position = new Vector2f(background.Position.X - 640, background.Position.Y);
            buffer.Draw(background);

        }

        public void ResetLevel()
        {
            foreach(Entity ent in entities)
            {
                ent.Reset();
            }
        }

        public void ResetScrollBounds()
        {
            maxScrollX = 9999999;
            minScrollX = -999999;
            maxScrollY = 9999999;
            minScrollY = -999999;
        }

        public void FindScrollBounds()
        {
            maxScrollX = -9999999;
            minScrollX = 999999;
            maxScrollY = -9999999;
            minScrollY = 999999;

            foreach(Entity ent in entities)
            {
                if (maxScrollX < ent.Position.X) maxScrollX = (int)ent.Position.X + 8;
                if (minScrollX > ent.Position.X) minScrollX = (int)ent.Position.X - 8;
                if (maxScrollY < ent.Position.Y) maxScrollY = (int)ent.Position.Y + 8;
                if (minScrollY > ent.Position.Y) minScrollY = (int)ent.Position.Y - 8;
            }
        }

        public bool LoadLevel(string name, bool fromDisk = false)
        {
            Assembly assembly;
            assembly = Assembly.GetExecutingAssembly();

            Stream bgStream = assembly.GetManifestResourceStream("TrainBox.gfx.bg.png");
            

            backgroundTexture = new Texture(bgStream);
            background = new Sprite();
            background.Texture = backgroundTexture;

            entities = null;

            entities = new List<Entity>();

            XmlReader reader = null;
            Stream levelStream = null;

            if (fromDisk) reader = XmlReader.Create("lvl/" + name + ".xml");
            else
            {
                levelStream = assembly.GetManifestResourceStream("TrainBox.lvl." + name + ".xml");
                reader = XmlReader.Create(levelStream);
            }

            Type entType = null;
            Vector2i entPos = new Vector2i(0, 0);
            Boolean entFlipped = false;

            reader.MoveToContent();

            while (reader.Read())
            {
                if (reader.Name == "Entity")
                {
                    string s = reader.GetAttribute("Name");

                    foreach (Type t in entityTypes)
                        if (s == t.Name)
                        {
                            entType = t;
                            break;
                        }

                    entPos.X = Convert.ToInt32(reader.GetAttribute("X"));
                    entPos.Y = Convert.ToInt32(reader.GetAttribute("Y"));
                    entFlipped = Convert.ToBoolean(reader.GetAttribute("Flipped"));

                    Entity a = (Entity)Activator.CreateInstance(entType, entPos);
                    a.Flipped = entFlipped;

                    if (entType == typeof(InfoBox))
                    {
                        InfoBox infoBox = (InfoBox)a;

                        infoBox.InfoString = reader.GetAttribute("InfoString");
                    }

                    entities.Add(a);
                }
            }

            reader.Dispose();

            FindScrollBounds();

            return true;
        }

        public void ScrollTo(Vector2f position, bool snap)
        {
            if (!snap)
            {

                if (scroll.X + 160 < position.X)
                {
                    scroll.X++;
                }
                if (scroll.X + 160 > position.X)
                {
                    scroll.X--;
                }

                if (scroll.Y + 60 < position.Y)
                {
                    scroll.Y++;
                }
                if (scroll.Y + 60 > position.Y)
                {
                    scroll.Y--;
                }

            }
            else
            {
                scroll.X = position.X - 160;
                scroll.Y = position.Y - 60;
            }

            if (scroll.X < 0) scroll.X = 0;
            if (scroll.Y > 0) scroll.Y = 0;

            if (scroll.X > maxScrollX - 320) scroll.X = maxScrollX - 320;
            if (scroll.X < minScrollX) scroll.X = minScrollX;

            if (scroll.Y > maxScrollY - 120) scroll.Y = maxScrollY - 120;
            if (scroll.Y < minScrollY) scroll.Y = minScrollY;

        }
    }
}
