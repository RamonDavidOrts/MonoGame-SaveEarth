using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace SaveEarth
{
    class PantallaDeCreditos
    {
        // fondo y fuente
        private Fondo fondo;
        private SpriteFont fuentePressStart2P;

        // Para controlar la vuelta a bienvenida
        public bool Terminado { get; set; }

        // Constructor
        public PantallaDeCreditos()
        {
            Terminado = false;
        }

        // Carga fondo y fuente
        public void CargarContenidos(ContentManager Content)
        {
            fondo = new Fondo(Content);
            fuentePressStart2P = Content.Load<SpriteFont>("PressStart2P");
        }

        // Comprueba teclas pulsadas y mueve fondo
        public void Actualizar(GameTime gameTime)
        {
            var estadoGamePad = GamePad.GetState(PlayerIndex.One);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) ||
                estadoGamePad.Buttons.Start == ButtonState.Pressed ||
                GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Terminado = true;
            }

            fondo.Mover(gameTime);
        }

       
        // Dibuja fondo y créditos
        public void Dibujar(SpriteBatch spriteBatch)
        {
            fondo.Dibujar(spriteBatch);


            spriteBatch.DrawString(fuentePressStart2P,
                "a game by",
                new Vector2(30, 150), Color.Yellow);
            spriteBatch.DrawString(fuentePressStart2P,
                "Ramon David Orts",
                new Vector2(30, 170), Color.White);

            spriteBatch.DrawString(fuentePressStart2P,
                "graphics by",
                new Vector2(30, 210), Color.Yellow);
            spriteBatch.DrawString(fuentePressStart2P,
                "Ansimuz (space backgrounds)",
                new Vector2(30, 230), Color.White);
            spriteBatch.DrawString(fuentePressStart2P,
                "davexunit (spacehips)",
                new Vector2(30, 250), Color.White);


            spriteBatch.DrawString(fuentePressStart2P,
                "music by",
                new Vector2(30, 290), Color.Yellow);
            spriteBatch.DrawString(fuentePressStart2P,
                "yd (intro)",
                new Vector2(30, 310), Color.White);
            spriteBatch.DrawString(fuentePressStart2P,
                "FoxSynergy (game)",
                new Vector2(30, 330), Color.White);

            spriteBatch.DrawString(fuentePressStart2P,
                "sound effects by",
                new Vector2(30, 370), Color.Yellow);
            spriteBatch.DrawString(fuentePressStart2P,
                "Little Robot Sound Factory",
                new Vector2(30, 390), Color.White);
        }
    }
}
