using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SaveEarth
{
    class Fondo : Sprite
    {
        // Constructor con fondo
        public Fondo(ContentManager Content)
            : base(0, -640, "fondo", Content)
        {
            VelocY = 50;
        }

        // Mueve el fondo y si baja demasiado vuelve a subirlo
        public override void Mover(GameTime gameTime)
        {
            Y += VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Y >= 0)
                Y = -640;
        }
    }
}