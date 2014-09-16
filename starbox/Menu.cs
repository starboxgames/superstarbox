using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SFML;
using SFML.Graphics;
using SFML.Window;

using System.IO;
using System.Reflection;
using System.Xml;
using System.Security.Cryptography;

namespace TrainBox
{
    class Menu
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        public enum MenuState { Intro, MainMenu, PlayMenu, StartLevel, Quit, LevelWon, GameWon };
        public MenuState State { get; set; }

        public string LevelName { get; set; }

        public bool Active { get; set; }

        public bool Cheater { get; set; }



        public TimeSpan LevelCompletiontime { get; set; }

        int menuOption = 0;

        Sprite spr = new Sprite();
        Text txt = new Text();

        Dictionary<string, string> levels = new Dictionary<string, string>()
        {
            {"Tutorial", "tutorial"},
            {"1. Boomkart", "1"},
            {"2. Out of reach", "2"},
            {"3. Blasting contraption", "3"},
            {"4. Super stacker", "4"},
            {"5. Crane lift crane", "5"},
            {"6. Just in time", "6"},
            {"7. A ride for two", "7"}
        };

        Dictionary<string, bool> levelCompleted = new Dictionary<string,bool>();
        Dictionary<string, TimeSpan> levelBestTime = new Dictionary<string,TimeSpan>();

        Texture superTexture;
        Texture starboxTexture;
        Texture crateTexture;
        Texture levelSelectBgTexture;
        Texture congratulationsTexture;
        Texture gameWonTexture;
        Texture gameWonTextTexture;

        public Menu()
        {
            superTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.super.png"));
            starboxTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.starbox.png"));
            crateTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.crate.png"));
            levelSelectBgTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.levelselectbg.png"));
            congratulationsTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.congratulations.png"));
            gameWonTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.gamewonbg.png"));
            gameWonTextTexture = new Texture(assembly.GetManifestResourceStream("TrainBox.gfx.menu.gamewontext.png"));

            Cheater = false;
        }


        public bool LoadLevelInfo()
        {

            foreach (KeyValuePair<string, string> level in levels)
            {
                levelCompleted.Add(level.Value, false);
                levelBestTime.Add(level.Value, new TimeSpan(0, 24, 59, 59, 999));
            }
            
            if (File.Exists("config.dat") && File.Exists("levelinfo.xml"))
            {
                Stream md5file = File.OpenRead("config.dat");

                FileInfo f = new FileInfo("config.dat");
                byte [] buffer = new byte[f.Length];
                md5file.Read(buffer, 0, (int)f.Length);
                md5file.Close();


                MD5 md5 = MD5.Create();
                Stream file = File.OpenRead("levelinfo.xml");
                byte[] hash = md5.ComputeHash(file);
                file.Close();

                if (buffer.SequenceEqual(hash)) Cheater = false;
                else Cheater = true;
           

            }

            if (File.Exists("config.dat") && File.Exists("levelinfo.xml") && !Cheater)
            {
                XmlReader reader = XmlReader.Create("levelinfo.xml");

                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.Name == "Level")
                    {
                        string name = reader.GetAttribute("Name");

                        levelCompleted[name] = Convert.ToBoolean(reader.GetAttribute("Completed"));
                        levelBestTime[name] = TimeSpan.Parse(reader.GetAttribute("BestTime"));
                    }
                }

