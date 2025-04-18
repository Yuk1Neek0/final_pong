using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Dynamic;

namespace pong
{
    // Represents the AI-controlled paddle (though it currently uses keyboard input for testing/debug)
    internal class PaddleAI : Sprite
    {
        public int score;            // AI player’s score
        public bool isSlowBall;     // Flag to activate a slowing effect on the ball
        public int slowTimes;       // Number of slow-ball activations remaining

        // Constructor initializing the paddle's texture, position, direction, speed, and screen bounds
        public PaddleAI(Texture2D texture, Vector2 position, Vector2 direction, float speed, Rectangle screen)
            : base(texture, position, direction, speed, screen)
        {
            score = 0;              // Initialize score to zero
            slowTimes = 1;          // The AI can use slow ball once per game/round
        }

        // Called every frame to update the paddle's state
        public override void Update(GameTime gametime)
        {
            direction = Vector2.Zero;  // Reset movement direction to prevent accidental movement
            InputKeyboard();           // Process keyboard input (currently for manual control for testing)
            BoundsRestrictions();      // Prevent paddle from moving outside the screen bounds
            base.Update(gametime);     // Update paddle's position based on current direction and speed
        }

        // Processes keyboard input to move the paddle or trigger the slow-ball effect
        private void InputKeyboard()
        {
            // Move the paddle up when the Up Arrow key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                direction.Y = -1;

            // Move the paddle down when the Down Arrow key is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                direction.Y = 1;

            // Press Left Arrow key to activate the slow-ball effect (only if still available)
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && slowTimes > 0)
            {
                slowTimes--;        // Decrease the remaining slow-ball activations
                isSlowBall = true;  // Activate the slow-ball effect
            }
        }

        // Prevents the paddle from moving outside the top or bottom of the screen
        public void BoundsRestrictions()
        {
            // Ensure the paddle doesn't move above the screen
            if (position.Y < 0)
                position.Y = 0;

            // Ensure the paddle doesn't move below the screen
            if (position.Y > screen.Height - texture.Height)
                position.Y = screen.Height - texture.Height;
        }
    }
}
