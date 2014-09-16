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
    class Editor
    {
        public bool Active { get; set; }
        public Level Level { get; set; }
        Vector2i CursorPos { get; set; }
        Vector2f CursorPosF { get; set; }

        public string LevelName { get; set; }

        public Editor()
        {
            Active = false;
            CursorPos = new Vector2i(0, 0);
        }

        int buttonDelay = 0;
        int counter = 0;

        int currentEnt = 0;

        Entity cursorEnt = null;

        bool showTextWindow = false;

        public void Loop(RenderWindow window)
        {
            counter++;

            CursorPosF = new Vector2f(CursorPos.X * 16, CursorPos.Y * 16 + 8);

            Level.ScrollTo(CursorPosF, false);
            Level.GetEntities().RemoveAll(a => a.Destroyed);

            Type entType = Level.EntityTypes[currentEnt];
            cursorEnt = (Entity)Activator.CreateInstance(entType, CursorPos);

            if (buttonDelay == 0)
            {

                if (Keyboard.IsKeyPressed(Keyboard.Key.F3) || Keyboard.IsKeyPressed(Keyboard.Key.F5) 
                        || Keyboard.IsKeyPressed(Keyboard.Key.F1)
                    )
                {
                    showTextWindow = !showTextWindow;

                    if (Keyboard.IsKeyPressed(Keyboard.Key.F3)) textInputOperation = TextInputOperation.Save;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.F5)) textInputOperation = TextInputOperation.Load;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.F1)) textInputOperation = TextInputOperation.InfoBox;

                    if (textInputOperation == TextInputOperation.Save || textInputOperation == TextInputOperation.Load) 
                        textInputString = LevelName;

                    bool addEvent = true;

                    if (textInputOperation == TextInputOperation.InfoBox) 
                    {
                        Entity ent = EntityFinder.GetEnt(new Vector2f(CursorPosF.X + 8, CursorPosF.Y - 8));

                        if (ent != null && ent is InfoBox)
                        {
                            InfoBox infoBox = (InfoBox)ent;
                            textInputString = infoBox.InfoString;
                        }
                        else
                        {
                            showTextWindow = false;
                            addEvent = false;
                        }
                    }

                    if (addEvent)
                    {
                        if (showTextWindow)
                        {
                            window.TextEntered += TextEntered;
                        }
                        else window.TextEntered -= TextEntered;
                    }

                    buttonDelay = 20;
                }

                if (showTextWindow)
                {
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                    {
                        if (textInputOperation == TextInputOperation.Save)
                        {
                            LevelName = textInputString;
                            Level.SaveLevel(LevelName);
                        }
                        if (textInputOperation == TextInputOperation.Load)
                        {
                            LevelName = textInputString;

                            Level.LoadLevel(LevelName, true);

                            CollisionMan.Entities = Level.GetEntities();
                            EntityFinder.Entities = Level.GetEntities();
                        }
                        if (textInputOperation == TextInputOperation.InfoBox)
                        {
                            Entity ent = EntityFinder.GetEnt(new Vector2f(CursorPosF.X + 8, CursorPosF.Y - 8));

                            if (ent != null && ent is InfoBox)
                            {
                                InfoBox infoBox = (InfoBox)ent;
                                infoBox.InfoString = textInputString;
                            }
                        }

                        showTextWindow = false;
                        window.TextEntered -= TextEntered;
                    }

                    if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                    {
                        showTextWindow = false;
                        window.TextEntered -= TextEntered;
                    }
                    
                }
            }

            if (buttonDelay == 0 && !showTextWindow)
            {


                if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
                {
                    if (currentEnt > 0) currentEnt--;
                    buttonDelay = 10; 
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.X))
                {
                    if (currentEnt < Level.EntityTypes.Count -1) currentEnt++;
                    buttonDelay = 10;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.F12))
                {
                    Level.GetEntities().Clear();

                    buttonDelay = 10;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Delete))
                {
                    Entity ent = EntityFinder.GetEnt(new Vector2f(CursorPosF.X+8,CursorPosF.Y-8));

                    if (ent != null) ent.Destroyed = true;

                    buttonDelay = 10;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.F))
                {
                    Entity ent = EntityFinder.GetEnt(new Vector2f(CursorPosF.X + 8, CursorPosF.Y - 8));

                    if (ent != null)
                    {
                        ent.Flipped = !ent.Flipped;
                    }

                    buttonDelay = 10;
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
                {
                    Entity ent = EntityFinder.GetEnt(new Vector2f(CursorPosF.X + 8, CursorPosF.Y - 8));

                    if (ent != null)
                    {
                        ent.Destroyed = true;
                    }

                    Entity newEnt = (Entity)Activator.CreateInstance(entType, CursorPos);
                    Level.GetEntities().Add(newEnt);


                    buttonDelay = 10;
                    
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    CursorPos = new Vector2i(CursorPos.X + 1, CursorPos.Y);
                    buttonDelay = 10;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                {
                    CursorPos = new Vector2i(CursorPos.X - 1, CursorPos.Y);
                    buttonDelay = 10;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
                {
                    CursorPos = new Vector2i(CursorPos.X, CursorPos.Y - 1);
                    buttonDelay = 10;
                }
                if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
                {
                    CursorPos = new Vector2i(CursorPos.X, CursorPos.Y + 1);
                    buttonDelay = 10;
                }
            }
            if (buttonDelay > 0) buttonDelay--;
        }

        RectangleShape cursorShape = new RectangleShape();
        RectangleShape windowShape = new RectangleShape();
        Text text = new Text();

        string textInputString;

        void TextEntered(object sender, TextEventArgs e)
        {

            if (e.Unicode != "\u0008" && e.Unicode != "\u001b" && e.Unicode != "\u000D") textInputString += e.Unicode;

            if (e.Unicode == "\u0008")
            {
                if (textInputString.Length > 0) textInputString = textInputString.Remove(textInputString.Length - 1);
            }

        }

        enum TextInputOperation { Save, Load, InfoBox };
        TextInputOperation textInputOperation;

        public void Draw(RenderTexture buffer)
        {
            cursorShape.Position = CursorPosF - Level.Scroll;
            cursorShape.Position = new Vector2f(cursorShape.Position.X, cursorShape.Position.Y - 16);

            cursorShape.Size = new Vector2f(16, 16);
            cursorShape.Origin = new Vector2f(0, 0);
            cursorShape.FillColor = new Color(0, 0, 0, 0);
            cursorShape.OutlineColor = new Color(255, 0, 0,128);
            cursorShape.OutlineThickness = 1;

            if (counter % 10 < 5 && !showTextWindow)
            {
                
                cursorEnt.Draw(buffer, Level.Scroll, false);
                buffer.Draw(cursorShape);
            }

            if (showTextWindow)
            {
                windowShape.Position = new Vector2f(120, 45);
                windowShape.Size = new Vector2f(80, 30);
                windowShape.FillColor = windowShape.OutlineColor = new Color(128,64,64);
                windowShape.OutlineThickness = 1;

                if (counter % 10 < 5)
                {
                    text.DisplayedString = textInputString + "|";
                }
                else text.DisplayedString = textInputString;                
                
                
                text.Font = TextureMan.Font;
                text.CharacterSize = 8;

                text.Position = new Vector2f(123, 57);
                if (textInputOperation == TextInputOperation.InfoBox)
                    text.Position = new Vector2f(0, 57);
                buffer.Draw(windowShape);
                buffer.Draw(text);

                if (textInputOperation == TextInputOperation.Save)
                {
                    text.DisplayedString = "SAVE TO:";
                }
                if (textInputOperation == TextInputOperation.Load)
                {
                    text.DisplayedString = "LOAD FROM:";
                }
                if (textInputOperation == TextInputOperation.InfoBox)
                {
                    text.DisplayedString = "INFO BOX TEXT:";
                }

                text.Position = new Vector2f(123, 47);

                buffer.Draw(text);
            }
        }
    }
}