                reader.Dispose();
            }            

            return true;
        }

        public bool SaveLevelInfo()
        {

            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = Environment.NewLine;

            XmlWriter writer = XmlWriter.Create("levelinfo.xml", settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("LevelInfo");
            
            foreach (KeyValuePair<string,bool> a in levelCompleted)
            {
                writer.WriteStartElement("Level");

                writer.WriteAttributeString("Name", a.Key);
                writer.WriteAttributeString("Completed", a.Value.ToString());
                writer.WriteAttributeString("BestTime", levelBestTime[a.Key].ToString());


                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Dispose();

            MD5 md5 = MD5.Create();
            Stream file = File.OpenRead("levelinfo.xml");
            byte[] hash = md5.ComputeHash(file);           
            file.Close();
           

            file = File.OpenWrite("config.dat");
            file.Write(hash, 0, hash.Count());
            file.Close();

            return true;
        }

        Sprite star;

        Vector2f[] starField = new Vector2f[20];


        void DrawGameWonScreen(RenderTexture buffer)
        {
            spr.Texture = gameWonTexture;
            spr.Position = new Vector2f(0, 0);
            spr.Color = new Color(255, 255, 255, 255);
            buffer.Draw(spr);

            for (int i = 0; i < starField.Count(); i++)
            {
                string texture = "star";

                if ((i + menuC) % 20 <= 5) texture = "star";
                if ((i + menuC) % 20 > 5 && (i + menuC) % 20 <= 10) texture = "star2";
                if ((i + menuC) % 20 > 10 && (i + menuC) % 20 <= 15) texture = "star3";
                if ((i + menuC) % 20 > 15) texture = "star2";

                star = TextureMan.GetSprite(texture, false);
                star.Position = starField[i];
                buffer.Draw(star);
            }

            spr.Texture = gameWonTextTexture;
            spr.Position = new Vector2f(0, 0);

            if (menuC < 120)
            {
                spr.Position = new Vector2f(0, 120 - menuC);
                //     spr.Scale = new Vector2f((120-menuC) / 100 + 1, (120-menuC) / 100 + 1);
            }

            spr.Color = new Color(255, 255, 255, 255);
            //  spr.Scale = new Vector2f(1, 1);
            buffer.Draw(spr);

        }

        void DrawLevelWonScreen(RenderTexture buffer)
        {
            spr.Texture = congratulationsTexture;
            spr.Position = new Vector2f(0, 0);
            spr.Color = new Color(255, 255, 255, 255);
            buffer.Draw(spr);

            txt.DisplayedString = "level complete!";
            FloatRect textRect = txt.GetLocalBounds();
            txt.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f,
                                        textRect.Top + textRect.Height / 2.0f);

            txt.Font = TextureMan.Font;
            txt.CharacterSize = 8;
            txt.Color = new Color(255, 179, 77);
            txt.Position = new Vector2f(160, 20);

            buffer.Draw(txt);

            txt.DisplayedString = "your time:";
            textRect = txt.GetLocalBounds();
            txt.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f,
                                        textRect.Top + textRect.Height / 2.0f);
            txt.Position = new Vector2f(160, 90);
            buffer.Draw(txt);

            txt.DisplayedString = LevelCompletiontime.ToString(@"mm\:ss\.ff");
            textRect = txt.GetLocalBounds();
            txt.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f,
                                        textRect.Top + textRect.Height / 2.0f);

            txt.Position = new Vector2f(160, 100);
            buffer.Draw(txt);

        }

        void DrawPlayMenuScreen(RenderTexture buffer)
        {
            spr.Texture = levelSelectBgTexture;
            spr.Position = new Vector2f(0, 0);
            spr.Color = new Color(255, 255, 255, 255);
            buffer.Draw(spr);

            txt.DisplayedString = "Back to main menu";

            txt.Font = TextureMan.Font;
            txt.CharacterSize = 8;
            txt.Color = new Color(226, 149, 37);
            txt.Position = new Vector2f(30, 10);

            buffer.Draw(txt);

            if (Cheater)
            {
                txt.DisplayedString = "Please don't cheat :)";
                txt.Position = new Vector2f(180, 10);

                buffer.Draw(txt);
            }

            int pos = 20;

            foreach (KeyValuePair<string, string> level in levels)
            {
                txt.DisplayedString = level.Key;
                txt.Position = new Vector2f(30, pos);

                if (!levelCompleted[level.Value])
                {
                    txt.Color = new Color(226, 149, 37);
                }
                else txt.Color = new Color(255, 255, 255);

                buffer.Draw(txt);

                pos += 10;
            }

            txt.Color = new Color(226, 149, 37);
            txt.DisplayedString = ">";
            txt.Position = new Vector2f(20, menuOption * 10 + 10);
            buffer.Draw(txt);

            if (menuOption > 0)
            {
                if (levelCompleted.ElementAt(menuOption - 1).Value == true)
                {
                    txt.Color = new Color(255, 255, 255);

                    txt.DisplayedString = "Level Completed";
                    txt.Position = new Vector2f(190, 50);
                    buffer.Draw(txt);

                    txt.DisplayedString = "Best time: " + levelBestTime.ElementAt(menuOption - 1).Value.ToString(@"mm\:ss\.ff");
                    txt.Position = new Vector2f(190, 60);
                    buffer.Draw(txt);
                }


            }


        }

        public void DrawMainMenuScreen(RenderTexture buffer)
        {
            spr.Texture = crateTexture;
            spr.Position = new Vector2f(0, 0);

            spr.Color = new Color(255, 255, 255, 255);
            buffer.Draw(spr);


            spr.Position = new Vector2f(0, 0);
            spr.Texture = superTexture;
            buffer.Draw(spr);
            spr.Texture = starboxTexture;
            buffer.Draw(spr);

            txt.DisplayedString = "Start game";

            txt.Font = TextureMan.Font;
            txt.CharacterSize = 8;
            txt.Color = new Color(226, 149, 37);
            txt.Position = new Vector2f(80, 90);

            buffer.Draw(txt);

            txt.DisplayedString = "Quit";
            txt.Position = new Vector2f(200, 90);

            buffer.Draw(txt);


            txt.DisplayedString = "1.1";
            txt.Position = new Vector2f(2, 111);

            buffer.Draw(txt);

            txt.DisplayedString = ">";

            if (menuOption == 0)
            {
                txt.Position = new Vector2f(65, 90);
            }
            else txt.Position = new Vector2f(185, 90);

            buffer.Draw(txt);

        }

        public void DrawIntroScreen(RenderTexture buffer)
        {
            if (menuC > 120 && menuC <= 140)
            {
                spr.Texture = crateTexture;
                spr.Position = new Vector2f(0, 0);
                spr.Color = new Color(255, 255, 255, (byte)((menuC - 120) * 12));

                buffer.Draw(spr);
            }

            if (menuC > 140)
            {
                spr.Texture = crateTexture;
                spr.Position = new Vector2f(0, 0);
                spr.Color = new Color(255, 255, 255, 255);

                buffer.Draw(spr);
            }

            if (menuC <= 80)
            {

                spr.Texture = starboxTexture;
                spr.Position = new Vector2f(0, 80 - menuC);
                spr.Color = new Color(255, 255, 255, (byte)(menuC * 3.1f));

                buffer.Draw(spr);
            }

            if (menuC > 80)
            {
                spr.Texture = starboxTexture;
                spr.Position = new Vector2f(0, 0);
                spr.Color = new Color(255, 255, 255, 255);

                buffer.Draw(spr);
            }

            if (menuC > 80 && menuC <= 120)
            {
                spr.Texture = superTexture;
                spr.Position = new Vector2f(0, menuC - 120);
                spr.Color = new Color(255, 255, 255, 255);

                buffer.Draw(spr);
            }

            if (menuC > 120)
            {
                spr.Texture = superTexture;
                spr.Position = new Vector2f(0, 0);
                spr.Color = new Color(255, 255, 255, 255);

                buffer.Draw(spr);
            }

        }

        public void Draw(RenderTexture buffer)
        {
            buffer.Clear(new Color(0, 0, 0));

            txt.Origin = new Vector2f(0, 0);

            if (State == MenuState.GameWon)
            {
                DrawGameWonScreen(buffer);
            }


            if (State == MenuState.LevelWon)
            {
                DrawLevelWonScreen(buffer);
            }

            if (State == MenuState.PlayMenu)
            {
                DrawPlayMenuScreen(buffer);                
            }


            if (State == MenuState.MainMenu)
            {
                DrawMainMenuScreen(buffer);
            }



            if (State == MenuState.Intro)
            {
                DrawIntroScreen(buffer);
            }


            if (menuC < 25)
            {
                RectangleShape rect = new RectangleShape();

                rect.Position = new Vector2f(0, 0);
                rect.Size = new Vector2f(320, 120);
                rect.FillColor = new Color(0, 0, 0, (byte)(255 - menuC * 10));

                buffer.Draw(rect);
            }
        }

        int menuC = 0;

        bool keypressTrigger = false;

        public void CounterReset()
        {
            menuC = 0;
        }
        
        bool gameWon = false;

        void GameWonLogic()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.Space)
                || Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {

                if (menuC > 100 && !keypressTrigger)
                {
                    State = MenuState.PlayMenu;

                    menuC = 0;
                }
                keypressTrigger = true;
            }


            if (menuC == 2)
            {


                SoundMan.PlayMusic(2);

                for (int i = 0; i < starField.Count(); i++)
                {
                    starField[i] = new Vector2f(r.Next(0, 320), r.Next(0, 120));
                    Console.WriteLine(starField[i].X);
                }
            }

            if (menuC > 2)
            {
                for (int i = 0; i < starField.Count(); i++)
                {
                    starField[i] = new Vector2f(starField[i].X, starField[i].Y + 1 + (i / 10f));
                    if (starField[i].Y > 130)
                    {
                        starField[i] = new Vector2f(r.Next(0, 320), -16);
                    }

                }
            }

        }

        void LevelWonLogic()
        {


            if (!levelCompleted[LevelName])
            {
                gameWon = true;

                levelCompleted[LevelName] = true;

                foreach (KeyValuePair<string, bool> alevel in levelCompleted)
                {

                    if (alevel.Value == false) gameWon = false;
                }
            }

            int a = LevelCompletiontime.CompareTo(levelBestTime[LevelName]);

            if (a <= 0)
            {
                levelBestTime[LevelName] = LevelCompletiontime;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.Space)
                || Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {


                State = MenuState.PlayMenu;
                if (gameWon)
                {
                    gameWon = false;
                    State = MenuState.GameWon;

                }

                menuC = 0;
                keypressTrigger = true;
            }

        }

        Random r = new Random();

        void PlayMenuLogic()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                if (!keypressTrigger) if (menuOption > 0) menuOption--;
                keypressTrigger = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down))
            {
                if (!keypressTrigger) if (menuOption < levels.Count) menuOption++;
                keypressTrigger = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                if (!keypressTrigger)
                {
                    State = MenuState.MainMenu;
                    menuC = 0;
                }
                keypressTrigger = true;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {


                if (!keypressTrigger)
                {

                    Cheater = false;

                    if (menuOption == 0)
                    {
                        State = MenuState.MainMenu;
                        menuC = 0;
                    }
                    else
                    {
                        LevelName = levels.ElementAt(menuOption - 1).Value;
                        State = MenuState.StartLevel;
                        menuC = 0;
                    }
                }
                keypressTrigger = true;
            }

        }

        public void Loop(RenderWindow window)
        {
            menuC++;           
            

            if (State == MenuState.GameWon)
            {
                GameWonLogic();
            }

            if (State == MenuState.Intro)
            {
                if (menuC == 160)
                {
                    State = MenuState.MainMenu;
                    menuC = 40;
                }
            }

            if (State == MenuState.LevelWon)
            {
                LevelWonLogic(); 
            }

            if (State == MenuState.PlayMenu)
            {
                PlayMenuLogic();
            }

            if (!Keyboard.IsKeyPressed(Keyboard.Key.Left) && !Keyboard.IsKeyPressed(Keyboard.Key.Right)
                && !Keyboard.IsKeyPressed(Keyboard.Key.Up) && !Keyboard.IsKeyPressed(Keyboard.Key.Down) 
                && !Keyboard.IsKeyPressed(Keyboard.Key.Return) && !Keyboard.IsKeyPressed(Keyboard.Key.Escape)
                && !Keyboard.IsKeyPressed(Keyboard.Key.Space)
                )
            {
                keypressTrigger = false;
            }

            if (State == MenuState.MainMenu)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left) || Keyboard.IsKeyPressed(Keyboard.Key.Right))
                {
                    if (keypressTrigger == false)
                    {
                        if (menuOption == 0) menuOption = 1;
                        else menuOption = 0;

                        keypressTrigger = true;
                    }
                }

                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                {
                    if (!keypressTrigger)
                    {
                        State = MenuState.Quit;
                        menuC = 0;
                    }
                }

                if ((Keyboard.IsKeyPressed(Keyboard.Key.Return) || Keyboard.IsKeyPressed(Keyboard.Key.Space)) && !keypressTrigger)
                {
                    keypressTrigger = true;

                    if (menuOption == 0)
                    {
                        State = MenuState.PlayMenu;
                        menuC = 0;
                    }
                    else 
                    { 
                        State = MenuState.Quit;
                        menuC = 0;
                    }
                }
            }
        }
    }
}
