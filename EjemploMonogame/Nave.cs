using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SaveEarth
{
    class Nave : Sprite
    {
        // La nave tiene tres imagenes según hacia el lado que se mueva
        private Texture2D imagenL;
        private Texture2D imagenR;
        private Texture2D imagenQ;

        // Constructor con content, carga imágenes y secuencia de explosión
        public Nave(ContentManager Content)
            : base(203, 600, new string[] { "nave" }, Content)
        {
            SetVelocidad(240, 240);
            imagenL = Content.Load<Texture2D>("navel");
            imagenR = Content.Load<Texture2D>("naver");
            imagenQ = Content.Load<Texture2D>("nave");

            CargarSecuencia((byte)Direcciones.DESAPARECIENDO,
                new string[] { "explosion2", "explosion3",
                    "explosion4" }, Content);
            CambiarDireccion(0);
        }

        // Carga la imagen de no movimiento
        public void NoMover()
        {
            imagen = imagenQ;
        }

        // Mueve a la izquierda y carga imagen de esa dirección
        public void MoverIzquierda(GameTime gameTime)
        {
            if (X <= 5)
                X = 5;
            else
                X -= VelocX * (float)gameTime.ElapsedGameTime.TotalSeconds;

            imagen = imagenL;
        }

        // Mueve a la derecha y carga imagen de esa direción
        public void MoverDerecha(GameTime gameTime)
        {
            if (X >= 480 - 5 - imagen.Width)
                X = 480 - 5 - imagen.Width;
            else
                X += VelocX * (float)gameTime.ElapsedGameTime.TotalSeconds;

            imagen = imagenR;
        }

        // Mueve arriba
        public void MoverArriba(GameTime gameTime)
        {
            if (Y <= 5)
                Y = 5;
            else
                Y -= VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Mueve abajo
        public void MoverAbajo(GameTime gameTime)
        {
            if (Y >= 640 - 5 - imagen.Height)
                Y = 640 - 5 - imagen.Height;
            else
                Y += VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
