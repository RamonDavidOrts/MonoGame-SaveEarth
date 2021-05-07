using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SaveEarth
{
    class ElementoFondo : Sprite
    {
        private string tipoElemento;

        // Constructor con Content y string con el tipo de elemento
        public ElementoFondo(string elemento, ContentManager Content)
            : base(0, 0, (elemento + "1"), Content)
        {
            tipoElemento = elemento;
            CargarElemento(Content);
        }

        // Carga el elemento aleatoriamente 
        // dependiendo del tipo (planetas o nebulosas)
        public void CargarElemento(ContentManager Content)
        {
            int numAleatorio;

            if (tipoElemento == "planeta")
            {
                numAleatorio = new Random().Next(1, 5);
                VelocY = 70;
                X = new Random().Next( ( - imagen.Width / 4 ), 
                    ( 480 - imagen.Width / 4 ) );
                Y = new Random().Next(-680, -80);
                imagen = Content.Load<Texture2D>(tipoElemento + numAleatorio);
            }

            if (tipoElemento == "nebulosas")
            {
                numAleatorio = new Random().Next(1, 6);
                VelocY = 60;
                X = new Random().Next(0,  ( 480 - imagen.Width ) );
                Y = new Random().Next(-680, -10);
                imagen = Content.Load<Texture2D>(tipoElemento + numAleatorio);
            }
        }

        public override void Mover(GameTime gameTime)
        {
            Y += VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Y >= 640)
            {
                CargarElemento(GestorDePantallas.Contenido);
            }
        }
    }
}