//Author: Sophia Lin
//File Name: Game1.cs
//Project Name: PASS3
//Creation Date: Dec 14, 2022
//Modified Date: Jan 22, 2023
//Description: A platformer game with the goal of beating the enemies and collecting the gems.
/*COURSE CONTENT USAGE:
 * Output: Displaying real-time game status such as gems collected, health remaining (hp bar), and power-ups claimed
 * Variables: Storing information like speeds, health, times
 * Input: User clicks on buttons and uses keyboard to control the character
 * Arrays: Keeping track of player rectangles, bullets, platforms, objects in-game
 * Subprograms: Resetting game and adjusting door
 * Selection: Enemy player detection and player input
 * Loops: Collision detection, drawing same images, and detecting active or inactive bullets
 */
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;
using Helper;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Track the screen dimensions
        int screenWidth;
        int screenHeight;

        //Define body parts to be used for collision rectangle array indices
        const int FEET = 0;
        const int HEAD = 1;
        const int LEFT = 2;
        const int RIGHT = 3;
        const int LEFT_TOE = 4;
        const int RIGHT_TOE = 5;

        //Define directions
        const int POS = 1;
        const int NEG = -1;

        //Store the force of gravity
        const float GRAVITY = 9.8f / 60;

        //Track the player's horizontal acceleration data
        const float ACCEL = 0.8f;
        const float FRICTION = ACCEL * 0.9f;
        const float TOLERANCE = 0.9f;

        //Track gamestates
        const int MENU = 0;
        const int INSTRUCTIONS = 1;
        const int HIGHSCORES = 2;
        const int MODES = 3;
        const int GAMEPLAY = 4;
        const int PAUSE = 5;
        const int GAMEOVER = 6;

        //Track the current game state
        int gameState = MENU;

        //Arrays for backgrounds
        const int MENUBG = 0;
        const int MODESBG = 1;
        const int GAMEBG = 2;
        const int HIGHSCOREBG = 3;
        const int ENDGAMEBG = 4;
        const int INSTRUCTIONSBG = 5;

        //Tracking player states
        const int IDLE = 0;
        const int RUN = 1;
        const int JUMP = 2;
        const int ATTACK = 3;

        //Track the current player state 
        int playerState = IDLE;

        //Tracking enemy states
        const int ENEMY_IDLE = 0;
        const int ENEMY_DEATH = 1;
        const int ENEMY_ATTACK = 2;
        const int ENEMY_WALK = 3;

        //Track the current enemy state
        int enemyEasyState = ENEMY_IDLE;
        int enemyMediumState = ENEMY_IDLE;
        int enemyHardState = ENEMY_WALK;

        //Array for keeping track of gamemode
        const int EASY = 0;
        const int MEDIUM = 1;
        const int HARD = 2;

        //Storing initial game data
        const int DOOR_TIME = 5000;
        const int MAX_BULLETS = 2;
        const int MAX_ENEMY_BULLETS = 2;
        const float MAX_HEALTH = 100f;
        int gems = 0;
        float health = MAX_HEALTH;
        float maxEnemyHealth = 100f;

        //Arrays for button data
        const int PLAYBTN = 0;
        const int INSTBTN = 1;
        const int HIGHSCOREBTN = 2;
        const int EXITBTN = 3;
        const int EASYBTN = 4;
        const int MEDIUMBTN = 5;
        const int HARDBTN = 6;
        const int BACKBTN = 7;
        const int MENUBTN = 8;
        Texture2D[] btnImgs = new Texture2D[9];
        Rectangle[] btnRecs = new Rectangle[9];
        float btnScaler = 2f;

        //Font Images
        Texture2D titleImg;
        Texture2D highscoresImg;
        Texture2D pausedImg;
        Texture2D gameoverImg;
        Texture2D modesImg;
        Texture2D easyImg;
        Texture2D mediumImg;
        Texture2D hardImg;
        Rectangle titleRec;
        Rectangle highscoresRec;
        Rectangle pausedRec;
        Rectangle gameoverRec;
        Rectangle modesRec;
        Rectangle easyRec;
        Rectangle mediumRec;
        Rectangle hardRec;
        float titleScaler = 2f;
        float modesScaler = 0.5f;

        //Backgrounds
        Texture2D[] bgImgs = new Texture2D[6];
        Rectangle[] bgRecs = new Rectangle[6];

        ////Game Data////
        //Track the images that make up the game environment
        Texture2D groundPlatformImg;
        Texture2D defaultPlatformImg;
        Texture2D tinyPlatformImg;
        Texture2D seperatorPlatformImg;
        Texture2D spikeImg;
        Texture2D tpLeftImg;
        Texture2D tpRightImg;
        Texture2D buttonImg;
        Texture2D blankImg;  
        Texture2D portalImg;
        Texture2D gemImg;
        Texture2D healthBoostImg;
        Texture2D speedBoostImg;
        Texture2D damageBoostImg;
        Texture2D healthBarImg;
        Texture2D clockImg;

        //Store the rectangles of all platforms the player can collide with 
        Rectangle[] platformEasyRecs = new Rectangle[10];
        Rectangle[] platformMediumRecs = new Rectangle[7];
        Rectangle[] platformHardRecs = new Rectangle[10];

        //Store the rectangles of all objects that can be collided with
        Rectangle[] easyObjectRecs = new Rectangle[13];
        Rectangle[] mediumObjectRecs = new Rectangle[12];
        Rectangle[] HardObjectRecs = new Rectangle[15];

        //Create original rectangles for door to move up
        Rectangle[] actualEasyDoorRecs = new Rectangle[3];
        Rectangle actualMediumDoorRec;
        Rectangle[] actualHardDoorRecs = new Rectangle[2];

        //User interface
        Rectangle playerHealthBarRec;
        Rectangle actualPlayerHealthBarRec;
        Rectangle[] enemyHealthBarRec = new Rectangle[3];
        Rectangle[] actualEnemyHealthBarRec = new Rectangle[3];
        Rectangle[] gemUiRec = new Rectangle[3];
        Rectangle[] boostUiRec = new Rectangle[2];
        Rectangle clockRec;

        //Scalers for size of objects
        float platformRebound = -0.2f;
        float spikeScaler = 2.5f;
        float tpScaler = 0.25f;
        float buttonScaler = 0.25f;
        float portalScaler = 0.25f;
        float gemScaler = 0.05f;
        float healthBarScaler = 5f;
        float gemUiScaler = 0.1f;
        float boostUiScaler = 1.5f;
        float clockScaler = 1.5f;

        //Player Data
        Texture2D[] playerImgs = new Texture2D[4];
        Animation[] playerAnims = new Animation[4];
        Vector2 playerPos;
        int dir = POS;
        float playerHealthPercent = 0f;

        //Track the 6 inner rectangles of the player for collision detection
        Rectangle[] playerRecs = new Rectangle[6];

        //Specify whether the collision rectangles should be displayed (for testing purposes)
        bool showCollisionRecs = true;

        //Player movement data
        float maxSpeed = 4f;
        float maxBoostedSpeed = 6f;
        Vector2 playerSpeed = new Vector2(0f, 0f);

        //Player's bullet Data
        Texture2D bulletImg;
        Rectangle[] bulletRecs = new Rectangle[MAX_BULLETS];
        Vector2 inactivePos;
        float bulletMaxSpeed = 3f;

        //Enemy data
        Texture2D[] enemyEasyImgs = new Texture2D[3];
        Animation[] enemyEasyAnims = new Animation[3];
        Vector2 enemyEasyPos;
        Texture2D[] enemyMediumImgs = new Texture2D[3];
        Animation[] enemyMediumAnims = new Animation[3];
        Vector2 enemyMediumPos;
        Texture2D[] enemyHardImgs = new Texture2D[4];
        Animation[] enemyHardAnims = new Animation[4];
        Vector2 enemyHardPos;
        float enemySpeed = 1f;
        int enemyDir = POS;
        int leftBoundary = 630;
        int rightBoundary = 900;
        float[] enemyHealth = new float[3] { 100f, 100f, 100f };
        float enemyHealthPercent = 0f;

        //Enemy bullet data
        Texture2D enemyBulletImg;
        Rectangle[] enemyBulletRecs = new Rectangle[MAX_ENEMY_BULLETS];
        float enemyBulletMaxSpeed = 1.5f;

        //Track the forces working against the player every update
        Vector2 forces = new Vector2(FRICTION, GRAVITY);

        //The initial jump speed that will be reduced by gravity each update
        float jumpSpeed = -8f;

        //Track if player is on the ground or not
        bool grounded = false;

        //Spritefonts for menu and game play
        SpriteFont statsFont;
        SpriteFont mainFont;

        //Input States for Keyboard and Mouse
        KeyboardState prevKb;
        KeyboardState kb;
        MouseState prevMouse;
        MouseState mouse;

        //Track the current game mode
        bool[] modes = new bool[3] { false, false, false };

        //Track when buttons are pressed
        bool[] isButtonPressed = new bool[3] { false, false, false };

        //Track if all gems are collected
        bool isGemsCollected = false;

        //Track if the player clicked the mouse
        bool isClicked = false;

        //Track direction of bullet
        int[] bulletDir = new int[2] { POS, POS };

        //Timer for opening doors
        Timer doorTimer;

        //Timer for attack animation
        Timer attackTimer;

        //Timer for Enemy attack period
        Timer mediumAttackTimer;
        Timer hardAttackTimer;

        //Shooting cooldown for enemies
        const int MEDIUM_SHOOT_COOLDOWN = 3000;
        const int HARD_SHOOT_COOLDOWN = 2000;

        //Timer for decreasing health of player when in contact with enemy
        Timer enemyHealthTimer;

        //Timer for tracking amount of time taken to beat the level
        Timer gameTimer;

        //Track if power up has been picked up
        bool isDamageBoosted = false;
        bool isSpeedBoosted = false;

        //Timer for tracking amount of time power-up lasts
        Timer damageTimer;
        Timer speedTimer;
        const int POWERUP_TIME = 10000;

        //Track the total amount of time taken to finish game
        double timePassed = 0;
        double timeInSeconds = 0;

        //Track if the highscore has changed anbd store the highscore
        bool[] isHighscoresChanged = new bool[3] { false, false, false };
        double[] highscores = new double[3] { 0, 0, 0 };

        //Store Sound Effects
        SoundEffect jumpSnd;
        SoundEffect shootSnd;
        SoundEffect hurtSnd;
        SoundEffect swishSnd;
        SoundEffect collectSnd;
        SoundEffect buttonClickSnd;

        //Store songs
        Song menuMusic;
        Song gameMusic;
        Song gameoverMusic;

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
            IsMouseVisible = true;

            //Setup screen dimensions
            this.graphics.PreferredBackBufferHeight = 900;
            this.graphics.PreferredBackBufferWidth = 1598;

            //Apply the screen dimension changes
            this.graphics.ApplyChanges();

            //Store the screen dimensions
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
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

            //Load fonts
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            mainFont = Content.Load<SpriteFont>("Fonts/MainFont");

            //Font Images
            titleImg = Content.Load<Texture2D>("Images/FontImages/TitleImg");
            highscoresImg = Content.Load<Texture2D>("Images/FontImages/HighscoresImg");
            pausedImg = Content.Load<Texture2D>("Images/FontImages/PausedImg");
            gameoverImg = Content.Load<Texture2D>("Images/FontImages/GameOverImg");
            modesImg = Content.Load<Texture2D>("Images/FontImages/ModesImg");
            easyImg = Content.Load<Texture2D>("Images/FontImages/EasyImg");
            mediumImg = Content.Load<Texture2D>("Images/FontImages/MediumImg");
            hardImg = Content.Load<Texture2D>("Images/FontImages/HardImg");
            titleRec = new Rectangle(600, 100, (int)(titleImg.Width * titleScaler), (int)(titleImg.Height * titleScaler));
            highscoresRec = new Rectangle(600, 50, highscoresImg.Width, highscoresImg.Height);
            pausedRec = new Rectangle(550, 250, pausedImg.Width, pausedImg.Height);
            gameoverRec = new Rectangle(515, 200, gameoverImg.Width, gameoverImg.Height);
            modesRec = new Rectangle(550, 0, modesImg.Width, modesImg.Height);
            easyRec = new Rectangle(690, 150, (int)(easyImg.Width * modesScaler), (int)(easyImg.Height * modesScaler));
            mediumRec = new Rectangle(675, 325, (int)(mediumImg.Width * modesScaler), (int)(mediumImg.Height * modesScaler));
            hardRec = new Rectangle(690, 450, (int)(hardImg.Width * modesScaler), (int)(hardImg.Height * modesScaler));

            //Setup Menu Data
            bgImgs[MENUBG] = Content.Load<Texture2D>("Images/Backgrounds/Bg1");
            btnImgs[PLAYBTN] = Content.Load<Texture2D>("Images/Sprites/BtnPlay");
            btnImgs[INSTBTN] = Content.Load<Texture2D>("Images/Sprites/BtnInst");
            btnImgs[HIGHSCOREBTN] = Content.Load<Texture2D>("Images/Sprites/BtnHighscore");
            btnImgs[EXITBTN] = Content.Load<Texture2D>("Images/Sprites/BtnExit");
            btnImgs[MENUBTN] = Content.Load<Texture2D>("Images/Sprites/BtnMenu");
            bgRecs[MENUBG] = new Rectangle(0, 0, screenWidth, screenHeight);
            btnRecs[PLAYBTN] = new Rectangle(300, 200, (int)(btnImgs[PLAYBTN].Width * btnScaler), (int)(btnImgs[PLAYBTN].Height * btnScaler));
            btnRecs[INSTBTN] = new Rectangle(300, 350, (int)(btnImgs[INSTBTN].Width * btnScaler), (int)(btnImgs[INSTBTN].Height * btnScaler));
            btnRecs[HIGHSCOREBTN] = new Rectangle(300, 500, (int)(btnImgs[HIGHSCOREBTN].Width * btnScaler), (int)(btnImgs[HIGHSCOREBTN].Height * btnScaler));
            btnRecs[EXITBTN] = new Rectangle(300, 650, (int)(btnImgs[EXITBTN].Width * btnScaler), (int)(btnImgs[EXITBTN].Height * btnScaler));
            btnRecs[MENUBTN] = new Rectangle(screenWidth / 2 - btnImgs[MENUBTN].Width - 30, 700, (int)(btnImgs[MENUBTN].Width * btnScaler), (int)(btnImgs[MENUBTN].Height * btnScaler));

            //Setup Instructions, Highscores, and End Game Data
            bgImgs[HIGHSCOREBG] = Content.Load<Texture2D>("Images/Backgrounds/HighscoresBg");
            btnImgs[BACKBTN] = Content.Load<Texture2D>("Images/Sprites/BtnBack");
            bgImgs[ENDGAMEBG] = Content.Load<Texture2D>("Images/Backgrounds/End game");
            bgImgs[INSTRUCTIONSBG] = Content.Load<Texture2D>("Images/Backgrounds/INSTRUCTIONS");
            bgRecs[HIGHSCOREBG] = new Rectangle(399, 25, 800, 850);
            btnRecs[BACKBTN] = new Rectangle(0, 800, (int)(btnImgs[BACKBTN].Width * btnScaler), (int)(btnImgs[BACKBTN].Height * btnScaler));
            bgRecs[ENDGAMEBG] = new Rectangle(0, 0, screenWidth, screenHeight);
            bgRecs[INSTRUCTIONSBG] = new Rectangle(199, 50, bgImgs[INSTRUCTIONSBG].Width, bgImgs[INSTRUCTIONSBG].Height);

            //Setup Mode Selection screen Data
            bgImgs[MODESBG] = Content.Load<Texture2D>("Images/Backgrounds/Bg2");
            btnImgs[EASYBTN] = Content.Load<Texture2D>("Images/Sprites/BtnEasy");
            btnImgs[MEDIUMBTN] = Content.Load<Texture2D>("Images/Sprites/BtnMedium");
            btnImgs[HARDBTN] = Content.Load<Texture2D>("Images/Sprites/BtnHard");
            bgRecs[MODESBG] = new Rectangle(0, 0, screenWidth, screenHeight);
            btnRecs[EASYBTN] = new Rectangle(screenWidth / 2 - btnImgs[EASYBTN].Width, 300, (int)(btnImgs[EASYBTN].Width * btnScaler), (int)(btnImgs[EASYBTN].Height * btnScaler));
            btnRecs[MEDIUMBTN] = new Rectangle(screenWidth / 2 - btnImgs[MEDIUMBTN].Width, 400, (int)(btnImgs[MEDIUMBTN].Width * btnScaler), (int)(btnImgs[MEDIUMBTN].Height * btnScaler));
            btnRecs[HARDBTN] = new Rectangle(screenWidth / 2 - btnImgs[HARDBTN].Width, 500, (int)(btnImgs[HARDBTN].Width * btnScaler), (int)(btnImgs[HARDBTN].Height * btnScaler));

            //Setup Game Data
            bgImgs[GAMEBG] = Content.Load<Texture2D>("Images/Backgrounds/Bg3");
            bgRecs[GAMEBG] = new Rectangle(0, 0, screenWidth, screenHeight);
            groundPlatformImg = Content.Load<Texture2D>("Images/Sprites/Platform1");
            defaultPlatformImg = Content.Load<Texture2D>("Images/Sprites/Platform2");
            tinyPlatformImg = Content.Load<Texture2D>("Images/Sprites/Platform3");
            seperatorPlatformImg = Content.Load<Texture2D>("Images/Sprites/Platform4");
            spikeImg = Content.Load<Texture2D>("Images/Sprites/Spikes");
            tpLeftImg = Content.Load<Texture2D>("Images/Sprites/TpLeft");
            tpRightImg = Content.Load<Texture2D>("Images/Sprites/TpRight");
            buttonImg = Content.Load<Texture2D>("Images/Sprites/Button");
            blankImg = Content.Load<Texture2D>("Images/Sprites/Blank");
            portalImg = Content.Load<Texture2D>("Images/Sprites/Portal");
            gemImg = Content.Load<Texture2D>("Images/Sprites/Gem");
            bulletImg = Content.Load<Texture2D>("Images/Sprites/Bullet");
            healthBarImg = Content.Load<Texture2D>("Images/Sprites/HealthBar");
            enemyBulletImg = Content.Load<Texture2D>("Images/Sprites/Turret Bullet");
            healthBoostImg = Content.Load<Texture2D>("Images/Sprites/HealthPowerUp");
            speedBoostImg = Content.Load<Texture2D>("Images/Sprites/SpeedPowerUp");
            damageBoostImg = Content.Load<Texture2D>("Images/Sprites/DamagePowerUp");
            clockImg = Content.Load<Texture2D>("Images/Sprites/Clock");

            //Easy platform collidable rectangles
            platformEasyRecs[0] = new Rectangle(0, 800, screenWidth, 200);
            platformEasyRecs[1] = new Rectangle(0, 650, 500, 25);
            platformEasyRecs[2] = new Rectangle(600, 650, 250, 25);
            platformEasyRecs[3] = new Rectangle(1448, 650, 150, 25);
            platformEasyRecs[4] = new Rectangle(300, 475, 300, 25);
            platformEasyRecs[5] = new Rectangle(1000, 350, 450, 25);
            platformEasyRecs[6] = new Rectangle(500, 200, 450, 25);
            platformEasyRecs[7] = new Rectangle(800, 400, 100, 50);
            platformEasyRecs[8] = new Rectangle(665, 675, 50, 125);

            //Easy objects collidable rectangles
            easyObjectRecs[0] = new Rectangle(1100, 790, (int)(spikeImg.Width * spikeScaler), (int)(spikeImg.Height * spikeScaler));
            easyObjectRecs[1] = new Rectangle(1200, 340, (int)(spikeImg.Width * spikeScaler), (int)(spikeImg.Height * spikeScaler));
            easyObjectRecs[2] = new Rectangle(1534, 675, (int)(tpRightImg.Width * tpScaler), (int)(tpRightImg.Height * tpScaler));
            easyObjectRecs[3] = new Rectangle(0, 525, (int)(tpLeftImg.Width * tpScaler), (int)(tpLeftImg.Height * tpScaler));
            easyObjectRecs[4] = new Rectangle(350, 785, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
            easyObjectRecs[5] = new Rectangle(1375, 335, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
            easyObjectRecs[6] = new Rectangle(425, 675, 25, 125);
            easyObjectRecs[7] = new Rectangle(900, 75, 25, 125);
            easyObjectRecs[8] = new Rectangle(720, 738, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
            easyObjectRecs[9] = new Rectangle(550, 70, (int)(portalImg.Width * portalScaler), (int)(portalImg.Height * portalScaler));
            easyObjectRecs[10] = new Rectangle(1500, 610, healthBoostImg.Width, healthBoostImg.Height);
            easyObjectRecs[11] = new Rectangle(100, 610, speedBoostImg.Width, speedBoostImg.Height);
            easyObjectRecs[12] = new Rectangle(1400, 760, damageBoostImg.Width, damageBoostImg.Height);

            //Medium platform collidable rectangles
            platformMediumRecs[0] = new Rectangle(0, 800, screenWidth, 200);
            platformMediumRecs[1] = new Rectangle(1100, 625, 100, 50);
            platformMediumRecs[2] = new Rectangle(600, 500, 300, 25);
            platformMediumRecs[3] = new Rectangle(50, 250, 700, 25);
            platformMediumRecs[4] = new Rectangle(750, 325, 300, 25);
            platformMediumRecs[5] = new Rectangle(1100, 200, 200, 25);
            platformMediumRecs[6] = new Rectangle(50, 50, 500, 25);

            //Medium objects collidable rectangles
            mediumObjectRecs[0] = new Rectangle(675, 790, (int)(spikeImg.Width * spikeScaler), (int)(spikeImg.Height * spikeScaler));
            mediumObjectRecs[1] = new Rectangle(900, 315, (int)(spikeImg.Width * spikeScaler), (int)(spikeImg.Height * spikeScaler));
            mediumObjectRecs[2] = new Rectangle(575, 383, (int)(tpLeftImg.Width * tpScaler), (int)(tpLeftImg.Height * tpScaler));
            mediumObjectRecs[3] = new Rectangle(1236, 83, (int)(tpRightImg.Width * tpScaler), (int)(tpRightImg.Height * tpScaler));
            mediumObjectRecs[4] = new Rectangle(1300, 785, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
            mediumObjectRecs[5] = new Rectangle(725, 375, 25, 125);
            mediumObjectRecs[6] = new Rectangle(300, 216, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
            mediumObjectRecs[7] = new Rectangle(1100, 766, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
            mediumObjectRecs[8] = new Rectangle(100, 120, (int)(portalImg.Width * portalScaler), (int)(portalImg.Height * portalScaler));
            mediumObjectRecs[9] = new Rectangle(830, 285, healthBoostImg.Width, healthBoostImg.Height);
            mediumObjectRecs[10] = new Rectangle(1500, 760, speedBoostImg.Width, speedBoostImg.Height);
            mediumObjectRecs[11] = new Rectangle(980, 285, damageBoostImg.Width, damageBoostImg.Height);

            //Hard platform collidable rectangles
            platformHardRecs[0] = new Rectangle(0, 800, screenWidth, 200);
            platformHardRecs[1] = new Rectangle(200, 650, 200, 25);
            platformHardRecs[2] = new Rectangle(450, 550, 200, 25);
            platformHardRecs[3] = new Rectangle(800, 450, 400, 25);
            platformHardRecs[4] = new Rectangle(50, 425, 400, 25);
            platformHardRecs[5] = new Rectangle(1300, 300, 300, 25);
            platformHardRecs[6] = new Rectangle(400, 200, 600, 25);
            platformHardRecs[7] = new Rectangle(0, 135, 350, 25);
            platformHardRecs[8] = new Rectangle(400, 50, 600, 25);
            platformHardRecs[9] = new Rectangle(200, 675, 50, 125);

            //Hard objects collidable rectangles
            HardObjectRecs[0] = new Rectangle(800, 790, (int)(spikeImg.Width * spikeScaler), (int)(spikeImg.Height * spikeScaler));
            HardObjectRecs[1] = new Rectangle(1000, 440, (int)(spikeImg.Width * spikeScaler), (int)(spikeImg.Height * spikeScaler));
            HardObjectRecs[2] = new Rectangle(430, 83, (int)(tpLeftImg.Width * tpScaler), (int)(tpLeftImg.Height * tpScaler));
            HardObjectRecs[3] = new Rectangle(136, 18, (int)(tpRightImg.Width * tpScaler), (int)(tpRightImg.Height * tpScaler));
            HardObjectRecs[4] = new Rectangle(375, 410, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
            HardObjectRecs[5] = new Rectangle(275, 785, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
            HardObjectRecs[6] = new Rectangle(390, 675, 25, 125);
            HardObjectRecs[7] = new Rectangle(850, 325, 25, 125);
            HardObjectRecs[8] = new Rectangle(1250, 766, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
            HardObjectRecs[9] = new Rectangle(60, 391, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
            HardObjectRecs[10] = new Rectangle(550, 166, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
            HardObjectRecs[11] = new Rectangle(0, 5, (int)(portalImg.Width * portalScaler), (int)(portalImg.Height * portalScaler));
            HardObjectRecs[12] = new Rectangle(1150, 410, healthBoostImg.Width, healthBoostImg.Height);
            HardObjectRecs[13] = new Rectangle(275, 740, speedBoostImg.Width, speedBoostImg.Height);
            HardObjectRecs[14] = new Rectangle(1450, 260, damageBoostImg.Width, damageBoostImg.Height);

            //Original rectangles of the moveable doors
            actualEasyDoorRecs[0] = new Rectangle(425, 675, 25, 125);
            actualEasyDoorRecs[1] = new Rectangle(900, 75, 25, 125);
            actualMediumDoorRec = new Rectangle(725, 475, 25, 125);
            actualHardDoorRecs[0] = new Rectangle(360, 675, 25, 125);
            actualHardDoorRecs[1] = new Rectangle(850, 325, 25, 125);

            //Player Data
            playerImgs[IDLE] = Content.Load<Texture2D>("Images/Sprites/PlayerIdle");
            playerImgs[RUN] = Content.Load<Texture2D>("Images/Sprites/PlayerRun");
            playerImgs[JUMP] = Content.Load<Texture2D>("Images/Sprites/PlayerJump");
            playerImgs[ATTACK] = Content.Load<Texture2D>("Images/Sprites/PlayerAttack");
            playerPos = new Vector2(10, 730);
            playerAnims[RUN] = new Animation(playerImgs[RUN], 6, 1, 6, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    2, playerPos, 2f, false);
            playerAnims[IDLE] = new Animation(playerImgs[IDLE], 4, 1, 4, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    5, playerPos, 2f, true);
            playerAnims[JUMP] = new Animation(playerImgs[JUMP], 4, 1, 4, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    5, playerPos, 2f, false);
            playerAnims[ATTACK] = new Animation(playerImgs[ATTACK], 8, 1, 8, 1,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    10, playerPos, 2f, false);
            actualPlayerHealthBarRec = new Rectangle(20, 825, (int)(healthBarImg.Width * healthBarScaler), (int)(healthBarImg.Height * healthBarScaler));
            playerHealthBarRec = new Rectangle(20, 825, (int)(healthBarImg.Width * healthBarScaler), (int)(healthBarImg.Height * healthBarScaler));

            //Enemy Data
            //Easy
            enemyEasyImgs[ENEMY_IDLE] = Content.Load<Texture2D>("Images/Sprites/Enemy1Attack");
            enemyEasyPos = new Vector2(760, 722);
            enemyEasyAnims[ENEMY_IDLE] = new Animation(enemyEasyImgs[ENEMY_IDLE], 4, 1, 4, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    5, enemyEasyPos, 1f, true);

            //Medium
            enemyMediumImgs[ENEMY_IDLE] = Content.Load<Texture2D>("Images/Sprites/Enemy2Idle");
            enemyMediumImgs[ENEMY_ATTACK] = Content.Load<Texture2D>("Images/Sprites/Enemy2Attack");
            enemyMediumPos = new Vector2(400, 185);
            enemyMediumAnims[ENEMY_IDLE] = new Animation(enemyMediumImgs[ENEMY_IDLE], 4, 1, 4, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    5, enemyMediumPos, 0.85f, true);
            enemyMediumAnims[ENEMY_ATTACK] = new Animation(enemyMediumImgs[ENEMY_ATTACK], 6, 1, 6, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    5, enemyMediumPos, 0.85f, true);

            //Hard
            enemyHardImgs[ENEMY_WALK] = Content.Load<Texture2D>("Images/Sprites/Enemy3Walk");
            enemyHardImgs[ENEMY_ATTACK] = Content.Load<Texture2D>("Images/Sprites/Enemy3Attack");
            enemyHardPos = new Vector2(650, 110);
            enemyHardAnims[ENEMY_WALK] = new Animation(enemyHardImgs[ENEMY_WALK], 6, 1, 6, 0,
                                    Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                    10, enemyHardPos, 0.75f, true);
            enemyHardAnims[ENEMY_ATTACK] = new Animation(enemyHardImgs[ENEMY_ATTACK], 6, 1, 6, 0,
                                  Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                  10, enemyHardPos, 0.75f, true);
            
            //Enemy health bars
            actualEnemyHealthBarRec[0] = new Rectangle((int)enemyEasyPos.X, (int)(enemyEasyPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
            enemyHealthBarRec[0] = new Rectangle((int)enemyEasyPos.X, (int)(enemyEasyPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
            actualEnemyHealthBarRec[1] = new Rectangle((int)enemyMediumPos.X + 25, (int)(enemyMediumPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
            enemyHealthBarRec[1] = new Rectangle((int)enemyMediumPos.X + 25, (int)(enemyMediumPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
            actualEnemyHealthBarRec[2] = new Rectangle((int)enemyHardPos.X, (int)(enemyHardPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
            enemyHealthBarRec[2] = new Rectangle((int)enemyHardPos.X, (int)(enemyHardPos.Y - 20), healthBarImg.Width, healthBarImg.Height);

            //UI display
            gemUiRec[0] = new Rectangle(1528, 0, (int)(gemImg.Width * gemUiScaler), (int)(gemImg.Height * gemUiScaler));
            gemUiRec[1] = new Rectangle(1458, 0, (int)(gemImg.Width * gemUiScaler), (int)(gemImg.Height * gemUiScaler));
            gemUiRec[2] = new Rectangle(1388, 0, (int)(gemImg.Width * gemUiScaler), (int)(gemImg.Height * gemUiScaler));
            boostUiRec[0] = new Rectangle(1530, 80, (int)(speedBoostImg.Width * boostUiScaler), (int)(speedBoostImg.Height * boostUiScaler));
            boostUiRec[1] = new Rectangle(1530, 160, (int)(damageBoostImg.Width * boostUiScaler), (int)(damageBoostImg.Height * boostUiScaler));
            clockRec = new Rectangle(1200, 0, (int)(clockImg.Width * clockScaler), (int)(clockImg.Height * clockScaler));

            //Create and store the player's collision rectangles, as well as the visible testing rectangles
            SetPlayerRecs(playerRecs, playerAnims);

            //Setup inactive storage location
            inactivePos = new Vector2(-200, 0);

            //Setup all player bullet rectangles and store them off screen 
            for (int i = 0; i < bulletRecs.Length; i++)
            {
                bulletRecs[i] = new Rectangle((int)inactivePos.X, (int)inactivePos.Y, bulletImg.Width, bulletImg.Height);
            }

            //Setup all enemy bullet rectangles and store them off screen 
            for (int i = 0; i < enemyBulletRecs.Length; i++)
            {
                enemyBulletRecs[i] = new Rectangle((int)inactivePos.X, (int)inactivePos.Y, enemyBulletImg.Width, enemyBulletImg.Height);
            }
            
            //Set timers
            doorTimer = new Timer(5000, false);
            attackTimer = new Timer(950, false);
            mediumAttackTimer = new Timer(Timer.INFINITE_TIMER, false);
            hardAttackTimer = new Timer(Timer.INFINITE_TIMER, false);
            gameTimer = new Timer(Timer.INFINITE_TIMER, false);
            speedTimer = new Timer(POWERUP_TIME, false);
            damageTimer = new Timer(POWERUP_TIME, false);
            enemyHealthTimer = new Timer(HARD_SHOOT_COOLDOWN, false);

            //Load audio
            jumpSnd = Content.Load<SoundEffect>("Audio/Sound/Jump Effect");
            shootSnd = Content.Load<SoundEffect>("Audio/Sound/Shoot Effect");
            hurtSnd = Content.Load<SoundEffect>("Audio/Sound/Hurt Effect");
            swishSnd = Content.Load<SoundEffect>("Audio/Sound/Swoosh Effect");
            collectSnd = Content.Load<SoundEffect>("Audio/Sound/Collect effect");
            buttonClickSnd = Content.Load<SoundEffect>("Audio/Sound/Button Click");
            menuMusic = Content.Load<Song>("Audio/Music/Menu music");
            gameMusic = Content.Load<Song>("Audio/Music/Game music");
            gameoverMusic = Content.Load<Song>("Audio/Music/Game over music");

            //Set audio
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(menuMusic);
            SoundEffect.MasterVolume = 0.7f;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Update all of the input states
            prevKb = kb;
            kb = Keyboard.GetState();
            prevMouse = mouse;
            mouse = Mouse.GetState();

            //Update the current gamestate
            switch (gameState)
            {
                case MENU:
                    UpdateMenu();
                    break;
                case INSTRUCTIONS:
                    UpdateInstructions();
                    break;
                case HIGHSCORES:
                    UpdateHighscores();
                    break;
                case MODES:
                    UpdateModes();
                    break;
                case GAMEPLAY:
                    UpdateGame(gameTime);
                    break;
                case PAUSE:
                    UpdatePause();
                    break;
                case GAMEOVER:
                    UpdateGameover();
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            switch (gameState)
            {
                case MENU:
                    DrawMenu();
                    break;
                case INSTRUCTIONS:
                    DrawInstructions();
                    break;
                case HIGHSCORES:
                    DrawHighscores();
                    break;
                case MODES:
                    DrawModes();
                    break;
                case GAMEPLAY:
                    DrawGame();
                    break;
                case PAUSE:
                    DrawPause();
                    break;
                case GAMEOVER:
                    DrawGameOver();
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Update subprograms
        //Pre: None
        //Post: None
        //Desc: Handle input in the menu
        private void UpdateMenu()
        {
            //Change to the next screen state or exit if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change to the play or instruction or highscore screen or exit the game
                if (btnRecs[PLAYBTN].Contains(mouse.Position))
                {
                    //Change the screen to the mode selection screen
                    gameState = MODES;
                    buttonClickSnd.CreateInstance().Play();

                    //Reset the stats from the previous game
                    ResetGame();
                }
                else if (btnRecs[INSTBTN].Contains(mouse.Position))
                {
                    //Change the screen to the instruction screen
                    gameState = INSTRUCTIONS;
                    buttonClickSnd.CreateInstance().Play();
                }
                else if (btnRecs[HIGHSCOREBTN].Contains(mouse.Position))
                {
                    //Change the screen to the highscore screen
                    gameState = HIGHSCORES;
                    buttonClickSnd.CreateInstance().Play();
                }
                else if (btnRecs[EXITBTN].Contains(mouse.Position))
                {
                    //Exit the game
                    Exit();
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the instructions screen
        private void UpdateInstructions()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change to menu screen if back button is pressed 
                if (btnRecs[BACKBTN].Contains(mouse.Position))
                {
                    //Change the screen to the menu screen
                    gameState = MENU;
                    buttonClickSnd.CreateInstance().Play();
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the highscores screen
        private void UpdateHighscores()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change to menu screen if back button is pressed
                if (btnRecs[BACKBTN].Contains(mouse.Position))
                {
                    //Change the screen to the menu screen
                    gameState = MENU;
                    buttonClickSnd.CreateInstance().Play();
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the mode selections screen
        private void UpdateModes()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change the screen to the game or menu screen based on the button pressed
                if (btnRecs[EASYBTN].Contains(mouse.Position))
                {
                    //Change the screen to the game screen
                    gameState = GAMEPLAY;
                    buttonClickSnd.CreateInstance().Play();

                    //Start the game timer
                    gameTimer.ResetTimer(true);

                    //Enable easy mode
                    modes[EASY] = true;
                    modes[MEDIUM] = false;
                    modes[HARD] = false;

                    //Play game music
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameMusic);
                }
                else if (btnRecs[MEDIUMBTN].Contains(mouse.Position))
                {
                    //Change the screen to the game screen
                    gameState = GAMEPLAY;
                    buttonClickSnd.CreateInstance().Play();

                    //Start the game timer
                    gameTimer.ResetTimer(true);

                    //Enable medium mode
                    modes[MEDIUM] = true;
                    modes[EASY] = false;
                    modes[HARD] = false;

                    //Play game music
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameMusic);
                }
                else if (btnRecs[HARDBTN].Contains(mouse.Position))
                {
                    //Change the screen to the game screen
                    gameState = GAMEPLAY;
                    buttonClickSnd.CreateInstance().Play();

                    //Start the game timer
                    gameTimer.ResetTimer(true);

                    //Enable hard mode
                    modes[HARD] = true;
                    modes[EASY] = false;
                    modes[MEDIUM] = false;

                    //Play game music
                    MediaPlayer.Stop();
                    MediaPlayer.Play(gameMusic);
                }
                else if (btnRecs[BACKBTN].Contains(mouse.Position))
                {
                    //Change the screen to the menu screen
                    gameState = MENU;
                    buttonClickSnd.CreateInstance().Play();
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle all game related functionality
        private void UpdateGame(GameTime gameTime)
        {
            //Update animation states
            playerAnims[playerState].Update(gameTime);
            enemyEasyAnims[enemyEasyState].Update(gameTime);
            enemyMediumAnims[enemyMediumState].Update(gameTime);
            enemyHardAnims[enemyHardState].Update(gameTime);

            //Update timers
            doorTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            attackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            mediumAttackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            hardAttackTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            gameTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            speedTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            damageTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            enemyHealthTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Change to the pause screen if p key is pressed or change to gameover if player loses all health
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to pause screen
                gameState = PAUSE;

                //Pause music
                MediaPlayer.Pause();
            }
            else if (health <= 0)
            {
                gameState = GAMEOVER;
                gameTimer.IsPaused();

                //Play game over music
                MediaPlayer.Stop();
                MediaPlayer.Play(gameoverMusic);
            }

            //Trigger the player's attack if the player clicks
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed && isClicked == false)
            {
                //Player clicked mouse
                isClicked = true;

                //Reset the attack timer
                attackTimer.ResetTimer(true);
            }

            ////Update the player's speed based on input////
            //Check for right and left input to accelerate the player in the chosen direction
            if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
            {
                //Change player's direction, state, and animation
                dir = POS;
                playerState = RUN;
                playerAnims[RUN].isAnimating = true;

                //Add acceleration to the player's current speed, but keep it within the limits of maxSpeed
                playerSpeed.X += ACCEL;

                //Change the speed limits and minimums based on whether a power-up has been picked up or not
                if (isSpeedBoosted)
                {
                    //With speed boost
                    playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
                }
                else
                {
                    //Without speed boost
                    playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
                }
            }
            else if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.A))
            {
                //Change player's direction, state, and animation
                dir = NEG;
                playerState = RUN;
                playerAnims[RUN].isAnimating = true;

                //Subtract acceleration to the player's current speed, but keep it within the limits of maxSpeed
                playerSpeed.X -= ACCEL;

                //Change the speed limits and minimums based on whether a power-up has been picked up or not
                if (isSpeedBoosted)
                {
                    //With speed boost
                    playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxBoostedSpeed, maxBoostedSpeed);
                }
                else 
                {
                    //Without speed boost
                    playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -maxSpeed, maxSpeed);
                }
            }
            else
            {
                //Only apply friction if player is on the ground and no input is given
                if (grounded == true)
                {
                    //Decelerate if no input for horizontal movement
                    playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;

                    //If the player has decelerated below the tolerance amount, set the speed to 0
                    if (Math.Abs(playerSpeed.X) <= TOLERANCE)
                    {
                        //Change player state, animation, and speed
                        playerAnims[RUN].isAnimating = false;
                        playerState = IDLE;
                        playerSpeed.X = 0f;
                    }
                }
            }

            //Jump if the player hits up key or w key and is on the ground
            if ((kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.W)) && grounded == true)
            {
                //Play a jump sound 
                jumpSnd.CreateInstance().Play();

                //Apply jump speed
                playerSpeed.Y = jumpSpeed;

                //Change player state and animation
                playerState = JUMP;
                playerAnims[JUMP].isAnimating = true;
            }
            
            //Only allow attack animation if the attack timer is active and the player has clicked
            if (attackTimer.IsActive() && isClicked == true) 
            {
                //Change the player state and animation
                playerState = ATTACK;
                playerAnims[ATTACK].isAnimating = true;
            }

            //Stop the attack animation if the attack timer is finished
            if (attackTimer.IsFinished())
            {
                //Change animation
                playerAnims[ATTACK].isAnimating = false;

                //Reset for later use
                isClicked = false;
            }

            //Add gravity to the y component of the player's speed
            playerSpeed.Y += forces.Y;

            //Change the position of the player 
            playerPos.X += playerSpeed.X;
            playerPos.Y += playerSpeed.Y;
            playerAnims[playerState].destRec.X = (int)playerPos.X;
            playerAnims[playerState].destRec.Y = (int)playerPos.Y;

            //Remove the enemy and its health bar if it is defeated
            if (enemyHealth[EASY] <= 0)
            {
                enemyEasyAnims[enemyEasyState].destRec = Rectangle.Empty;
                enemyHealthBarRec[0] = Rectangle.Empty;
                actualEnemyHealthBarRec[0] = Rectangle.Empty;
            }
            else if (enemyHealth[MEDIUM] <= 0)
            {
                enemyMediumAnims[enemyMediumState].destRec = Rectangle.Empty;
                enemyHealthBarRec[1] = Rectangle.Empty;
                actualEnemyHealthBarRec[1] = Rectangle.Empty;
            }
            else if (enemyHealth[HARD] <= 0)
            {
                enemyHardAnims[enemyHardState].destRec = Rectangle.Empty;
                enemyHealthBarRec[2] = Rectangle.Empty;
                actualEnemyHealthBarRec[2] = Rectangle.Empty;
            }

            //Update the player's rectangles
            SetPlayerRecs(playerRecs, playerAnims);

            //Detect if the player hits the wall
            PlayerWallCollision();

            //Detect if the player is colliding with the platforms
            PlatformCollision();

            //Detect if the player hits an object
            ObjectCollision();

            //Detect if the player is within an enemy's radius
            PlayerDetection();

            //Make the enemy patrol 
            EnemyPatrol();

            //Handle the player's bullets
            PlayerBullets();

            //Handle the Enemy's bullets
            EnemyBullets();

            //Adjust the player and enemy health bars according to their health
            ModifyHealthBar();

            //Count how many gems are collected
            ModifyGems();
        }

        //Pre: None
        //Post: None
        //Desc: Handle input on the pause screen 
        private void UpdatePause()
        {
            //Change to the play screen if p key is pressed
            if (kb.IsKeyDown(Keys.P) && !prevKb.IsKeyDown(Keys.P))
            {
                //Change to play screen
                gameState = GAMEPLAY;

                //Resume music
                MediaPlayer.Resume();
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle input in the game over screen
        private void UpdateGameover()
        {
            //Change to another screen if a button is pressed
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Change to menu screen if menu button is pressed 
                if (btnRecs[MENUBTN].Contains(mouse.Position))
                {
                    //Change to menu screen
                    gameState = MENU;
                    buttonClickSnd.CreateInstance().Play();

                    //Play menu music
                    MediaPlayer.Stop();
                    MediaPlayer.Play(menuMusic);
                }
            }
        }
        #endregion

        #region Draw subprograms
        //Pre: None
        //Post: None
        //Desc: Draw the menu interface
        private void DrawMenu()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MENU], bgRecs[MENU], Color.White);

            //Display buttons
            spriteBatch.Draw(btnImgs[PLAYBTN], btnRecs[PLAYBTN], Color.White);
            spriteBatch.Draw(btnImgs[INSTBTN], btnRecs[INSTBTN], Color.White);
            spriteBatch.Draw(btnImgs[HIGHSCOREBTN], btnRecs[HIGHSCOREBTN], Color.White);
            spriteBatch.Draw(btnImgs[EXITBTN], btnRecs[EXITBTN], Color.White);

            //Display title
            spriteBatch.Draw(titleImg, titleRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the game instructions and back button
        private void DrawInstructions()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MENUBG], bgRecs[MENUBG], Color.White);

            //Display font
            spriteBatch.Draw(bgImgs[INSTRUCTIONSBG], bgRecs[INSTRUCTIONSBG], Color.White);

            //Display button
            spriteBatch.Draw(btnImgs[BACKBTN], btnRecs[BACKBTN], Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the highscores and back button
        private void DrawHighscores()
        {
            //Display backgrounds
            spriteBatch.Draw(bgImgs[MENUBG], bgRecs[MENUBG], Color.White);
            spriteBatch.Draw(bgImgs[HIGHSCOREBG], bgRecs[HIGHSCOREBG], Color.Gray);

            //Display font
            spriteBatch.Draw(highscoresImg, highscoresRec, Color.White);
            spriteBatch.Draw(easyImg, easyRec, Color.White);
            spriteBatch.Draw(mediumImg, mediumRec, Color.White);
            spriteBatch.Draw(hardImg, hardRec, Color.White);

            //Display button
            spriteBatch.Draw(btnImgs[BACKBTN], btnRecs[BACKBTN], Color.White);

            //Display highscore data
            spriteBatch.DrawString(mainFont, Convert.ToString(Math.Round(highscores[EASY], 2)), new Vector2(775, 280), Color.DarkGray);
            spriteBatch.DrawString(mainFont, Convert.ToString(Math.Round(highscores[EASY], 2)), new Vector2(777, 282), Color.White);
            spriteBatch.DrawString(mainFont, Convert.ToString(Math.Round(highscores[MEDIUM], 2)), new Vector2(775, 430), Color.DarkGray);
            spriteBatch.DrawString(mainFont, Convert.ToString(Math.Round(highscores[MEDIUM], 2)), new Vector2(777, 432), Color.White);
            spriteBatch.DrawString(mainFont, Convert.ToString(Math.Round(highscores[HARD], 2)), new Vector2(775, 625), Color.DarkGray);
            spriteBatch.DrawString(mainFont, Convert.ToString(Math.Round(highscores[HARD], 2)), new Vector2(777, 627), Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the buttons for selection of game mode
        private void DrawModes()
        {
            //Display background
            spriteBatch.Draw(bgImgs[MODESBG], bgRecs[MODESBG], Color.White);

            //Display font
            spriteBatch.Draw(modesImg, modesRec, Color.White);

            //Display buttons
            spriteBatch.Draw(btnImgs[BACKBTN], btnRecs[BACKBTN], Color.White);
            spriteBatch.Draw(btnImgs[EASYBTN], btnRecs[EASYBTN], Color.White);
            spriteBatch.Draw(btnImgs[MEDIUMBTN], btnRecs[MEDIUMBTN], Color.White);
            spriteBatch.Draw(btnImgs[HARDBTN], btnRecs[HARDBTN], Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw all game elements
        private void DrawGame()
        {
            //Display background
            spriteBatch.Draw(bgImgs[GAMEBG], bgRecs[GAMEBG], Color.White);

            //Display the environment based on the mode
            if (modes[EASY] == true)
            {
                //Display the platforms
                spriteBatch.Draw(groundPlatformImg, platformEasyRecs[0], Color.White);
                for (int i = 1; i < 7; i++)
                {
                    spriteBatch.Draw(defaultPlatformImg, platformEasyRecs[i], Color.White);
                }
                spriteBatch.Draw(tinyPlatformImg, platformEasyRecs[7], Color.White);
                spriteBatch.Draw(seperatorPlatformImg, platformEasyRecs[8], Color.White);

                //Display in-game objects
                spriteBatch.Draw(spikeImg, easyObjectRecs[0], Color.White);
                spriteBatch.Draw(spikeImg, easyObjectRecs[1], Color.White);
                spriteBatch.Draw(tpRightImg, easyObjectRecs[2], Color.White);
                spriteBatch.Draw(tpLeftImg, easyObjectRecs[3], Color.White);
                spriteBatch.Draw(buttonImg, easyObjectRecs[4], Color.Red);
                spriteBatch.Draw(buttonImg, easyObjectRecs[5], Color.Green);
                spriteBatch.Draw(blankImg, easyObjectRecs[6], Color.Red);
                spriteBatch.Draw(blankImg, easyObjectRecs[7], Color.Green);
                spriteBatch.Draw(gemImg, easyObjectRecs[8], Color.White);
                spriteBatch.Draw(portalImg, easyObjectRecs[9], Color.White);
                spriteBatch.Draw(healthBoostImg, easyObjectRecs[10], Color.White);
                spriteBatch.Draw(speedBoostImg, easyObjectRecs[11], Color.White);
                spriteBatch.Draw(damageBoostImg, easyObjectRecs[12], Color.White);

                //Display enemy
                enemyEasyAnims[enemyEasyState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);

                //Display enemy health bar
                spriteBatch.Draw(blankImg, enemyHealthBarRec[0], Color.Lerp(Color.Red, Color.Green, enemyHealthPercent));
                spriteBatch.Draw(healthBarImg, actualEnemyHealthBarRec[0], Color.White);

                //Display collected gems
                if (isGemsCollected)
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White);
                }
                else
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White * 0.5f);
                }
            }
            else if (modes[MEDIUM] == true)
            {
                //Display the platforms
                spriteBatch.Draw(groundPlatformImg, platformMediumRecs[0], Color.White);
                spriteBatch.Draw(tinyPlatformImg, platformMediumRecs[1], Color.White);
                for (int i = 2; i < 7; i++)
                {
                    spriteBatch.Draw(defaultPlatformImg, platformMediumRecs[i], Color.White);
                }

                //Display in-game objects
                spriteBatch.Draw(spikeImg, mediumObjectRecs[0], Color.White);
                spriteBatch.Draw(spikeImg, mediumObjectRecs[1], Color.White);
                spriteBatch.Draw(tpLeftImg, mediumObjectRecs[2], Color.White);
                spriteBatch.Draw(tpRightImg, mediumObjectRecs[3], Color.White);
                spriteBatch.Draw(buttonImg, mediumObjectRecs[4], Color.White);
                spriteBatch.Draw(blankImg, mediumObjectRecs[5], Color.Red);
                spriteBatch.Draw(gemImg, mediumObjectRecs[6], Color.White);
                spriteBatch.Draw(gemImg, mediumObjectRecs[7], Color.White);
                spriteBatch.Draw(portalImg, mediumObjectRecs[8], Color.White);
                spriteBatch.Draw(healthBoostImg, mediumObjectRecs[9], Color.White);
                spriteBatch.Draw(speedBoostImg, mediumObjectRecs[10], Color.White);
                spriteBatch.Draw(damageBoostImg, mediumObjectRecs[11], Color.White);

                //Display enemy
                enemyMediumAnims[enemyMediumState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);

                //Display enemy health bar
                spriteBatch.Draw(blankImg, enemyHealthBarRec[1], Color.Lerp(Color.Red, Color.Green, enemyHealthPercent));
                spriteBatch.Draw(healthBarImg, actualEnemyHealthBarRec[1], Color.White);

                //Display collected gems
                if (isGemsCollected)
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White);
                }
                else if (gems == 1)
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White * 0.5f);
                }
                else
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White * 0.5f);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White * 0.5f);
                }
            }
            else if (modes[HARD] == true)
            {
                //Display the platforms
                spriteBatch.Draw(groundPlatformImg, platformHardRecs[0], Color.White);
                for (int i = 1; i < 9; i++)
                {
                    spriteBatch.Draw(defaultPlatformImg, platformHardRecs[i], Color.White);
                }
                spriteBatch.Draw(seperatorPlatformImg, platformHardRecs[9], Color.White);

                //Display in-game objects
                for (int i = 0; i < 2; i++)
                {
                    spriteBatch.Draw(spikeImg, HardObjectRecs[i], Color.White);
                }
                spriteBatch.Draw(tpLeftImg, HardObjectRecs[2], Color.White);
                spriteBatch.Draw(tpRightImg, HardObjectRecs[3], Color.White);
                spriteBatch.Draw(buttonImg, HardObjectRecs[4], Color.White);
                spriteBatch.Draw(buttonImg, HardObjectRecs[5], Color.Blue);
                spriteBatch.Draw(blankImg, HardObjectRecs[6], Color.Red);
                spriteBatch.Draw(blankImg, HardObjectRecs[7], Color.Blue);
                for (int i = 8; i < 11; i++)
                {
                    spriteBatch.Draw(gemImg, HardObjectRecs[i], Color.White);
                }
                spriteBatch.Draw(portalImg, HardObjectRecs[11], Color.White);
                spriteBatch.Draw(healthBoostImg, HardObjectRecs[12], Color.White);
                spriteBatch.Draw(speedBoostImg, HardObjectRecs[13], Color.White);
                spriteBatch.Draw(damageBoostImg, HardObjectRecs[14], Color.White);


                //Display enemy and adjust its appearance based on the direction it is facing
                if (enemyDir == POS)
                {
                    //Do not flip animation when enemy is facing right
                    enemyHardAnims[enemyHardState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
                }
                else if (enemyDir == NEG)
                {
                    //Flip animation when enemy is facing left
                    enemyHardAnims[enemyHardState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
                }

                //Display enemy health bar
                spriteBatch.Draw(blankImg, enemyHealthBarRec[2], Color.Lerp(Color.Red, Color.Green, enemyHealthPercent));
                spriteBatch.Draw(healthBarImg, actualEnemyHealthBarRec[2], Color.White);

                //Display collected gems
                if (isGemsCollected)
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[2], Color.White);

                }
                else if (gems == 1)
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White * 0.5f);
                    spriteBatch.Draw(gemImg, gemUiRec[2], Color.White * 0.5f);
                }
                else if (gems == 2)
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White);
                    spriteBatch.Draw(gemImg, gemUiRec[2], Color.White * 0.5f);
                }
                else
                {
                    spriteBatch.Draw(gemImg, gemUiRec[0], Color.White * 0.5f);
                    spriteBatch.Draw(gemImg, gemUiRec[1], Color.White * 0.5f);
                    spriteBatch.Draw(gemImg, gemUiRec[2], Color.White * 0.5f);
                }
            }

            //Draw all active player bullets
            for (int i = 0; i < bulletRecs.Length; i++)
            {
                //A bullet is active if its X or Y components are different than the initial inactive storage location
                if (bulletRecs[i].X != inactivePos.X || bulletRecs[i].Y != inactivePos.Y)
                {
                    //Draw the current bullet
                    spriteBatch.Draw(bulletImg, bulletRecs[i], Color.White);
                }
            }

            //Draw all active enemy bullets
            for (int i = 0; i < enemyBulletRecs.Length; i++)
            {
                //A bullet is active if its X or Y components are different than the initial inactive storage location
                if (enemyBulletRecs[i].X != inactivePos.X || enemyBulletRecs[i].Y != inactivePos.Y)
                {
                    //Draw the current enemy bullet
                    spriteBatch.Draw(enemyBulletImg, enemyBulletRecs[i], Color.White);
                }
            }

            //Display the player based on the direction it is facing
            if (dir == POS)
            {
                //If the player is facing right, do not flip the animation
                playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
            else if (dir == NEG)
            {
                //If the player is facing left, flip the animation
                playerAnims[playerState].Draw(spriteBatch, Color.White, Animation.FLIP_HORIZONTAL);
            }

            //Display the player's health bar
            spriteBatch.Draw(blankImg, playerHealthBarRec, Color.Lerp(Color.Red, Color.Green, playerHealthPercent));
            spriteBatch.Draw(healthBarImg, actualPlayerHealthBarRec, Color.White);

            //Display the game information
            spriteBatch.DrawString(statsFont, gameTimer.GetTimePassedAsString(Timer.FORMAT_MIN_SEC_MIL), new Vector2(1275, 18), Color.White);
            spriteBatch.DrawString(mainFont, "PRESS P TO PAUSE", new Vector2(900, 0), Color.White);

            //Display when speed is boosted
            if (isSpeedBoosted)
            {
                spriteBatch.Draw(speedBoostImg, boostUiRec[0], Color.White);
                spriteBatch.DrawString(statsFont, speedTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(1530, 80), Color.White);
            }
            else if (!isSpeedBoosted)
            {
                spriteBatch.Draw(speedBoostImg, boostUiRec[0], Color.Gray);
            }

            //Display when damage is boosted
            if (isDamageBoosted)
            {
                spriteBatch.Draw(damageBoostImg, boostUiRec[1], Color.White);
                spriteBatch.DrawString(statsFont, damageTimer.GetTimeRemainingAsString(Timer.FORMAT_SEC_MIL), new Vector2(1530, 160), Color.White);
            }
            else if (!isDamageBoosted)
            {
                spriteBatch.Draw(damageBoostImg, boostUiRec[1], Color.Gray);
            }

            //Display clock 
            spriteBatch.Draw(clockImg, clockRec, Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the pause screen and menu interface
        private void DrawPause()
        {
            //Display background
            spriteBatch.Draw(bgImgs[GAMEBG], bgRecs[GAMEBG], Color.White);

            //Display title
            spriteBatch.Draw(pausedImg, pausedRec, Color.White);

            //Let player know that game is paused and how to unpause
            spriteBatch.DrawString(mainFont, "PRESS P TO UNPAUSE", new Vector2(630, 475), Color.White);
        }

        //Pre: None
        //Post: None
        //Desc: Draw the Game Over screen and menu interface
        private void DrawGameOver()
        {
            //Display background
            spriteBatch.Draw(bgImgs[ENDGAMEBG], bgRecs[ENDGAMEBG], Color.White);

            //Display title
            spriteBatch.Draw(gameoverImg, gameoverRec, Color.White);

            //Display button
            spriteBatch.Draw(btnImgs[MENUBTN], btnRecs[MENUBTN], Color.White);

            if (health <= 0)
            {
                spriteBatch.DrawString(mainFont, "FAILED", new Vector2(725, 100), Color.DarkRed);
            }
            else
            {
                //Display total amount of time taken for player to beat level
                spriteBatch.DrawString(mainFont, "Time:" + Math.Round(timeInSeconds, 2), new Vector2(690, 100), Color.White);
            }
        }
        #endregion

        //Pre: playerRecs holds the player's individual collision rectangles (for individual body parts), playerRec holds the player's current location
        //Post: None
        //Description: Calculate the player's collision rectangles based on its current location and size
        private void SetPlayerRecs(Rectangle[] playerRecs, Animation[] playerAnims)
        {
            //Define player Collision Recs based on its position and scaled size
            playerRecs[HEAD] = new Rectangle(playerAnims[playerState].destRec.X + (int)(0.25f * playerAnims[playerState].destRec.Width), playerAnims[playerState].destRec.Y,
                                             (int)(playerAnims[playerState].destRec.Width * 0.5f), (int)(playerAnims[playerState].destRec.Height * 0.25f));
            playerRecs[LEFT] = new Rectangle(playerAnims[playerState].destRec.X + 5, playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                             (int)(playerAnims[playerState].destRec.Width * 0.4f), (int)(playerAnims[playerState].destRec.Height * 0.5f));
            playerRecs[RIGHT] = new Rectangle(playerRecs[LEFT].X + playerRecs[LEFT].Width, playerRecs[HEAD].Y + playerRecs[HEAD].Height,
                                             (int)(playerAnims[playerState].destRec.Width * 0.4f), (int)(playerAnims[playerState].destRec.Height * 0.5f));
            playerRecs[FEET] = new Rectangle(playerAnims[playerState].destRec.X + (int)(0.3f * playerAnims[playerState].destRec.Width), playerRecs[LEFT].Y + playerRecs[LEFT].Height,
                                             (int)(playerAnims[playerState].destRec.Width * 0.4f), (int)(playerAnims[playerState].destRec.Height * 0.25f));
            playerRecs[LEFT_TOE] = new Rectangle(playerAnims[playerState].destRec.X + (int)(0.2f * playerAnims[playerState].destRec.Width), playerRecs[FEET].Y,
                                             (int)(playerAnims[playerState].destRec.Width * 0.3f), (int)(playerAnims[playerState].destRec.Height * 0.25f));
            playerRecs[RIGHT_TOE] = new Rectangle(playerAnims[playerState].destRec.X + (int)(0.2f * playerAnims[playerState].destRec.Width) + playerRecs[LEFT_TOE].Width, playerRecs[FEET].Y,
                                             (int)(playerAnims[playerState].destRec.Width * 0.3f), (int)(playerAnims[playerState].destRec.Height * 0.25f));
        }

        //Pre: None
        //Post: None
        //Desc: Detect wall collision with the player and stop their movement to keep them on screen
        private void PlayerWallCollision()
        {
            //Player is not initially colliding with a wall
            bool collision = false;

            //If the player hits the side walls, pull them in bounds and stop their horizontal movement
            if (playerAnims[playerState].destRec.X < 0)
            {
                //Player past left side of screen, realign to be exactly the left side and stop movement
                playerAnims[playerState].destRec.X = 0;
                playerPos.X = playerAnims[playerState].destRec.X;
                playerSpeed.X = 0;
                collision = true;
            }
            else if (playerAnims[playerState].destRec.Right > screenWidth)
            {
                //Player past right side of screen, realign to be exactly the right side and stop movement
                playerAnims[playerState].destRec.X = screenWidth - playerAnims[playerState].destRec.Width;
                playerPos.X = playerAnims[playerState].destRec.X;
                playerSpeed.X = 0;
                collision = true;
            }

            //If the player hits the top wall, pull them in bounds and stop their vertical movement 
            if (playerAnims[playerState].destRec.Y < 0)
            {
                //Player past top side of screen, realign to be exactly the top side and stop movement
                playerAnims[playerState].destRec.Y = 0;
                playerPos.Y = playerAnims[playerState].destRec.Y;
                playerSpeed.Y = 0;
                collision = true;
            }
            else
            {
                grounded = false;
            }

            //If a collision occured then the player's collision rectangles need to be adjusted
            if (collision == true)
            {
                //Adjust the player's collision rectangles
                SetPlayerRecs(playerRecs, playerAnims);
            }
        }

        //Pre: None
        //Post: None
        //Desc: Tests the player against every platform for collision and adjust player if collided
        private void PlatformCollision()
        {
            //Player is not initially colliding with a platform
            bool collision = false;

            //Test collision according to the level's environment
            if (modes[EASY] == true)
            {
                //Test collision between the player and every platform
                for (int i = 0; i < platformEasyRecs.Length; i++)
                {
                    //Check for a bounding box collision first
                    if (playerAnims[playerState].destRec.Intersects(platformEasyRecs[i]))
                    {
                        //Shift the player to just outside of the collision location depending on body part
                        if (playerRecs[FEET].Intersects(platformEasyRecs[i]))
                        {
                            playerAnims[playerState].destRec.Y = platformEasyRecs[i].Y - playerAnims[playerState].destRec.Height;
                            playerPos.Y = playerAnims[playerState].destRec.Y;
                            playerSpeed.Y = 0f;
                            grounded = true;
                            collision = true;
                        }
                        else if (playerRecs[LEFT].Intersects(platformEasyRecs[i]))
                        {
                            playerAnims[playerState].destRec.X = platformEasyRecs[i].X + platformEasyRecs[i].Width + 1;
                            playerPos.X = playerAnims[playerState].destRec.X;
                            playerSpeed.X *= platformRebound;
                            collision = true;
                        }
                        else if (playerRecs[RIGHT].Intersects(platformEasyRecs[i]))
                        {
                            playerAnims[playerState].destRec.X = platformEasyRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                            playerPos.X = playerAnims[playerState].destRec.X;
                            playerSpeed.X = platformRebound;
                            collision = true;
                        }
                        else if (playerRecs[HEAD].Intersects(platformEasyRecs[i]))
                        {
                            playerAnims[playerState].destRec.Y = platformEasyRecs[i].Y + platformEasyRecs[i].Height + 1;
                            playerPos.Y = playerAnims[playerState].destRec.Y;
                            playerSpeed.Y *= NEG;
                            collision = true;
                        }

                        //If a collision occured then the player's collision rectangles need to be adjusted
                        if (collision == true)
                        {
                            SetPlayerRecs(playerRecs, playerAnims);
                            collision = false;
                        }
                    }
                }
            }
            else if (modes[MEDIUM] == true)
            {
                //Test collision between the player and every platform
                for (int i = 0; i < platformMediumRecs.Length; i++)
                {
                    //Check for a bounding box collision first
                    if (playerAnims[playerState].destRec.Intersects(platformMediumRecs[i]))
                    {
                        //Shift the player to just outside of the collision location depending on body part
                        if (playerRecs[FEET].Intersects(platformMediumRecs[i]))
                        {
                            playerAnims[playerState].destRec.Y = platformMediumRecs[i].Y - playerAnims[playerState].destRec.Height;
                            playerPos.Y = playerAnims[playerState].destRec.Y;
                            playerSpeed.Y = 0f;
                            grounded = true;
                            collision = true;
                        }
                        else if (playerRecs[LEFT].Intersects(platformMediumRecs[i]))
                        {
                            playerAnims[playerState].destRec.X = platformMediumRecs[i].X + platformMediumRecs[i].Width + 1;
                            playerPos.X = playerAnims[playerState].destRec.X;
                            playerSpeed.X = platformRebound;
                            collision = true;
                        }
                        else if (playerRecs[RIGHT].Intersects(platformMediumRecs[i]))
                        {
                            playerAnims[playerState].destRec.X = platformMediumRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                            playerPos.X = playerAnims[playerState].destRec.X;
                            playerSpeed.X = platformRebound;
                            collision = true;
                        }
                        else if (playerRecs[HEAD].Intersects(platformMediumRecs[i]))
                        {
                            playerAnims[playerState].destRec.Y = platformMediumRecs[i].Y + platformMediumRecs[i].Height + 1;
                            playerPos.Y = playerAnims[playerState].destRec.Y;
                            playerSpeed.Y *= NEG;
                            collision = true;
                        }

                        //If a collision occured then the player's collision rectangles need to be adjusted
                        if (collision == true)
                        {
                            SetPlayerRecs(playerRecs, playerAnims);
                            collision = false;
                        }
                    }
                }
            }
            else if (modes[HARD] == true)
            {
                //Test collision between the player and every platform
                for (int i = 0; i < platformHardRecs.Length; i++)
                {
                    //Check for a bounding box collision first
                    if (playerAnims[playerState].destRec.Intersects(platformHardRecs[i]))
                    {
                        //Shift the player to just outside of the collision location depending on body part
                        if (playerRecs[FEET].Intersects(platformHardRecs[i]))
                        {
                            playerAnims[playerState].destRec.Y = platformHardRecs[i].Y - playerAnims[playerState].destRec.Height;
                            playerPos.Y = playerAnims[playerState].destRec.Y;
                            playerSpeed.Y = 0f;
                            grounded = true;
                            collision = true;
                        }
                        else if (playerRecs[LEFT].Intersects(platformHardRecs[i]))
                        {
                            playerAnims[playerState].destRec.X = platformHardRecs[i].X + platformHardRecs[i].Width + 1;
                            playerPos.X = playerAnims[playerState].destRec.X;
                            playerSpeed.X = platformRebound;
                            collision = true;
                        }
                        else if (playerRecs[RIGHT].Intersects(platformHardRecs[i]))
                        {
                            playerAnims[playerState].destRec.X = platformHardRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                            playerPos.X = playerAnims[playerState].destRec.X;
                            playerSpeed.X = platformRebound;
                            collision = true;
                        }
                        else if (playerRecs[HEAD].Intersects(platformHardRecs[i]))
                        {
                            playerAnims[playerState].destRec.Y = platformHardRecs[i].Y + platformHardRecs[i].Height + 1;
                            playerPos.Y = playerAnims[playerState].destRec.Y;
                            playerSpeed.Y *= NEG;
                            collision = true;
                        }

                        //If a collision occured then the player's collision rectangles need to be adjusted
                        if (collision == true)
                        {
                            SetPlayerRecs(playerRecs, playerAnims);
                            collision = false;
                        }
                    }
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Tests the player against every object for collision and adjust player or change stats accordingly
        private void ObjectCollision()
        {
            //Player is not initially colliding with an object
            bool collision = false;

            //Detect collision based on the level's environment
            if (modes[EASY] == true)
            {
                //Detect collision for spikes
                for (int i = 0; i < 2; i++)
                {
                    if (playerRecs[FEET].Intersects(easyObjectRecs[i]))
                    {
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        collision = true;
                    }
                    else if (playerRecs[RIGHT_TOE].Intersects(easyObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = easyObjectRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }
                    else if (playerRecs[LEFT_TOE].Intersects(easyObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = easyObjectRecs[i].X + easyObjectRecs[i].Width + 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }
                }

                //Detect collision for teleporters and adjust position based on teleporter entered
                if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[2]))
                {
                    swishSnd.CreateInstance().Play();
                    playerAnims[playerState].destRec.X = easyObjectRecs[3].Right + 1;
                    playerAnims[playerState].destRec.Y = platformEasyRecs[1].Y - playerAnims[playerState].destRec.Height;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerPos.Y = playerAnims[playerState].destRec.Y;
                    collision = true;
                }
                else if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[3]))
                {
                    swishSnd.CreateInstance().Play();
                    playerAnims[playerState].destRec.X = easyObjectRecs[2].X - playerAnims[playerState].destRec.Width - 1;
                    playerAnims[playerState].destRec.Y = platformEasyRecs[0].Y - playerAnims[playerState].destRec.Height;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerPos.Y = playerAnims[playerState].destRec.Y;
                    collision = true;
                }

                //Detect collision of buttons
                if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[4]))
                {
                    //First button
                    isButtonPressed[0] = true;
                    easyObjectRecs[4] = Rectangle.Empty;
                    doorTimer.ResetTimer(true);
                }
                else if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[5]))
                {
                    //Second button
                    isButtonPressed[1] = true;
                    easyObjectRecs[5] = Rectangle.Empty;
                    doorTimer.ResetTimer(true);
                }

                //Detect collision of doors
                for (int i = 6; i < 8; i++)
                {
                    if (playerRecs[RIGHT].Intersects(easyObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = easyObjectRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        playerSpeed.X *= NEG;
                        collision = true;
                    }
                    else if (playerRecs[LEFT].Intersects(easyObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = easyObjectRecs[i].X + easyObjectRecs[3].Width + 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        playerSpeed.X *= NEG;
                        collision = true;
                    }
                }

                //If the player collides with a gem, it disappears and is added to the score
                if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[8]))
                {
                    //Increment gem
                    gems++;
                    easyObjectRecs[8] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }

                //If the escape portal is collided with and all the required gems are collected, then change the screen to gameover
                if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[9]))
                {
                    //If all the gems are collected then change screen to gameover
                    if (isGemsCollected == true)
                    {
                        //Change screen to gameover
                        gameState = GAMEOVER;

                        //Store the amount of time taken to pass level
                        timePassed = gameTimer.GetTimePassed();
                        gameTimer.IsPaused();

                        //Check for a new highscore
                        CheckHighscore();

                        //Play game over music
                        MediaPlayer.Stop();
                        MediaPlayer.Play(gameoverMusic);
                    }
                }

                //If the player collides with a power-up, apply the power-up to the player
                if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[10]))
                {
                    //Increase health
                    health += 20;

                    //If the health is over the max amount of health, then set it to the max health
                    if (health >= MAX_HEALTH)
                    {
                        health = MAX_HEALTH;
                    }

                    easyObjectRecs[10] = Rectangle.Empty;
                }
                else if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[11]))
                {
                    //Speed is boosted and cooldown timer for it begins
                    isSpeedBoosted = true;
                    speedTimer.ResetTimer(true);
                    easyObjectRecs[11] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }
                else if (playerAnims[playerState].destRec.Intersects(easyObjectRecs[12]))
                {
                    //Damage is boosted and cooldown timer for it begins
                    isDamageBoosted = true;
                    damageTimer.ResetTimer(true);
                    easyObjectRecs[12] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }

                //If the player hits the enemy, it cannot go past it and will bounce back while reducing hp
                if (playerAnims[playerState].destRec.Intersects(enemyEasyAnims[enemyEasyState].destRec))
                {
                    hurtSnd.CreateInstance().Play();
                    health -= 10;
                    playerAnims[playerState].destRec.X = enemyEasyAnims[enemyEasyState].destRec.X + playerAnims[playerState].destRec.Width + 1;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerSpeed.X *= NEG;
                    collision = true;
                }

                //Adjust the door based on the button that was collided with
                if (isButtonPressed[0] == true)
                {
                    AdjustDoor(1);
                }
                if (isButtonPressed[1] == true)
                {
                    AdjustDoor(2);
                }

                //If a collision occured then the player's collision rectangles need to be adjusted
                if (collision == true)
                {
                    SetPlayerRecs(playerRecs, playerAnims);
                    collision = false;
                }
            }
            else if (modes[MEDIUM] == true)
            {
                //Detect collision for spikes
                for (int i = 0; i < 2; i++)
                {
                    if (playerRecs[FEET].Intersects(mediumObjectRecs[i]))
                    {
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        collision = true;
                    }
                    else if (playerRecs[RIGHT_TOE].Intersects(mediumObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = mediumObjectRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }
                    else if (playerRecs[LEFT_TOE].Intersects(mediumObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = mediumObjectRecs[i].X + easyObjectRecs[i].Width + 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }
                }

                //Detect collision for teleporters and adjust position based on teleporter entered
                if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[2]))
                {
                    swishSnd.CreateInstance().Play();
                    playerAnims[playerState].destRec.X = mediumObjectRecs[3].X - playerAnims[playerState].destRec.Width - 1;
                    playerAnims[playerState].destRec.Y = platformMediumRecs[5].Y - playerAnims[playerState].destRec.Height;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerPos.Y = playerAnims[playerState].destRec.Y;
                    collision = true;
                }
                else if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[3]))
                {
                    swishSnd.CreateInstance().Play();
                    playerAnims[playerState].destRec.X = mediumObjectRecs[2].X + mediumObjectRecs[2].Width + 1;
                    playerAnims[playerState].destRec.Y = platformMediumRecs[2].Y - playerAnims[playerState].destRec.Height;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerPos.Y = playerAnims[playerState].destRec.Y;
                    collision = true;
                }

                //Detect collision of button
                if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[4]))
                {
                    isButtonPressed[0] = true;
                    mediumObjectRecs[4] = Rectangle.Empty;
                    doorTimer.ResetTimer(true);
                }

                //Detect collision of doors
                if (playerRecs[RIGHT].Intersects(mediumObjectRecs[5]))
                {
                    playerAnims[playerState].destRec.X = mediumObjectRecs[5].X - playerAnims[playerState].destRec.Width - 1;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerSpeed.X *= NEG;
                    collision = true;
                }
                else if (playerRecs[LEFT].Intersects(mediumObjectRecs[5]))
                {
                    playerAnims[playerState].destRec.X = mediumObjectRecs[5].X + mediumObjectRecs[5].Width + 1;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerSpeed.X *= NEG;
                    collision = true;
                }

                //If the player collides with a gem, it disappears and is added to the score
                for (int i = 6; i < 8; i++)
                {
                    //Add gems if player collides with gem
                    if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[i]))
                    {
                        //Increment gems
                        gems++;
                        mediumObjectRecs[i] = Rectangle.Empty;
                        collectSnd.CreateInstance().Play();
                    }
                }

                //If the escape portal is collided with and all the required gems are collected, then change the screen to gameover
                if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[8]))
                {
                    //If all the gems are collected then change screen to gameover
                    if (isGemsCollected == true)
                    {
                        //Change screen to gameover
                        gameState = GAMEOVER;

                        //Store the amount of time taken to pass level
                        timePassed = gameTimer.GetTimePassed();
                        gameTimer.IsPaused();

                        //Check for a new highscore
                        CheckHighscore();

                        //Play game over music
                        MediaPlayer.Stop();
                        MediaPlayer.Play(gameoverMusic);
                    }
                }

                //If the player collides with a power-up, apply the power-up to the player
                if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[9]))
                {        
                    //Increase health
                    health += 40;

                    //If the health is over the max amount of health, then set it to the max health
                    if (health >= MAX_HEALTH)
                    {
                        health = MAX_HEALTH;
                    }

                    mediumObjectRecs[9] = Rectangle.Empty;
                }
                else if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[10]))
                {
                    //Speed is boosted and cooldown timer for it begins
                    isSpeedBoosted = true;
                    speedTimer.ResetTimer(true);
                    mediumObjectRecs[10] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }
                else if (playerAnims[playerState].destRec.Intersects(mediumObjectRecs[11]))
                {
                    //Damage is boosted and cooldown timer for it begins
                    isDamageBoosted = true;
                    damageTimer.ResetTimer(true);
                    mediumObjectRecs[11] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }

                //If the player hits the enemy, it cannot go past it and will bounce back while reducing hp
                if (playerAnims[playerState].destRec.Intersects(enemyMediumAnims[enemyMediumState].destRec))
                {
                    hurtSnd.CreateInstance().Play();
                    health -= 5;
                    playerAnims[playerState].destRec.X = enemyMediumAnims[enemyMediumState].destRec.Right + 1;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerSpeed.X *= NEG;
                    collision = true;
                }

                //Adjust the door based on the button that was collided with
                if (isButtonPressed[0] == true)
                {
                    AdjustDoor(1);
                }

                //If a collision occured then the player's collision rectangles need to be adjusted
                if (collision == true)
                {
                    SetPlayerRecs(playerRecs, playerAnims);
                    collision = false;
                }
            }
            else if (modes[HARD] == true)
            {
                //Detect collision for spikes
                for (int i = 0; i < 2; i++)
                {
                    if (playerRecs[FEET].Intersects(HardObjectRecs[i]))
                    {
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        collision = true;
                    }
                    else if (playerRecs[RIGHT_TOE].Intersects(HardObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = HardObjectRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }
                    else if (playerRecs[LEFT_TOE].Intersects(HardObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = HardObjectRecs[i].X + easyObjectRecs[i].Width + 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        playerSpeed.X *= NEG;
                        playerSpeed.Y *= NEG;
                        collision = true;
                    }
                }

                //Detect collision for teleporters and adjust position based on teleporter entered
                if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[2]))
                {
                    swishSnd.CreateInstance().Play();
                    playerAnims[playerState].destRec.X = HardObjectRecs[3].X - playerAnims[playerState].destRec.Width - 1;
                    playerAnims[playerState].destRec.Y = platformHardRecs[7].Y - playerAnims[playerState].destRec.Height;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerPos.Y = playerAnims[playerState].destRec.Y;
                    collision = true;
                }
                else if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[3]))
                {
                    swishSnd.CreateInstance().Play();
                    playerAnims[playerState].destRec.X = HardObjectRecs[2].X + HardObjectRecs[2].Width + 1;
                    playerAnims[playerState].destRec.Y = platformHardRecs[6].Y - playerAnims[playerState].destRec.Height;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerPos.Y = playerAnims[playerState].destRec.Y;
                    collision = true;
                }

                //Detect collision of buttons
                if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[4]))
                {
                    //First button
                    isButtonPressed[0] = true;
                    HardObjectRecs[4] = Rectangle.Empty;
                    doorTimer.ResetTimer(true);
                }
                else if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[5]))
                {
                    //Second button
                    isButtonPressed[1] = true;
                    HardObjectRecs[5] = Rectangle.Empty;
                    doorTimer.ResetTimer(true);
                }

                //Detect collision of doors
                for (int i = 6; i < 8; i++)
                {
                    if (playerRecs[RIGHT].Intersects(HardObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = HardObjectRecs[i].X - playerAnims[playerState].destRec.Width - 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        playerSpeed.X *= NEG;
                        collision = true;
                    }
                    else if (playerRecs[LEFT].Intersects(HardObjectRecs[i]))
                    {
                        playerAnims[playerState].destRec.X = HardObjectRecs[i].X + HardObjectRecs[3].Width + 1;
                        playerPos.X = playerAnims[playerState].destRec.X;
                        playerSpeed.X *= NEG;
                        collision = true;
                    }
                }

                //If the player collides with a gem, it disappears and is added to the score
                for (int i = 8; i < 11; i++)
                {
                    //Add gems if player collides with gem
                    if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[i]))
                    {
                        //Increment gems
                        gems++;
                        HardObjectRecs[i] = Rectangle.Empty;
                        collectSnd.CreateInstance().Play();
                    }
                }
                
                //If the escape portal is collided with and all the required gems are collected, then change the screen to gameover
                if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[11]))
                {
                    //If all the gems are collected then change screen to gameover
                    if (isGemsCollected == true)
                    {
                        //Change screen to game over
                        gameState = GAMEOVER;

                        //Store the amount of time taken to pass level
                        timePassed = gameTimer.GetTimePassed();
                        gameTimer.IsPaused();

                        //Check for a new highscore
                        CheckHighscore();

                        //Play game over music
                        MediaPlayer.Stop();
                        MediaPlayer.Play(gameoverMusic);
                    }
                }

                //If the player collides with a power-up, apply the power-up to the player
                if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[12]))
                {
                    //Increase health
                    health += 50;

                    //If the health is over the max amount of health, then set it to the max health
                    if (health >= MAX_HEALTH)
                    {
                        health = MAX_HEALTH;
                    }

                    HardObjectRecs[12] = Rectangle.Empty;
                }
                else if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[13]))
                {
                    //Speed is boosted and cooldown timer for it begins
                    isSpeedBoosted = true;
                    speedTimer.ResetTimer(true);
                    HardObjectRecs[13] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }
                else if (playerAnims[playerState].destRec.Intersects(HardObjectRecs[14]))
                {
                    //Damage is boosted and cooldown timer for it begins
                    isDamageBoosted = true;
                    damageTimer.ResetTimer(true);
                    HardObjectRecs[14] = Rectangle.Empty;
                    collectSnd.CreateInstance().Play();
                }

                //If the player hits the enemy, it cannot go past it and will bounce back while reducing hp
                if (playerAnims[playerState].destRec.Intersects(enemyHardAnims[enemyHardState].destRec))
                {
                    playerAnims[playerState].destRec.X = enemyHardAnims[enemyHardState].destRec.Right + 20;
                    playerPos.X = playerAnims[playerState].destRec.X;
                    playerSpeed.X *= NEG;
                    collision = true;

                    //Only allow collision with hard enemy to take away health once every few seconds 
                    if (!enemyHealthTimer.IsActive())
                    {
                        hurtSnd.CreateInstance().Play();
                        health -= 5;
                        enemyHealthTimer.ResetTimer(true);
                    }
                }

                //Adjust the door based on the button that was collided with
                if (isButtonPressed[0] == true)
                {
                    AdjustDoor(1);
                }
                if (isButtonPressed[1] == true)
                {
                    AdjustDoor(2);
                }

                //If a collision occured then the player's collision rectangles need to be adjusted
                if (collision == true)
                {
                    SetPlayerRecs(playerRecs, playerAnims);
                    collision = false;
                }
            }

            //Deactivate power-ups when their timers are done
            if (speedTimer.IsFinished())
            {
                isSpeedBoosted = false;
            }
            else if (damageTimer.IsFinished())
            {
                isDamageBoosted = false;
            }
        }

        //Pre: doorNo is corresponding to which button was pressed
        //Post: None
        //Desc: Adjusts the door to allow the player to go through.
        private void AdjustDoor(int doorNo)
        {
            //Adjust door based on the level's environment
            if (modes[EASY] == true)
            {
                //Change the Height of the door
                easyObjectRecs[doorNo + 5].Height = (int)(actualEasyDoorRecs[doorNo - 1].Height * (doorTimer.GetTimeRemaining() / DOOR_TIME));

                //Once the door is fully opened, reset the button's state
                if (easyObjectRecs[doorNo + 5].Height == 0)
                {
                    isButtonPressed[doorNo - 1] = false;
                    easyObjectRecs[doorNo + 5] = Rectangle.Empty;
                }
            }
            else if (modes[MEDIUM] == true)
            {
                //Change the Height of the door
                mediumObjectRecs[doorNo + 4].Height = (int)(actualMediumDoorRec.Height * (doorTimer.GetTimeRemaining() / DOOR_TIME));

                //Once the door is fully opened, reset the button's state
                if (mediumObjectRecs[doorNo + 4].Height == 0)
                {
                    isButtonPressed[doorNo - 1] = false;
                    mediumObjectRecs[doorNo + 4] = Rectangle.Empty;
                }
            }
            else if (modes[HARD] == true)
            {
                //Change the Height of the door
                HardObjectRecs[doorNo + 5].Height = (int)(actualHardDoorRecs[doorNo - 1].Height * (doorTimer.GetTimeRemaining() / DOOR_TIME));

                //Once the door is fully opened, reset the button's state
                if (HardObjectRecs[doorNo + 5].Height == 0)
                {
                    isButtonPressed[doorNo - 1] = false;
                    HardObjectRecs[doorNo + 5] = Rectangle.Empty;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle when a bullet can be shot by the enemy
        private void EnemyBullets()
        {
            //Move all active bullets
            for (int i = 0; i < enemyBulletRecs.Length; i++)
            {
                //A bullet is active if its X or Y components are different than the initial inactive storage location
                if (enemyBulletRecs[i].X != inactivePos.X || enemyBulletRecs[i].Y != inactivePos.Y)
                {
                    //Move enemy bullets 
                    enemyBulletRecs[i].X = (int)(enemyBulletRecs[i].X + enemyBulletMaxSpeed);
                }
            }

            //Handle collision detection for all active enemy bullets
            for (int i = 0; i < enemyBulletRecs.Length; i++)
            {
                //A bullet is active if its X or Y components are different than the initial inactive storage location
                if (enemyBulletRecs[i].X != inactivePos.X || enemyBulletRecs[i].Y != inactivePos.Y)
                {
                    //If bullets go past sides of screen, store it in an inactive location
                    if (enemyBulletRecs[i].X <= 0 || enemyBulletRecs[i].X >= screenWidth)
                    {
                        //Deactivate by moving it to the storage location
                        enemyBulletRecs[i].X = (int)inactivePos.X;
                        enemyBulletRecs[i].Y = (int)inactivePos.Y;
                    }

                    //Detect collision based on the mode
                    if (modes[MEDIUM] == true)
                    {
                        //Detect collision for every platform
                        for (int j = 0; j < platformMediumRecs.Length; j++)
                        {
                            //Detect if bullet collides with environment's platforms
                            if (enemyBulletRecs[i].Intersects(platformMediumRecs[j]))
                            {
                                //Deactivate by moving it to the storage location
                                enemyBulletRecs[i].X = (int)inactivePos.X;
                                enemyBulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect collision with player and take away health if collided
                        if (enemyBulletRecs[i].Intersects(playerAnims[playerState].destRec))
                        {
                            //Deactivate by moving it to the storage location
                            enemyBulletRecs[i].X = (int)inactivePos.X;
                            enemyBulletRecs[i].Y = (int)inactivePos.Y;
                            hurtSnd.CreateInstance().Play();
                            health -= 25f;
                        }
                    }
                    else if (modes[HARD] == true)
                    {
                        //Detect collision for every platform
                        for (int j = 0; j < platformHardRecs.Length; j++)
                        {
                            //Detect if bullet collides with environment's platforms
                            if (enemyBulletRecs[i].Intersects(platformHardRecs[j]))
                            {
                                //Deactivate by moving it to the storage location
                                enemyBulletRecs[i].X = (int)inactivePos.X;
                                enemyBulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect collision with player and take away health if collided
                        if (enemyBulletRecs[i].Intersects(playerAnims[playerState].destRec))
                        {
                            //Deactivate by moving it to the storage location
                            enemyBulletRecs[i].X = (int)inactivePos.X;
                            enemyBulletRecs[i].Y = (int)inactivePos.Y;
                            hurtSnd.CreateInstance().Play();
                            health -= 30f;
                        }
                    }
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Handle when a bullet can be shot by the player
        private void PlayerBullets()
        {
            //Only shoot on new mouse clicks
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Search for an inactive bullet 
                int bulletIndex = FindInactiveItem(bulletRecs);

                //If an inactive bullet was found, activate it
                if (bulletIndex >= 0)
                {
                    //Play a shoot sound effect
                    shootSnd.CreateInstance().Play();

                    //Determine which direction the bullet must travel
                    if (dir == POS && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        bulletDir[bulletIndex] = POS;
                    }
                    else if (dir == NEG && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
                    {
                        bulletDir[bulletIndex] = NEG;
                    }

                    //Move the bullet to the side the character is facing
                    if (bulletDir[bulletIndex] == POS)
                    {
                        bulletRecs[bulletIndex].X = playerAnims[playerState].destRec.Right;
                    }
                    else if (bulletDir[bulletIndex] == NEG)
                    {
                        bulletRecs[bulletIndex].X = playerAnims[playerState].destRec.Left;
                    }

                    //Make the bullet come from the the upper half of the player's body
                    bulletRecs[bulletIndex].Y = playerAnims[playerState].destRec.Y + playerAnims[playerState].destRec.Height / 4;
                }

            }

            //Move all active bullets
            for (int i = 0; i < bulletRecs.Length; i++)
            {
                //A bullet is active if its X or Y components are different than the initial inactive storage location
                if (bulletRecs[i].X != inactivePos.X || bulletRecs[i].Y != inactivePos.Y)
                {
                    //Move the bullet based on the direction it must travel
                    if (bulletDir[i] == POS)
                    {
                        //Move bullet to right
                        bulletRecs[i].X = (int)(bulletRecs[i].X + bulletMaxSpeed);
                    }
                    else if (bulletDir[i] == NEG)
                    {
                        //Move bullet to left
                        bulletRecs[i].X = (int)(bulletRecs[i].X - bulletMaxSpeed);
                    }
                }
            }

            //Handle collision detection for all active bullets
            for (int i = 0; i < bulletRecs.Length; i++)
            {
                //A bullet is active if its X or Y components are different than the initial inactive storage location
                if (bulletRecs[i].X != inactivePos.X || bulletRecs[i].Y != inactivePos.Y)
                {
                    //Width of screen collision test
                    if (bulletRecs[i].X <= 0 || bulletRecs[i].X >= screenWidth)
                    {
                        //Deactivate by moving it to the storage location
                        bulletRecs[i].X = (int)inactivePos.X;
                        bulletRecs[i].Y = (int)inactivePos.Y;
                    }

                    //Detect collision based on the mode
                    if (modes[EASY] == true)
                    {
                        //Detect collision for every platform
                        for (int j = 0; j < platformEasyRecs.Length; j++)
                        {
                            //Detect if bullet collides with environment's platforms
                            if (bulletRecs[i].Intersects(platformEasyRecs[j]))
                            {
                                //Deactivate by moving it to the storage location
                                bulletRecs[i].X = (int)inactivePos.X;
                                bulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect collision for every object
                        for (int k = 0; k < easyObjectRecs.Length; k++)
                        {
                            //Detect if bullet collides with environment's objects
                            if (bulletRecs[i].Intersects(easyObjectRecs[k]))
                            {
                                //Deactivate by moving it to the storage location
                                bulletRecs[i].X = (int)inactivePos.X;
                                bulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect if bullet collides with enemy
                        if (bulletRecs[i].Intersects(enemyEasyAnims[enemyEasyState].destRec))
                        {
                            //Deactivate by moving it to the storage location
                            bulletRecs[i].X = (int)inactivePos.X;
                            bulletRecs[i].Y = (int)inactivePos.Y;

                            //Increase damage against enemy if damage boost is enabled
                            if (isDamageBoosted)
                            {
                                enemyHealth[EASY] -= 50f;
                            }
                            else
                            {
                                enemyHealth[EASY] -= 25f;
                            }
                        }
                    }
                    else if (modes[MEDIUM] == true)
                    {
                        //Detect collision for every platform
                        for (int j = 0; j < platformMediumRecs.Length; j++)
                        {
                            //Detect if bullet collides with environment's platforms
                            if (bulletRecs[i].Intersects(platformMediumRecs[j]))
                            {
                                //Deactivate by moving it to the storage location
                                bulletRecs[i].X = (int)inactivePos.X;
                                bulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect collision for every object
                        for (int k = 0; k < mediumObjectRecs.Length; k++)
                        {
                            //Detect if bullet collides with environment's objects
                            if (bulletRecs[i].Intersects(mediumObjectRecs[k]))
                            {
                                //Deactivate by moving it to the storage location
                                bulletRecs[i].X = (int)inactivePos.X;
                                bulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect if bullet collides with enemy
                        if (bulletRecs[i].Intersects(enemyMediumAnims[enemyMediumState].destRec))
                        {
                            //Deactivate by moving it to the storage location
                            bulletRecs[i].X = (int)inactivePos.X;
                            bulletRecs[i].Y = (int)inactivePos.Y;

                            //Increase damage against enemy if damage boost is enabled
                            if (isDamageBoosted)
                            {
                                enemyHealth[MEDIUM] -= 40f;
                            }
                            else
                            {
                                enemyHealth[MEDIUM] -= 20f;
                            }
                        }
                    }
                    else if (modes[HARD] == true)
                    {
                        //Detect collision for every platform
                        for (int j = 0; j < platformHardRecs.Length; j++)
                        {
                            //Detect if bullet collides with environment's platforms
                            if (bulletRecs[i].Intersects(platformHardRecs[j]))
                            {
                                //Deactivate by moving it to the storage location
                                bulletRecs[i].X = (int)inactivePos.X;
                                bulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect collision for every object
                        for (int k = 0; k < HardObjectRecs.Length; k++)
                        {
                            //Detect if bullet collides with environment's objects
                            if (bulletRecs[i].Intersects(HardObjectRecs[k]))
                            {
                                //Deactivate by moving it to the storage location
                                bulletRecs[i].X = (int)inactivePos.X;
                                bulletRecs[i].Y = (int)inactivePos.Y;
                            }
                        }

                        //Detect if bullet collides with enemy
                        if (bulletRecs[i].Intersects(enemyHardAnims[enemyHardState].destRec))
                        {
                            //Deactivate by moving it to the storage location
                            bulletRecs[i].X = (int)inactivePos.X;
                            bulletRecs[i].Y = (int)inactivePos.Y;

                            //Increase damage against enemy if damage boost is enabled
                            if (isDamageBoosted)
                            {
                                enemyHealth[HARD] -= 30f;
                            }
                            else
                            {
                                enemyHealth[HARD] -= 10f;
                            }
                        }
                    }
                }
            }
        }

        //Pre: objectRecs is an array of rectangles for any rectangles in the game
        //Post: Returns the index of the first inactive item, -1 if none are found
        //Desc: Loop through all Rectangles and return the index of the first inactive one
        private int FindInactiveItem(Rectangle[] objectRecs)
        {
            //Determine if objectRecs is the player's bullets or the enemy's bullets
            if (objectRecs == bulletRecs)
            {
                //Loop through the collection of items to find the first item at the inactive storage location
                for (int i = 0; i < bulletRecs.Length; i++)
                {
                    //A bullet is active if its X or Y components are different than the initial inactive storage location
                    if (objectRecs[i].X == inactivePos.X && objectRecs[i].Y == inactivePos.Y)
                    {
                        //The first inactive item was found at index i
                        return i;
                    }
                }
            }
            else if (objectRecs == enemyBulletRecs)
            {
                //Loop through the collection of items to find the first item at the inactive storage location
                for (int i = 0; i < enemyBulletRecs.Length; i++)
                {
                    //A bullet is active if its X or Y components are different than the initial inactive storage location
                    if (objectRecs[i].X == inactivePos.X && objectRecs[i].Y == inactivePos.Y)
                    {
                        //The first inactive item was found at index i
                        return i;
                    }
                }
            }

            //No inactive item was found, return -1
            return -1;
        }

        //Pre: None
        //Post: None
        //Desc: Change both the player and enemy's health bar based on the amount of health they have remaining
        private void ModifyHealthBar()
        {
            //Adjust the amount of health displayed for the player
            playerHealthPercent = health / MAX_HEALTH;
            playerHealthBarRec.Width = (int)(actualPlayerHealthBarRec.Width * playerHealthPercent);

            //Modify the healthbar depending on which mode it is
            if (modes[EASY])
            {
                enemyHealthPercent = enemyHealth[EASY] / maxEnemyHealth;
                enemyHealthBarRec[0].Width = (int)(actualEnemyHealthBarRec[0].Width * enemyHealthPercent);
            }
            else if (modes[MEDIUM])
            {
                enemyHealthPercent = enemyHealth[MEDIUM] / maxEnemyHealth;
                enemyHealthBarRec[1].Width = (int)(actualEnemyHealthBarRec[1].Width * enemyHealthPercent);
            }
            else if (modes[HARD])
            {
                enemyHealthPercent = enemyHealth[HARD] / maxEnemyHealth;
                enemyHealthBarRec[2].Width = (int)(actualEnemyHealthBarRec[2].Width * enemyHealthPercent);
            }
               
        }

        //Pre: None
        //Post: None
        //Desc: Update the number of gems found 
        private void ModifyGems()
        {
            //Update the status of whether the gems have been collected or not
            if (modes[EASY])
            {
                if (gems == 1)
                {
                    isGemsCollected = true;
                }
            }
            else if (modes[MEDIUM])
            {
                if (gems == 2)
                {
                    isGemsCollected = true;
                }
            }
            else if (modes[HARD])
            {
                if (gems == 3)
                {
                    isGemsCollected = true;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Reset the game
        private void ResetGame()
        {
            //Reset game stats
            health = MAX_HEALTH;
            gems = 0;
            isGemsCollected = false;

            //Reset player info
            dir = POS;
            playerSpeed = new Vector2(0f, 0f);
            grounded = false;

            //Reset enemy percentage
            enemyHealthPercent = 0f;

            //Reset bools that track if player has clicked, if there is a power-up collected, or if a button has been pressed
            isClicked = false;
            isDamageBoosted = false;
            isSpeedBoosted = false;
            for (int i = 0; i < isButtonPressed.Length; i++)
            {
                isButtonPressed[i] = false;
            }


            //Reset based on the mode
            if (modes[EASY] == true)
            {
                //Reset object rectangles and enemy info
                easyObjectRecs[4] = new Rectangle(350, 785, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
                easyObjectRecs[5] = new Rectangle(1375, 310, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
                easyObjectRecs[6] = new Rectangle(425, 675, 25, 125);
                easyObjectRecs[7] = new Rectangle(900, 75, 25, 125);
                easyObjectRecs[8] = new Rectangle(720, 738, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
                easyObjectRecs[10] = new Rectangle(1500, 610, healthBoostImg.Width, healthBoostImg.Height);
                easyObjectRecs[11] = new Rectangle(100, 610, speedBoostImg.Width, speedBoostImg.Height);
                easyObjectRecs[12] = new Rectangle(1400, 760, damageBoostImg.Width, damageBoostImg.Height);
                enemyEasyPos = new Vector2(760, 722);
                enemyEasyAnims[ENEMY_IDLE] = new Animation(enemyEasyImgs[ENEMY_IDLE], 4, 1, 4, 0,
                        Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                        5, enemyEasyPos, 1f, true);
                actualEnemyHealthBarRec[0] = new Rectangle((int)enemyEasyPos.X, (int)(enemyEasyPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
                enemyHealthBarRec[0] = new Rectangle((int)enemyEasyPos.X, (int)(enemyEasyPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
                enemyHealth[EASY] = 100f;
                enemyEasyState = ENEMY_IDLE;
            }
            else if (modes[MEDIUM] == true)
            {
                //Reset object rectangles and enemy info
                mediumObjectRecs[4] = new Rectangle(1300, 785, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
                mediumObjectRecs[5] = new Rectangle(725, 375, 25, 125);
                mediumObjectRecs[6] = new Rectangle(300, 216, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
                mediumObjectRecs[7] = new Rectangle(1100, 766, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
                mediumObjectRecs[9] = new Rectangle(830, 285, healthBoostImg.Width, healthBoostImg.Height);
                mediumObjectRecs[10] = new Rectangle(1500, 760, speedBoostImg.Width, speedBoostImg.Height);
                mediumObjectRecs[11] = new Rectangle(980, 285, damageBoostImg.Width, damageBoostImg.Height);
                enemyMediumPos = new Vector2(400, 185);
                enemyMediumAnims[ENEMY_IDLE] = new Animation(enemyMediumImgs[ENEMY_IDLE], 4, 1, 4, 0,
                                   Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                   5, enemyMediumPos, 0.85f, true);
                enemyMediumAnims[ENEMY_ATTACK] = new Animation(enemyMediumImgs[ENEMY_ATTACK], 6, 1, 6, 0,
                                        Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                        5, enemyMediumPos, 0.85f, true);
                actualEnemyHealthBarRec[1] = new Rectangle((int)enemyMediumPos.X + 25, (int)(enemyMediumPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
                enemyHealthBarRec[1] = new Rectangle((int)enemyMediumPos.X + 25, (int)(enemyMediumPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
                enemyHealth[MEDIUM] = 100f;
                enemyMediumState = ENEMY_IDLE;
            }
            else if (modes[HARD] == true)
            {
                //Reset object rectangles and enemy info
                HardObjectRecs[4] = new Rectangle(375, 410, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
                HardObjectRecs[5] = new Rectangle(275, 785, (int)(buttonImg.Width * buttonScaler), (int)(buttonImg.Height * buttonScaler));
                HardObjectRecs[6] = new Rectangle(360, 675, 25, 125);
                HardObjectRecs[7] = new Rectangle(850, 325, 25, 125);
                HardObjectRecs[8] = new Rectangle(1250, 766, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
                HardObjectRecs[9] = new Rectangle(60, 391, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
                HardObjectRecs[10] = new Rectangle(550, 166, (int)(gemImg.Width * gemScaler), (int)(gemImg.Height * gemScaler));
                HardObjectRecs[12] = new Rectangle(1150, 410, healthBoostImg.Width, healthBoostImg.Height);
                HardObjectRecs[13] = new Rectangle(275, 740, speedBoostImg.Width, speedBoostImg.Height);
                HardObjectRecs[14] = new Rectangle(1450, 260, damageBoostImg.Width, damageBoostImg.Height);
                enemyDir = POS;
                enemyHardPos = new Vector2(650, 110);
                enemyHardAnims[ENEMY_WALK] = new Animation(enemyHardImgs[ENEMY_WALK], 6, 1, 6, 0,
                                        Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                        10, enemyHardPos, 0.75f, true);
                enemyHardAnims[ENEMY_ATTACK] = new Animation(enemyHardImgs[ENEMY_ATTACK], 6, 1, 6, 0,
                                      Animation.NO_IDLE, Animation.ANIMATE_FOREVER,
                                      10, enemyHardPos, 0.75f, true);
                actualEnemyHealthBarRec[2] = new Rectangle((int)enemyHardPos.X, (int)(enemyHardPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
                enemyHealthBarRec[2] = new Rectangle((int)enemyHardPos.X, (int)(enemyHardPos.Y - 20), healthBarImg.Width, healthBarImg.Height);
                enemyHealth[HARD] = 100f;
                enemyHardState = ENEMY_WALK;
            }

            //Reset player position
            playerPos = new Vector2(10, 730);

            //Reset the current active mode
            for (int i = 0; i < modes.Length; i++)
            {
                modes[i] = false;
            }

            //Reset bool for if the highscore was changed
            isHighscoresChanged[EASY] = false;
            isHighscoresChanged[MEDIUM] = false;
            isHighscoresChanged[HARD] = false;
        }

        //Pre: None
        //Post: None
        //Desc: Detect if the player is within radius
        private void PlayerDetection()
        {
            //Distance between player and enemy
            int distanceMediumBetween = 0;
            int distanceHardBetween = 0;

            //Calculate the distance between player and enemy based on the mode
            if (modes[MEDIUM] == true)
            {
                //Distance between player and medium enemy
                distanceMediumBetween = Math.Abs(playerAnims[playerState].destRec.X - enemyMediumAnims[enemyMediumState].destRec.Right);
            }
            else if (modes[HARD] == true)
            {
                //Distance between player and hard enemy
                distanceHardBetween = Math.Abs(playerAnims[playerState].destRec.X - enemyHardAnims[enemyHardState].destRec.Right);
            }
                
            //Attack the player based on distance between player and enemy
            EnemyAction(distanceMediumBetween, distanceHardBetween);
        }

        //Pre: The player is on the screen at a distance from the enemy 
        //Post: None
        //Desc: Enemies attack the player
        private void EnemyAction(int distMed, int distHard)
        {
            //Perform enemy actions based on the mode
            if (modes[MEDIUM] == true)
            {
                //Enemy attacks player if player is within its radius
                if (distMed <= 300 && playerAnims[playerState].destRec.Y <= platformMediumRecs[3].Y)
                {
                    //Change enemy state
                    enemyMediumState = ENEMY_ATTACK;

                    //Only allow enemy to shoot bullets after cooldown is over
                    if (mediumAttackTimer.GetTimePassed() >= MEDIUM_SHOOT_COOLDOWN || !mediumAttackTimer.IsActive())
                    {
                        mediumAttackTimer.ResetTimer(true);
                        ShootEnemyBullets();
                    }
                }
                else 
                {
                    //Enemy remains idle and does not attack
                    enemyMediumState = ENEMY_IDLE;
                    mediumAttackTimer.IsInactive();
                }
            }
            else if (modes[HARD] == true)
            {
                //Enemy attacks player if player is within its radius
                if (distHard <= 100 && playerAnims[playerState].destRec.Y <= platformHardRecs[6].Y)
                {
                    //Change enemy state
                    enemyHardState = ENEMY_ATTACK;
                    enemyDir = POS;

                    //Only allow enemy to shoot bullets after cooldown is over
                    if (hardAttackTimer.GetTimePassed() >= HARD_SHOOT_COOLDOWN || !hardAttackTimer.IsActive())
                    {
                        hardAttackTimer.ResetTimer(true);
                        ShootEnemyBullets();
                    }
                }
                else 
                {
                    //Enemy remains walking and does not attack
                    enemyHardState = ENEMY_WALK;
                    hardAttackTimer.IsInactive();
                }
            }
        }

        //Pre: None 
        //Post: None
        //Desc: Enemy shoots the bullet
        private void ShootEnemyBullets()
        {
            //Search for an inactive bullet 
            int enemyBulletIndex = FindInactiveItem(enemyBulletRecs);

            //If an inactive bullet was found, activate it
            if (enemyBulletIndex >= 0)
            {
                //Position bullets based on the enemy in the mode
                if (modes[MEDIUM] == true)
                {
                    enemyBulletRecs[enemyBulletIndex].X = enemyMediumAnims[enemyMediumState].destRec.Right;
                    enemyBulletRecs[enemyBulletIndex].Y = enemyMediumAnims[enemyMediumState].destRec.Y + enemyMediumAnims[enemyMediumState].destRec.Height / 2;
                }
                else if (modes[HARD] == true)
                {
                    enemyBulletRecs[enemyBulletIndex].X = enemyHardAnims[enemyHardState].destRec.Right;
                    enemyBulletRecs[enemyBulletIndex].Y = enemyHardAnims[enemyHardState].destRec.Y + enemyHardAnims[enemyHardState].destRec.Height / 2;
                }
            }
        }

        //Pre: None 
        //Post: None
        //Desc: Check for a new highscore and replace the old one if there is a new one
        private void CheckHighscore()
        {
            //Convert the game time passed to time in seconds
            timeInSeconds = timePassed / 1000;

            //Check the highscore based on the game mode
            if (modes[EASY] == true)
            {
                //Change the highscore if the current easy mode score is higher than past easy mode highscores
                if (timeInSeconds > highscores[EASY])
                {
                    //Change new easy highscore to current score
                    highscores[EASY] = timeInSeconds;
                    isHighscoresChanged[EASY] = true;
                }
            }
            else if (modes[MEDIUM] == true)
            {
                //Change the highscore if the current medium mode score is higher than past medium mode highscores
                if (timeInSeconds > highscores[MEDIUM])
                {
                    //Change new medium highscore to current score
                    highscores[MEDIUM] = timeInSeconds;
                    isHighscoresChanged[MEDIUM] = true;
                }
            }
            if (modes[HARD] == true)
            {
                //Change the highscore if the current hard mode score is higher than past hard mode highscores
                if (timeInSeconds > highscores[HARD])
                {
                    //Change new hard highscore to current score
                    highscores[HARD] = timeInSeconds;
                    isHighscoresChanged[HARD] = true;
                }
            }
        }

        //Pre: None
        //Post: None
        //Desc: Make the enemy patrol like an AI
        private void EnemyPatrol()
        {
            //If the enemy reaches the a boundary, go in the opposite direction.
            if (enemyHardAnims[enemyHardState].destRec.X < leftBoundary)
            {
                enemyDir = POS;
            }
            else if (enemyHardAnims[enemyHardState].destRec.X > rightBoundary)
            {
                enemyDir = NEG;
            }

            //Move the enemy health bar with the enemy
            actualEnemyHealthBarRec[2].X += (int)(enemyDir * enemySpeed);
            enemyHealthBarRec[2].X += (int)(enemyDir * enemySpeed);

            //Move the enemy
            enemyHardPos.X += enemyDir * enemySpeed;
            enemyHardAnims[enemyHardState].destRec.X = (int)enemyHardPos.X;
        }
    }
}
