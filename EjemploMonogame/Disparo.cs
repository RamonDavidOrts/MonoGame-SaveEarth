using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SaveEarth
{
    class Disparo : Sprite
    {
        // Constructor con content
        public Disparo(ContentManager Content)
            : base(0, 0, "fire", Content)
        {
            VelocY = 300;
        }

        // Constructor apoyado en una instancia previa
        public Disparo(int x, int y, Disparo disparo)
            : base(x, y, disparo)
        {
            VelocY = 300;
        }

        public override void Mover(GameTime gameTime)
        {
           Y -= VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
