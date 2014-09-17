namespace TrainBox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    using SFML;
    using SFML.Audio;
    using SFML.Graphics;
    using SFML.Window;

    using System.Xml;

    class TrainBoxApp
    {
        bool fullScreen = false;

        RenderWindow window;
        bool windowHasFocus = true;

        uint screenSizeX;
        uint screenSizeY;

        bool buttonTriggerX = false;
        bool buttonTriggerZ = false;

        Level level = new Level();

        RenderTexture buffer;
        Sprite bufferSpr;

        Editor editor = new Editor();
        Menu menu = new Menu();


        string levelName = "level1";
        
        enum GameState {  StartLevel, GamePlay, ResetFadeOut, ResetFadeIn, LevelFinished, Paused };

        GameState gameState;
        int gameStateC = 0;

        float screenScale = 1;

        public void StartTrainBoxApp()
        {

            Level.PrepareEntityTypes();

            bool quit = false;

            TextureMan.LoadTextures();
            TextureMan.CreateShadows();

            bufferSpr = new Sprite();

            buffer = new RenderTexture(320, 120);
            buffer.Clear(Color.Black);

            menu.Active = true;
            menu.State = Menu.MenuState.Intro;
            menu.CounterReset();
            menu.LoadLevelInfo();

            while (true)
            {
                if (fullScreen)
                {
                    screenSizeX = 1366;
                    screenSizeY = 768;                    
                }
                else
                {
                    screenSizeX = 960;
                    screenSizeY = 540;                    
                }

                if (fullScreen) window = new RenderWindow(new VideoMode(screenSizeX, screenSizeY), 
                    "Super Starbox", Styles.Fullscreen);  
                else window = new RenderWindow(new VideoMode(screenSizeX, screenSizeY), 
                    "Super Starbox", Styles.Titlebar);
                    
                window.SetMouseCursorVisible(!fullScreen);

                window.SetVisible(true);
                window.Closed += new EventHandler(OnClosed);
                window.SetFramerateLimit(60);

                window.LostFocus += new EventHandler(OnLostFocus);
                window.GainedFocus += new EventHandler(OnGainedFocus);

                bool editorButtonTrigger = false;

                SoundMan.PlayMusic();

                while (true)
                {

                    buffer.Clear(Color.Black);

                    if (!menu.Active)
                    {
                        level.DrawLevel(buffer);
                    }
                    
                    
                    if (fadeOut != 0)
                    {
                        RectangleShape rect = new RectangleShape();

                        rect.Position = new Vector2f(0,0);
                        rect.Size = new Vector2f(320, 120);
                        rect.FillColor = new Color(0, 0, 0, (byte)fadeOut);

                        buffer.Draw(rect);
                    }

                    if (menu.Active) menu.Draw(buffer);
                    if (editor.Active) editor.Draw(buffer);

                    buffer.Display();

                    bufferSpr.Texture = buffer.Texture;
                    bufferSpr.Scale = new Vector2f((screenSizeX / 320f) * screenScale, (screenSizeY / 120f) * screenScale);
                    
                    bufferSpr.Origin = new Vector2f(160, 60);
                    bufferSpr.Position = new Vector2f(screenSizeX/2, screenSizeY/2);

                    if (screenShake > 0)
                    {
                        bufferSpr.Position = new Vector2f(bufferSpr.Position.X + (float)new Random().Next(-200,200)/75, 
                                                          bufferSpr.Position.Y + (float)new Random().Next(-200,200)/75);
                    }

                    if (gameState == GameState.ResetFadeOut)
                    {
                        
                        bufferSpr.Rotation = gameStateC / 2;

                        screenScale = 1f + (float)gameStateC / 50f;
              
                    }

                    if (gameState == GameState.LevelFinished)
                    {

                        bufferSpr.Rotation = gameStateC / 2;

                        screenScale = 1f + (float)gameStateC / 25f;

                    }
                    
                    if (gameState == GameState.ResetFadeIn)
                    {

                        bufferSpr.Rotation = (50-gameStateC) / 2;

                        screenScale = 1f + (float)(50 - gameStateC) / 50f;

                    }                    

                    if (gameState == GameState.GamePlay || menu.Active)
                    {
                        screenScale = 1;
                        bufferSpr.Rotation = 0;
                    }

                    window.DispatchEvents();
                    window.Clear(Color.Black);
                    window.Draw(bufferSpr);
                    window.Display();


                    if (Keyboard.IsKeyPressed(Keyboard.Key.F4))
                    {
                        fullScreen = !fullScreen;
                        break;
                    }

                    if (Keyboard.IsKeyPressed(Keyboard.Key.F10) && false)
                    {
                        if (!menu.Active)
                        if (!editorButtonTrigger)
                        {
                            editor.Active = !editor.Active;
                            if (editor.Active)
                            {
                                editor.LevelName = levelName;
                                level.ResetLevel();
                                level.ResetScrollBounds();
                                CollisionMan.Entities = level.GetEntities();
                                EntityFinder.Entities = level.GetEntities();
                                editor.Level = level;
                            }
                            else
                            {
                                levelName = editor.LevelName;
                                level = editor.Level;
                                level.FindScrollBounds();
                                gameState = GameState.StartLevel;
                            }
                        }
                        editorButtonTrigger = true;
                    }
                    else editorButtonTrigger = false;

                    if (!editor.Active && !menu.Active)
                    {
                        GameLoop();
                    }

                    if (editor.Active) if (windowHasFocus) editor.Loop(window);
                    if (menu.Active)
                    {
                        if (windowHasFocus) menu.Loop(window);

                        if (menu.State == Menu.MenuState.Quit)
                        {
                            quit = true;
                            break;
                        }

                        if (menu.State == Menu.MenuState.StartLevel)
                        {
                            levelName = menu.LevelName;

                            menu.Active = false;
                            level.LoadLevel(levelName);

                            gameStateC = 0;
                            gameState = GameState.ResetFadeIn;
                            fadeOut = 255;

                            CollisionMan.Entities = level.GetEntities();
                            EntityFinder.Entities = level.GetEntities();
                        }
                    }
                }

                window.Dispose();
                if (quit) break;
            }

            menu.SaveLevelInfo();
        }

        bool buttonTriggerR = false;
        bool buttonTriggerP = false;

        int fadeOut = 0;
        int screenShake = 0;

        void ControlPlayer(Player player)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up) && !Keyboard.IsKeyPressed(Keyboard.Key.Z))
            {
                player.ClimbUp(level.GetEntities());
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Down) && !Keyboard.IsKeyPressed(Keyboard.Key.Z))
            {
                player.ClimbDown(level.GetEntities());
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
            {
                player.WalkLeft();
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
            {
                player.WalkRight();
            }
            else
            {
                player.StopWalk();
            }

            if (!Keyboard.IsKeyPressed(Keyboard.Key.Z)) buttonTriggerZ = false;
            if (!Keyboard.IsKeyPressed(Keyboard.Key.X)) buttonTriggerX = false;
            if (!Keyboard.IsKeyPressed(Keyboard.Key.R)) buttonTriggerR = false;
            
            if (!buttonTriggerZ && Keyboard.IsKeyPressed(Keyboard.Key.Z))
            {

                player.DoJump = true;
                buttonTriggerZ = true;
                player.Hooked = false;
                player.Carried = false;
            }
            else
            {
                player.DoJump = false;
            }

            if (!buttonTriggerX && Keyboard.IsKeyPressed(Keyboard.Key.X))
            {
                player.PickUp();
                buttonTriggerX = true;
            }

            if (!buttonTriggerR && Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                gameState = GameState.ResetFadeOut;
                gameStateC = 0;

                buttonTriggerR = true;
            }


        }


        bool GameLoop()
        {
            bool openDoor = true;

            CollisionMan.PopulateCells();
            CollisionMan.CheckCollisionCells();

            if (screenShake > 0) screenShake--;

            gameStateC++;

            if (gameState == GameState.StartLevel)
            {
                level.Timer.Reset();
                level.Timer.Start();
               
                gameState = GameState.GamePlay;
            }

            if (gameState == GameState.ResetFadeOut)
            {
                level.Timer.Stop();

                fadeOut = gameStateC * 10;

                if (gameStateC == 25)
                {
                    gameStateC = 0;
                    gameState = GameState.ResetFadeIn;

                    level.ResetLevel();
                }
            }

            if (gameState == GameState.ResetFadeIn)
            {
                level.ScrollTo(EntityFinder.FindOfType(typeof(Player)).Position, true);

                if (gameStateC > 10) fadeOut = 255 - (int)((gameStateC - 10) * 6.3);
                else fadeOut = 255;

                if (gameStateC == 50)
                {
                    gameStateC = 0;
                    gameState = GameState.StartLevel;
                }
            }

            if (gameState == GameState.LevelFinished)
            {
                level.Timer.Stop();

                if (gameStateC < 50) fadeOut = gameStateC * 5;
                else fadeOut = 255;

                if (gameStateC == 2)
                {
                    SoundMan.PlaySound("win", SoundMan.Center);
                }


                if (gameState == GameState.LevelFinished && gameStateC >= 70)
                {
                    menu.Active = true;
                    menu.State = Menu.MenuState.LevelWon;
                    menu.CounterReset();
                    menu.LevelCompletiontime = level.Timer.Elapsed;
                    menu.LevelName = levelName;

                    return false;
                }

            }

            if (!Keyboard.IsKeyPressed(Keyboard.Key.P)) buttonTriggerP = false;
            if (!buttonTriggerP && Keyboard.IsKeyPressed(Keyboard.Key.P))
            {
                if (gameState == GameState.GamePlay)
                {
                    gameState = GameState.Paused;
                    level.Timer.Stop();
                }
                else if (gameState == GameState.Paused)
                {
                    gameState = GameState.GamePlay;
                    level.Timer.Start();
                }

                buttonTriggerP = true;
            }

            if (gameState == GameState.Paused) fadeOut = 128;

            if (gameState == GameState.GamePlay)
            {
                fadeOut = 0;


                for (int i = 0; i < level.GetEntities().Count; i++)
                {
                    Entity ent = level.GetEntities()[i];

                    if (ent is StarBlock)
                    {
                        StarBlock starBlock = (StarBlock)ent;

                        if (!starBlock.StarOnTop) openDoor = false;
                    }

                    if (windowHasFocus)
                    {

                        if (ent is Player)
                        {
                            Player player = (Player)ent;
                            level.ScrollTo(player.Position, false);
                            ControlPlayer(player);

                            if (player.Position.Y > 140)
                            {
                               

                                gameState = GameState.ResetFadeOut;
                                gameStateC = 0;
                            }
                        }

                        if (!ent.Destroyed)
                        {
                            ent.Act();
                        }
                        else 
                        {
                            if (ent.ScreenShake)
                            {
                                screenShake = 50;
                                ent.ScreenShake = false;
                            }
                        }

                    }
                }

                foreach (Entity ent in level.GetEntities())
                {
                    if (ent is Exit)
                    {
                        Exit exit = (Exit)ent;

                        if (!exit.Open && openDoor)
                        {
                            SoundMan.PlaySound("open", exit.Position);
                            exit.BounceC = 20;
                        }
                        
                        exit.Open = openDoor;

                        if (openDoor)
                        {
                            Entity entp = EntityFinder.FindOfType(typeof(Player));
                            if (entp != null)
                            {
                                Player pl = (Player)entp;

                                if (pl.BlockedLeftEnt == exit || pl.BlockedRightEnt == exit || pl.CeilingEnt == exit || pl.GroundEnt == exit)
                                {
                                   
                                    gameState = GameState.LevelFinished;
                                    gameStateC = 0;
                                }
                            }
                        }
                    }
                }
            }

            SoundMan.Center = new Vector2f(level.Scroll.X + 160, level.Scroll.Y + 60);
           
            //level.GetEntities().RemoveAll(a => a.Destroyed);

            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape) && windowHasFocus)
            {
                menu.Active = true;
                menu.State = Menu.MenuState.PlayMenu;
                menu.CounterReset();
            }

            return true;
        }



        void OnClosed(object sender, EventArgs e)
        {
            window.Close();
        }

        void OnGainedFocus(object sender, EventArgs e)
        {
            windowHasFocus = true;
        }

        void OnLostFocus(object sender, EventArgs e)
        {
            windowHasFocus = false;
        }


    }

}