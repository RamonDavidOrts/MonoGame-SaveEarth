using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SaveEarth
{
    class DisparoEnemigo : Sprite
    {
        // Constructor con Content
        public DisparoEnemigo(ContentManager Content)
            : base(0, 0, "fireEn1", Content)
        {
            VelocY = 300;
        }

        // Constructor apoyado en una instancia previa
        public DisparoEnemigo(int x, int y, DisparoEnemigo disparoEnemigo)
            : base(x, y, disparoEnemigo)
        {
            VelocY = 300;
        }

        // Cambia la imagen del disparo pasada por string
        public void CambiaImagen(ContentManager Content, string imagen)
        {
            this.imagen = Content.Load<Texture2D>(imagen);
        }

        public override void Mover(GameTime gameTime)
        {
            Y += VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}