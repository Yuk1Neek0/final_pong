using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    // Base class for all game objects with a texture, position, direction, and speed
    internal class Sprite
    {
        protected Texture2D texture;        // Image used to represent the sprite
        protected Vector2 direction;        // Movement direction of the sprite
        protected Vector2 position;         // Current position of the sprite

        // Public property to access or modify the sprite's position
        public Vector2 Poisition
        {
            get { return this.position; }
            set { this.position = value; }
        }

        // Public property to access or modify the sprite's movement direction
        public Vector2 Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        protected float speed;              // Movement speed of the sprite
        protected Rectangle screen;         // Screen bounds used to check collisions

        // Public property to access or modify speed
        public float Speed
        {
            get { return this.speed; }
            set { this.speed = value; }
        }

        // Property that returns the current rectangular boundary of the sprite
        public Rectangle spriteBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height); }
        }

        // Constructor initializes the sprite with a texture, position, direction, speed, and screen boundaries
        public Sprite(Texture2D texture, Vector2 position, Vector2 direction, float speed, Rectangle screen)
        {
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.screen = screen;
        }

        // Virtual method to update the sprite’s position based on direction and speed (called every frame)
        public virtual void Update(GameTime gametime)
        {
            position += direction * speed;
        }

        // Virtual method to draw the sprite onto the screen using a SpriteBatch
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
