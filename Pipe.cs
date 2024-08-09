using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoogeyMan
{
    public class Pipe
    {
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public bool isTopPipe { get; set; }
        public const int PipeWidth = 60;
        public const int PipeHeight = 400;
        public int speed;

        public Pipe(Texture2D texture, Vector2 position, bool isTopPipe, int speed)
        {
            this.texture = texture;
            this.position = position;
            this.isTopPipe = isTopPipe;
            this.speed = speed;
        }

        public void Update()
        {
            this.position = new Vector2(this.position.X - this.speed, this.position.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.position, null, Color.White, 0, Vector2.Zero, 1, this.isTopPipe ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
        }
    }
}
