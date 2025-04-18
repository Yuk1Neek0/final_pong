using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Threading;
using SharpDX;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace pong
{
    // The Ball class controls ball behavior, including movement, collisions, scoring, and speed changes
    internal class Ball : Sprite
    {
        private Random rand; // Used to randomize ball direction after reset
        public int player1Score;
        public int player2Score;
        public bool isSlow;          // Indicates if the ball is currently slowed down
        public bool isSlowBegin;     // Indicates if the slowdown timer has started
        public long end;             // Timestamp when slowdown should end
        public bool isrestart;       // Indicates if the ball has just been reset

        // Constructor to initialize ball with default values
        public Ball(Texture2D texture, Vector2 position, Vector2 direction, float speed, Rectangle screen)
            : base(texture, position, direction, speed, screen)
        {
            rand = new Random();
            player1Score = 0;
            player2Score = 0;
            isSlow = false;
            isSlowBegin = false;
            isrestart = false;
        }

        // Main update method called every frame
        public override void Update(GameTime gametime)
        {
            BoundsScreen(); // Check if the ball hits the screen boundaries

            if (!isSlow)
            {
                SpeedUp(); // Gradually increase speed over time
                isSlowBegin = false;
            }
            else
            {
                // Start slowdown only once
                if (!isSlowBegin)
                {
                    isSlowBegin = true;
                    end = this.SlowBall(gametime); // Set when slowdown ends
                }

                // End slow state after a duration
                if (gametime.TotalGameTime.Ticks - end >= 0)
                    isSlow = false;
            }

            base.Update(gametime); // Call base class update (moves sprite)
        }

        // Check collision with screen edges and handle scoring
        public void BoundsScreen()
        {
            if (position.Y < 0 || position.Y > screen.Height - texture.Height)
                direction.Y *= -1; // Bounce vertically

            if (position.X < 0)
            {
                player2Score++;    // AI scores
                Restart();         // Reset ball
                isrestart = true;
            }

            if (position.X > screen.Width - texture.Width)
            {
                player1Score++;    // Player scores
                Restart();         // Reset ball
                isrestart = true;
            }
        }

        // Check collision with paddles and reflect ball direction
        public void BoundsPaddle(Paddle_Player PaddlePlayer, PaddleAI PaddleAI)
        {
            if (spriteBox.Intersects(PaddlePlayer.spriteBox) || spriteBox.Intersects(PaddleAI.spriteBox))
            {
                direction.X *= -1; // Bounce horizontally
                direction.Y += rand.NextFloat(-1, 1); // Add some randomness to Y direction
            }
        }

        // Unused placeholder (optional)
        public void StartBallPosition()
        {
        }

        // Reset ball to center and set a new random direction
        public void Restart()
        {
            Poisition = new Vector2(screen.Width / 2 - texture.Width / 2, screen.Height / 2 - texture.Height / 2);
            speed = 3f;

            int randNumber = rand.Next(0, 4);

            switch (randNumber)
            {
                case 0:
                    Direction = new Vector2(1, 1);
                    break;
                case 1:
                    Direction = new Vector2(-1, 1);
                    break;
                case 2:
                    Direction = new Vector2(1, -1);
                    break;
                case 3:
                    Direction = new Vector2(-1, -1);
                    break;
            }
        }

        // Gradually increase ball speed (max 20)
        private void SpeedUp()
        {
            if (speed < 20)
                speed += 0.05f;
        }

        // Slow the ball down temporarily and return the time when slowdown ends
        public long SlowBall(GameTime gametime)
        {
            isSlow = true;
            speed = 3f;
            TimeSpan duration = new TimeSpan(50000000); // 5 seconds approx (in ticks)
            long end1 = gametime.TotalGameTime.Ticks + duration.Ticks;
            return end1;
        }
    }
}
