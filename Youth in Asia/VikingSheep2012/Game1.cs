using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace VikingSheep2012
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Viking viking;
        Vector2 c_pos;
        Vector2 c_target;
        Vector4[] positionHistory = new Vector4[441];
        Vector3[] stateHistory = new Vector3[441];
        Vector4[,] sheepHistory = new Vector4[42, 441];
        Vector4[,] sheepStateHistory = new Vector4[42, 441];
        Texture2D ground;
        Texture2D city;
        Texture2D valhalla;
        Texture2D background;
        Texture2D sheepGfx;
        Texture2D ram;
        Texture2D bombSheep;
        SpriteFont font;
        SpriteFont medFont;
        SpriteFont smallFont;
        Sheep[] sheep = new Sheep[42];
        Random ranGen = new Random(1);
        KeyboardState kbs;
        KeyboardState old_kbs;
        GamePadState gps;
        GamePadState old_gps;
        MouseState ms;
        MouseState old_ms;
        int sheepCount = 0;
        int[] sheepCountHistory = new int[441];
        int posCount = 0;
        string[] comments = new string[22];
        string line;

        Texture2D overlay;
        Texture2D likesBar;
        Texture2D fullScreenButton;
        Texture2D pauseButton;
        Texture2D playButton;
        Texture2D volumeBar;
        Texture2D volumeSlider;
        Texture2D fullScreenButtonLarge;
        Texture2D pauseButtonLarge;
        Texture2D playButtonLarge;
        Texture2D volumeBarLarge;
        Texture2D volumeSliderLarge;
        Texture2D youTubeBarLarge;
        Texture2D title;
        Texture2D game1;
        Texture2D game2;
        int views = -1;
        string viewString = "";
        int likes = 0;
        int dislikes = 0;
        string likeString = "";
        int level = 0;
        int timer = 0;
        string timerString = "";
        int comment = 0;
        int commentTimer = 901;
        string user = "";
        string commentText = "";

        bool fullScreen = false;
        bool pause = false;
        bool start = false;

        int volume = 100;

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue bgm;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            c_pos = Vector2.Zero;
            c_target = c_pos;
            audioEngine = new AudioEngine("Content\\Sheep1.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");
            bgm = soundBank.GetCue("sexysoundtrack");
            StreamReader sr = new StreamReader("Content\\Comment.txt");
            int counter = 0;
            while ((line = sr.ReadLine()) != null)
            {
                comments[counter] = line; ;
                counter++;
            }
            InitGraphicsMode(1280, 720, false);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viking = new Viking(Vector2.Zero, Vector2.Zero, Content.Load<Texture2D>("Vkfall"), Content.Load<Texture2D>("Vkjump"), Content.Load<Texture2D>("VkRun"), Content.Load<Texture2D>("Man Static Transp"), spriteBatch, PlayerIndex.One);
            valhalla = Content.Load<Texture2D>("Valhalla_reloaded_A");
            sheepGfx = Content.Load<Texture2D>("Sheep");
            ram = Content.Load<Texture2D>("ram");
            city = Content.Load<Texture2D>("city");
            bombSheep = Content.Load<Texture2D>("Sheepill");
            for (byte i = 0; i < 42; i++)
                NewSheep(i);
            ground = Content.Load<Texture2D>("Ground");

            overlay = Content.Load<Texture2D>("YouTubeLayout_MASTER_FLATTENED");
            likesBar = Content.Load<Texture2D>("Likes");
            fullScreenButton = Content.Load<Texture2D>("fullscreen_button_top");
            pauseButton = Content.Load<Texture2D>("pause_button");
            playButton = Content.Load<Texture2D>("play_button");
            volumeBar = Content.Load<Texture2D>("volume_bar");
            volumeSlider = Content.Load<Texture2D>("volume_slider");
            youTubeBarLarge = Content.Load<Texture2D>("youtube_bar_large");
            fullScreenButtonLarge = Content.Load<Texture2D>("fullscreen_button_large");
            pauseButtonLarge = Content.Load<Texture2D>("pause_button_large");
            playButtonLarge = Content.Load<Texture2D>("play_button_large");
            volumeBarLarge = Content.Load<Texture2D>("volume_bar_large");
            volumeSliderLarge = Content.Load<Texture2D>("volume_slider_large");
            title = Content.Load<Texture2D>("title_screen");
            game1 = Content.Load<Texture2D>("Game1");
            game2 = Content.Load<Texture2D>("Game2");
            font = Content.Load<SpriteFont>("font");
            medFont = Content.Load<SpriteFont>("medFont");
            smallFont = Content.Load<SpriteFont>("smallFont");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public void ResetGame()
        {
            sheepCount = 0;
            posCount = 0;
            viking.SetPos(Vector2.Zero);
            viking.SetVel(Vector2.Zero);
            viking.SetState(new Vector3(0, 0, 0));
            views = -1;
            likes = 0;
            dislikes = 0;
            timer = 0;
            for (byte i = 0; i < 42; i++)
                NewSheep(i);
        }

        public void NewSheep(byte id)
        {
            byte num = (byte)ranGen.Next(10);
            Vector2 vel = Vector2.Zero;
            byte type = 0;
            Texture2D gfx = sheepGfx;
            switch (num)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    type = 0;
                    gfx = sheepGfx;
                    break;
                case 4:
                case 5:
                case 6:
                    vel = new Vector2((float)(ranGen.NextDouble() * 10.0f - 5.0f), 0.0f);
                    type = 1;
                    gfx = sheepGfx;
                    break;
                case 7:
                case 8:
                    type = 2;
                    gfx = bombSheep;
                    break;
                case 9:
                    type = 3;
                    gfx = ram;
                    break;
            }
            sheep[id] = new Sheep(new Vector2(ranGen.Next(1180) - 590, -sheepCount * 210 - 210), vel, gfx, spriteBatch, type, (byte)(ranGen.Next(240) + 15), (ranGen.Next(2) == 0));
            sheepCount++;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            kbs = Keyboard.GetState();
            gps = GamePad.GetState(PlayerIndex.One);
            ms = Mouse.GetState();
            if ((kbs.IsKeyDown(Keys.Enter) || gps.IsButtonDown(Buttons.Start)) && old_kbs.IsKeyUp(Keys.Enter) && old_gps.IsButtonUp(Buttons.Start))
            {
                pause = !pause;
                if (pause)
                    bgm.Pause();
                else
                    bgm.Resume();
            }
            if ((kbs.IsKeyDown(Keys.O) || gps.IsButtonDown(Buttons.LeftShoulder)) && old_kbs.IsKeyUp(Keys.O) && old_gps.IsButtonUp(Buttons.LeftShoulder))
            {
                volume -= 10;
                if (volume < 0)
                    volume = 0;
                audioEngine.GetCategory("Default").SetVolume(volume * 0.01f);
            }
            if ((kbs.IsKeyDown(Keys.P) || gps.IsButtonDown(Buttons.RightShoulder)) && old_kbs.IsKeyUp(Keys.P) && old_gps.IsButtonUp(Buttons.RightShoulder))
            {
                volume += 10;
                if (volume > 100)
                    volume = 100;
                audioEngine.GetCategory("Default").SetVolume(volume * 0.01f);
            }
            if (start)
            {
                if (kbs.IsKeyDown(Keys.Z) || kbs.IsKeyDown(Keys.M) || (gps.Triggers.Left > 0.2f))
                {
                    byte loops = 1;
                    if (gps.Triggers.Left > 0.2f)
                        loops = (byte)(gps.Triggers.Left * 5);
                    for (byte j = 0; j < loops; j++)
                    {
                        posCount--;
                        if (posCount < 0)
                        {
                            posCount = 0;
                            timer++;
                        }
                        viking.SetPos(new Vector2(positionHistory[posCount].X, positionHistory[posCount].Y));
                        viking.SetVel(new Vector2(positionHistory[posCount].Z, positionHistory[posCount].W));
                        viking.SetState(stateHistory[posCount]);
                        for (byte i = 0; i < 42; i++)
                        {
                            sheep[i].SetPos(new Vector2(sheepHistory[i, posCount].X, sheepHistory[i, posCount].Y));
                            sheep[i].SetVel(new Vector2(sheepHistory[i, posCount].Z, sheepHistory[i, posCount].W));
                            sheep[i].SetState(sheepStateHistory[i, posCount]);
                            switch (sheep[i].GetBehaviour())
                            {
                                case 0:
                                case 1:
                                    sheep[i].SetGfx(sheepGfx);
                                    break;
                                case 2:
                                    sheep[i].SetGfx(bombSheep);
                                    break;
                                case 3:
                                    sheep[i].SetGfx(ram);
                                    break;
                            }
                        }
                        sheepCount = sheepCountHistory[posCount];
                        timer--;
                        int minutes = timer / 3600;
                        int seconds = (timer % 3600) / 60;
                        timerString = seconds.ToString();
                        if (seconds < 10)
                            timerString = "0" + timerString;
                        timerString = minutes.ToString() + ":" + timerString + " / 5:00";
                    }
                    c_target = viking.GetPos() + viking.GetVel() - gps.ThumbSticks.Right * 250.0f - new Vector2(640, 360);
                    if (c_target.Y > -540)
                        c_target.Y = -540;
                    c_pos += (c_target - c_pos) * 0.5f;
                    c_pos.X = -640;
                }
                else if (!pause)
                {
                    byte type = viking.Update(sheep);
                    if (type < 255)
                    {
                        dislikes++;
                        switch (type)
                        {
                            case 0:
                            case 1:
                                soundBank.PlayCue("Baa");
                                break;
                            case 2:
                                soundBank.PlayCue("Sick");
                                break;
                            case 3:
                                soundBank.PlayCue("GeneralRaaam");
                                break;
                        }
                    }
                    for (byte i = 0; i < 42; i++)//!!!!!!!!!!!!!!!!!!!!!!!!!
                    {
                        if (sheep[i].Update(viking))
                            soundBank.PlayCue("pop");
                        if (sheep[i].GetPos().Y > viking.GetPos().Y + 1764)
                            NewSheep(i);
                    }
                    positionHistory[posCount] = new Vector4(viking.GetPos().X, viking.GetPos().Y, viking.GetVel().X, viking.GetVel().Y);
                    stateHistory[posCount] = viking.GetState();
                    for (byte i = 0; i < 42; i++)
                    {
                        sheepHistory[i, posCount] = new Vector4(sheep[i].GetPos().X, sheep[i].GetPos().Y, sheep[i].GetVel().X, sheep[i].GetVel().Y);
                        sheepStateHistory[i, posCount] = sheep[i].GetState();
                    }
                    sheepCountHistory[posCount] = sheepCount;
                    posCount++;
                    if (posCount == 441)
                    {
                        for (short i = 0; i < 440; i++)
                        {
                            positionHistory[i] = positionHistory[i + 1];
                            stateHistory[i] = stateHistory[i + 1];
                            for (byte j = 0; j < 42; j++)
                            {
                                sheepHistory[j, i] = sheepHistory[j, i + 1];
                                sheepStateHistory[j, i] = sheepStateHistory[j, i + 1];
                            }
                            sheepCountHistory[i] = sheepCountHistory[i + 1];
                        }
                        posCount--;
                    }
                    if (-viking.GetPos().Y > views)
                    {
                        views = (int)-viking.GetPos().Y;
                        viewString = "";
                        int millions = views / 1000000;
                        string millionString = millions.ToString();
                        int thousands = (views - millions * 1000000) / 1000;
                        string thousandString = thousands.ToString();
                        if (millions > 0)
                        {
                            if (thousands < 100)
                                thousandString = "0" + thousandString;
                            if (thousands < 10)
                                thousandString = "0" + thousandString;
                        }
                        int ones = views % 1000;
                        string oneString = ones.ToString();
                        if (millions > 0 || thousands > 0)
                        {
                            if (ones < 100)
                                oneString = "0" + oneString;
                            if (ones < 10)
                                oneString = "0" + oneString;
                        }
                        if (millions > 0)
                            viewString += millionString + ",";
                        if (millions > 0 || thousands > 0)
                            viewString += thousandString + ",";
                        viewString += oneString;
                        likes = (int)Math.Sqrt(views);
                    }
                    likeString = likes.ToString() + " likes, " + dislikes.ToString() + " dislikes";
                    timer++;
                    int minutes = timer / 3600;
                    int seconds = (timer % 3600) / 60;
                    timerString = seconds.ToString();
                    if (seconds < 10)
                        timerString = "0" + timerString;
                    timerString = minutes.ToString() + ":" + timerString + " / 5:00";
                    commentTimer++;
                    if (commentTimer > 900)
                    {
                        commentTimer = 0;
                        comment = ranGen.Next(22);
                        int cursor = 0;
                        while (comments[comment].Substring(cursor, 1) != "#")
                            cursor++;
                        user = comments[comment].Substring(0, cursor);
                        commentText = comments[comment].Substring(cursor + 2, comments[comment].Length - cursor - 2);
                    }
                    c_target = viking.GetPos() + viking.GetVel() - gps.ThumbSticks.Right * 250.0f - new Vector2(640, 360);
                    if (c_target.Y > -540)
                        c_target.Y = -540;
                    c_pos += (c_target - c_pos) * 0.5f;
                    c_pos.X = -640;
                }
                if (fullScreen)
                {
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        if (ms.X > 180 && ms.X < 311 && ms.Y > 657)
                        {
                            volume = (int)((ms.X - 181) / 1.28f);
                            if (volume < 0)
                                volume = 0;
                            if (volume > 100)
                                volume = 100;
                            audioEngine.GetCategory("Default").SetVolume(volume * 0.01f);
                        }
                        if (old_ms.LeftButton == ButtonState.Released)
                        {
                            if (ms.X > 1200 && ms.Y > 657)
                                fullScreen = false;
                            if (ms.X > 0 && ms.X < 113 && ms.Y > 657)
                            {
                                pause = !pause;
                                if (pause)
                                    bgm.Pause();
                                else
                                    bgm.Resume();
                            }
                        }
                    }
                }
                else
                {
                    if (ms.LeftButton == ButtonState.Pressed)
                    {
                        if (ms.X > 250 && ms.X < 317 && ms.Y > 457 && ms.Y < 485)
                        {
                            volume = (int)((ms.X - 251) / 0.64f);
                            if (volume < 0)
                                volume = 0;
                            if (volume > 100)
                                volume = 100;
                            audioEngine.GetCategory("Default").SetVolume(volume * 0.01f);
                        }
                        if (old_ms.LeftButton == ButtonState.Released)
                        {
                            if (ms.X > 763 && ms.X < 787 && ms.Y > 457 && ms.Y < 485)
                                fullScreen = true;
                            if (ms.X > 155 && ms.X < 211 && ms.Y > 457 && ms.Y < 485)
                            {
                                pause = !pause;
                                if (pause)
                                    bgm.Pause();
                                else
                                    bgm.Resume();
                            }
                            if (ms.X > 822 && ms.Y > 103 && ms.Y < 172)
                            {
                                level = 0;
                                ResetGame();
                            }
                            if (ms.X > 822 && ms.Y > 183 && ms.Y < 252)
                            {
                                level = 1;
                                ResetGame();
                            }
                            if (ms.X > 1065 && ms.X < 1129 && ms.Y > 14 && ms.Y < 36)
                                Exit();
                        }
                    }
                }
            }
            else
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    if (old_ms.LeftButton == ButtonState.Released)
                    {
                        if (ms.X > 319 && ms.X < 960 && ms.Y > 479 && ms.Y < 580)
                        {
                            start = true;
                            bgm.Play();
                        }
                    }
                }                
            }
            old_kbs = kbs;
            old_gps = gps;
            old_ms = ms;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (level)
            {
                case 0:
                    GraphicsDevice.Clear(new Color(0, 151, 198));
                    background = city;
                    break;
                case 1:
                    GraphicsDevice.Clear(new Color(0, 0, 0));
                    background = valhalla;
                    break;
            }
            spriteBatch.Begin();
            if (start)
            {
                switch (level)
                {
                    case 0:
                        if (fullScreen)
                            spriteBatch.Draw(background, new Vector2(0, -(c_pos.Y * 0.04f) - 1850), Color.White);
                        else
                            spriteBatch.Draw(background, new Vector2(155, -(c_pos.Y * 0.02f) - 832), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                        break;
                    case 1:
                        if (fullScreen)
                            spriteBatch.Draw(background, new Vector2(0, -(c_pos.Y * 0.04f) - 4410), Color.White);
                        else
                            spriteBatch.Draw(background, new Vector2(155, -(c_pos.Y * 0.02f) - 2112), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                        break;
                }
                viking.Draw(c_pos, fullScreen);
                for (byte i = 0; i < 42; i++)
                    sheep[i].Draw(c_pos, fullScreen);
                if (fullScreen)
                {
                    spriteBatch.Draw(ground, new Vector2(640, -c_pos.Y), null, Color.White, 0, new Vector2(640, 0), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(youTubeBarLarge, new Vector2(0, 658), Color.White);
                    spriteBatch.Draw(fullScreenButtonLarge, new Vector2(1225, 675), Color.White);
                    if (pause)
                        spriteBatch.Draw(playButtonLarge, new Vector2(45, 673), Color.White);
                    else
                        spriteBatch.Draw(pauseButtonLarge, new Vector2(45, 673), Color.White);
                    spriteBatch.Draw(volumeBarLarge, new Vector2(179, 680), new Rectangle(0, 0, 128, 20), Color.Gray);
                    spriteBatch.Draw(volumeBarLarge, new Vector2(179, 680), new Rectangle(0, 0, (int)(volume * 1.28f), 20), Color.Red);
                    spriteBatch.Draw(volumeSliderLarge, new Vector2(175 + volume * 1.28f, 670), Color.White);
                    spriteBatch.DrawString(font, timerString, new Vector2(340, 680), Color.White);
                }
                else
                {
                    spriteBatch.Draw(ground, new Vector2(475, 96 - c_pos.Y * 0.5f), null, Color.White, 0, new Vector2(640, 0), 0.5f, SpriteEffects.None, 0);
                    spriteBatch.Draw(overlay, Vector2.Zero, Color.White);
                    spriteBatch.Draw(fullScreenButton, new Vector2(770, 462), Color.White);
                    if (pause)
                        spriteBatch.Draw(playButton, new Vector2(180, 462), Color.White);
                    else
                        spriteBatch.Draw(pauseButton, new Vector2(180, 462), Color.White);
                    spriteBatch.Draw(volumeBar, new Vector2(252, 465), new Rectangle(0, 0, 64, 10), Color.Gray);
                    spriteBatch.Draw(volumeBar, new Vector2(252, 465), new Rectangle(0, 0, (int)(volume * 0.64f), 10), Color.Red);
                    spriteBatch.Draw(volumeSlider, new Vector2(248 + volume * 0.64f, 460), Color.White);
                    spriteBatch.Draw(game1, new Vector2(822, 103), Color.White);
                    spriteBatch.Draw(game2, new Vector2(822, 183), Color.White);
                    spriteBatch.Draw(likesBar, new Vector2(630, 542), new Rectangle(0, 0, 160, 8), Color.Red);
                    if (likes + dislikes > 0)
                        spriteBatch.Draw(likesBar, new Vector2(630, 542), new Rectangle(0, 0, likes * 160 / (likes + dislikes), 8), Color.Green);
                    if (level == 0)
                        spriteBatch.DrawString(medFont, "Sheepless in Seattle", new Vector2(155, 69), Color.Black);
                    else
                        spriteBatch.DrawString(medFont, "Do Vikings dream of electric sheep?", new Vector2(155, 69), Color.Black);
                    spriteBatch.DrawString(smallFont, timerString, new Vector2(330, 462), Color.White);
                    spriteBatch.DrawString(font, viewString, new Vector2(755 - font.MeasureString(viewString).X, 502), Color.Black);
                    spriteBatch.DrawString(smallFont, likeString, new Vector2(630, 552), Color.Black);
                    spriteBatch.DrawString(smallFont, commentText, new Vector2(155, 620), Color.Black);
                    spriteBatch.DrawString(smallFont, user, new Vector2(155, 635), Color.Blue);
                }
            }
            else
                spriteBatch.Draw(title, Vector2.Zero, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
