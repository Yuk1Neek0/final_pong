using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Dynamic;
using System.Windows.Forms;

namespace pong
{
    // Represents the player-controlled paddle
    internal class Paddle_Player : Sprite
    {
        public int score;            // Player's current score
        public bool isSlowBall;     // Whether the player has triggered the slow ball ability
        public int slowTimes;       // Remaining uses of the slow ball ability

        // Constructor initializes the paddle's properties and sets initial score and slow count
        public Paddle_Player(Texture2D texture, Vector2 position, Vector2 direction, float speed, Rectangle screen)
            : base(texture, position, direction, speed, screen)
        {
            score = 0;
            slowTimes = 1;        // Player can slow the ball once per round
            isSlowBall = false;
        }

        // Called every frame to update paddle movement and behavior
        public override void Update(GameTime gametime)
        {
            direction = Vector2.Zero;    // Reset movement direction each frame
            InputKeyboard(gametime);     // Check for keyboard input
            BoundsRestrictions();        // Keep paddle within the screen
            base.Update(gametime);       // Move the paddle
        }

        // Prevents the paddle from moving beyond the top and bottom screen edges
        public void BoundsRestrictions()
        {
            if (position.Y < 0)
                position.Y = 0;
            if (position.Y > screen.Height - texture.Height)
                position.Y = screen.Height - texture.Height;
        }

        // Handles player keyboard input: W/S to move, A to activate slow ball
        private void InputKeyboard(GameTime gametime)
        {
            // Move up
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                direction.Y = -1;

            // Move down
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                direction.Y = 1;

            // Activate slow ball (only if available)
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) && slowTimes > 0)
            {
                slowTimes--;          // Use up one slow
                isSlowBall = true;    // Set flag to trigger slow logic in Ball
            }
        }
    }
}
