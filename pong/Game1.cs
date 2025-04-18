// Main game class for the Pong game using MonoGame framework
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using YourGameNamespace;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    public class Game1 : Game
    {
        // Core game components
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game objects
        private Paddle_Player PaddlePlayer;
        private PaddleAI PlayerAI;
        private Ball _ball;

        // Textures and screen rectangle
        private Texture2D PaddleTexture, BallTexture, PaddleAITexture;
        private Microsoft.Xna.Framework.Rectangle screen;

        // Random generator for various effects
        private Random rand;

        // Fonts for displaying text
        private SpriteFont ScorePlayer, ScoreAI, BallSpeed, winLose, font;

        // Win condition and game state
        private int winScore;
        private bool isOver;

        // Background music
        private Song backgroundMusic;

        // Historical win counters
        private int player1HistoryWins = 0;
        private int player2HistoryWins = 0;
        private string winner = "";

        // Starfield background
        List<Star> stars;
        Texture2D starTexture;
        private Random random;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set screen size
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 600;
            screen = new Microsoft.Xna.Framework.Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _graphics.ApplyChanges();

            rand = new Random();
        }

        protected override void Initialize()
        {
            // Game logic initialization
            winScore = 5;
            isOver = false;
            base.Initialize();

            // Create stars for background
            int numberOfStars = 200;
            stars = new List<Star>();
            for (int i = 0; i < numberOfStars; i++)
            {
                stars.Add(new Star(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, rand));
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures and create paddles/ball
            PaddleTexture = Content.Load<Texture2D>("Paddle");
            PaddlePlayer = new Paddle_Player(PaddleTexture, new Vector2(0, screen.Height / 2 - PaddleTexture.Height / 2), Vector2.Zero, 5f, screen);

            BallTexture = Content.Load<Texture2D>("Ball");
            PaddleAITexture = Content.Load<Texture2D>("Paddle");
            PlayerAI = new PaddleAI(PaddleTexture, new Vector2(screen.Width - PaddleAITexture.Width, screen.Height / 2 - PaddleTexture.Height / 2), Vector2.Zero, 5f, screen);

            // Load fonts
            ScorePlayer = Content.Load<SpriteFont>("File");
            ScoreAI = Content.Load<SpriteFont>("File");
            BallSpeed = Content.Load<SpriteFont>("File");
            winLose = Content.Load<SpriteFont>("File");
            font = Content.Load<SpriteFont>("File");

            // Initialize ball
            _ball = new Ball(BallTexture, new Vector2(screen.Width / 2 - BallTexture.Width / 2, screen.Height / 2 - BallTexture.Height / 2), new Vector2(0, -1), 1f, screen);
            _ball.Restart();

            // Create texture for stars
            starTexture = CreateCircleTexture(10);

            // Load and play background music
            backgroundMusic = Content.Load<Song>("bgm");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(backgroundMusic);
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit on ESC or Back button
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            // Main gameplay update loop (only if game is not over)
            if (_ball.player1Score < winScore && _ball.player2Score < winScore)
            {
                PaddlePlayer.Update(gameTime);
                PlayerAI.Update(gameTime);
                _ball.BoundsPaddle(PaddlePlayer, PlayerAI);
                _ball.Update(gameTime);

                // Check and apply slow ball logic
                if (PaddlePlayer.isSlowBall)
                {
                    PaddlePlayer.isSlowBall = false;
                    _ball.isSlow = true;
                }
                if (PlayerAI.isSlowBall)
                {
                    PlayerAI.isSlowBall = false;
                    _ball.isSlow = true;
                }

                // Handle restart conditions
                if (_ball.isrestart)
                {
                    _ball.isSlow = false;
                    PlayerAI.slowTimes = 1;
                    PaddlePlayer.slowTimes = 1;
                    _ball.isrestart = false;
                }
            }
            else if (!isOver)
            {
                // First time detecting game over
                isOver = true;

                if (_ball.player1Score >= winScore)
                {
                    winner = "Player1";
                    UpdateHistoryFile(winner); // Save win history
                }
                else if (_ball.player2Score >= winScore)
                {
                    winner = "Player2";
                    UpdateHistoryFile(winner); // Save win history
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            _spriteBatch.Begin();

            // Draw scores
            _spriteBatch.DrawString(ScorePlayer, _ball.player1Score.ToString(), new Vector2(screen.Width / 2 - 50, 50), Microsoft.Xna.Framework.Color.White);
            _spriteBatch.DrawString(ScoreAI, _ball.player2Score.ToString(), new Vector2(screen.Width / 2 + 50, 50), Microsoft.Xna.Framework.Color.White);

            if (!isOver)
            {
                if (_ball.player1Score < winScore && _ball.player2Score < winScore)
                {
                    // Draw slow ball info
                    _spriteBatch.DrawString(winLose, "Slow Times:" + PaddlePlayer.slowTimes.ToString(), new Vector2(0, 70), Microsoft.Xna.Framework.Color.White);
                    _spriteBatch.DrawString(winLose, "Slow Times:" + PlayerAI.slowTimes.ToString(), new Vector2(screen.Width - 250, 70), Microsoft.Xna.Framework.Color.White);
                }

                // Draw game objects
                PaddlePlayer.Draw(_spriteBatch);
                PlayerAI.Draw(_spriteBatch);
                _ball.Draw(_spriteBatch);

                if (_ball.isSlow)
                    _spriteBatch.DrawString(winLose, "Slow>_<", new Vector2(screen.Width / 2 - 50, 90), Microsoft.Xna.Framework.Color.White);
            }
            else
            {
                // Draw win/lose messages
                if (_ball.player1Score >= winScore)
                {
                    _spriteBatch.DrawString(winLose, "Win!!!", new Vector2(10, 70), Microsoft.Xna.Framework.Color.White);
                    _spriteBatch.DrawString(winLose, "Lose...", new Vector2(screen.Width - 100, 70), Microsoft.Xna.Framework.Color.White);
                }
                else if (_ball.player2Score >= winScore)
                {
                    _spriteBatch.DrawString(winLose, "Lose...", new Vector2(50, 70), Microsoft.Xna.Framework.Color.White);
                    _spriteBatch.DrawString(winLose, "WIN...", new Vector2(screen.Width - 150, 70), Microsoft.Xna.Framework.Color.White);
                }

                // Draw history scores
                string winText = $"Winner: {winner}";
                string historyText = $"History Wins\nPlayer1: {player1HistoryWins}\nPlayer2: {player2HistoryWins}";
                _spriteBatch.DrawString(font, winText, new Vector2(100, 100), Color.White);
                _spriteBatch.DrawString(font, historyText, new Vector2(100, 140), Color.Yellow);
            }

            // Draw background stars
            foreach (var star in stars)
            {
                star.Draw(_spriteBatch, starTexture);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // Creates a circular white texture used for stars
        private Texture2D CreateCircleTexture(int diameter)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];
            float radius = diameter / 2f;

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float dist = dx * dx + dy * dy;

                    colorData[y * diameter + x] = dist <= radius * radius ? Color.White : Color.Transparent;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        // Loads and updates the win history from/to history.txt
        private void UpdateHistoryFile(string winner)
        {
            string filePath = "history.txt";
            player1HistoryWins = 0;
            player2HistoryWins = 0;

            // Read history if file exists
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Player1:"))
                            int.TryParse(line.Substring("Player1:".Length).Trim(), out player1HistoryWins);
                        else if (line.StartsWith("Player2:"))
                            int.TryParse(line.Substring("Player2:".Length).Trim(), out player2HistoryWins);
                    }
                }
            }

            // Update win count
            if (winner == "Player1") player1HistoryWins++;
            else if (winner == "Player2") player2HistoryWins++;

            // Write updated history
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine($"Player1: {player1HistoryWins}");
                writer.WriteLine($"Player2: {player2HistoryWins}");
            }
        }
    }
}
