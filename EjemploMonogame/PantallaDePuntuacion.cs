using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace SaveEarth
{
    class PantallaDePuntuacion
    {
        // Fondo y fuentes
        private Fondo fondo;
        private SpriteFont fuentePressStart2P;

        // Terminar Entrada en puntuación
        public bool Terminado { get; set; }

        // Entrada de iniciales
        private int contIniciales = 0;
        private Keys[] teclas = new Keys[3];
        private string[] iniciales = { "-", "-", "-" };

        // Puntuación
        private int puntos;
        private string[][] tablaPuntuaciones;
        private int posicionTabla = -1;

        
        public PantallaDePuntuacion()
        {
            Terminado = false;
        }


        // Carga fondo, fuentes los puntos y la tabla de puntuaciones
        public void CargarContenidos(ContentManager Content, int puntos, 
            string[][] tablaPuntuaciones)
        {
            fondo = new Fondo(Content);
            fuentePressStart2P = Content.Load<SpriteFont>("PressStart2P");

            this.puntos = puntos;
            this.tablaPuntuaciones = tablaPuntuaciones;
        }


        // Comprueba las entradas de teclado y mueve el fondo
        public void Actualizar(GameTime gameTime)
        {
            fondo.Mover(gameTime);

            var estadoGamePad = GamePad.GetState(PlayerIndex.One);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) ||
                    estadoGamePad.Buttons.Start == ButtonState.Pressed)
                Terminado = true;


            IntroducirIniciales();
        }


        private void IntroducirIniciales()
        {
            // Encontrar la posición donde van los puntos
            if (posicionTabla == -1)
            {
                int iTabla = tablaPuntuaciones.Length - 1;
                do
                {
                    if (iTabla > 0)
                    {
                        int puntosPos =
                            Convert.ToInt32(tablaPuntuaciones[iTabla - 1][1]);
                        if (puntos > puntosPos)
                        {
                            tablaPuntuaciones[iTabla][1] =
                                tablaPuntuaciones[iTabla - 1][1];
                            tablaPuntuaciones[iTabla][2] =
                                tablaPuntuaciones[iTabla - 1][2];
                        }
                        else
                            posicionTabla = iTabla;
                    }
                    else
                        posicionTabla = iTabla;

                    iTabla--;
                }
                while (iTabla >= 0 && posicionTabla == -1);
            }

            // Introducir la iniciales
            var tecla = Keyboard.GetState().GetPressedKeys();
            if (tecla.Length == 1)
            {
                if (tecla[0] >= Keys.A && tecla[0] <= Keys.Z)
                    teclas[contIniciales] = tecla[0];
                else if (tecla[0] == Keys.Back && contIniciales > 0)
                {
                    if (contIniciales == 2 &&
                        teclas[contIniciales] != Keys.None)
                    {
                        teclas[contIniciales] = Keys.None;
                    }
                    else
                    {
                        contIniciales--;
                        teclas[contIniciales] = Keys.None;
                    }
                }
            }
            // si se ha introducido una letra y 
            // no se está pulsando una tecla avanza de posición
            else
            {
                if (teclas[contIniciales] != Keys.None && contIniciales < 2)
                    contIniciales++;
            }

            //convertir las iniciales pulsadas a string
            for (int i = 0; i < teclas.Length; i++)
            {
                if (teclas[i] != Keys.None)
                    iniciales[i] = teclas[i].ToString();
                else
                    iniciales[i] = "-";
            }

            // introduce las iniciales en la tabla
            tablaPuntuaciones[posicionTabla][1] = 
                String.Format("{0:00000}", puntos);
            tablaPuntuaciones[posicionTabla][2] = string.Join("", iniciales);
            
        }

        // Muestra la posición, puntos y la iniciales que se introducen
        public void Dibujar(SpriteBatch spriteBatch)
        {
            fondo.Dibujar(spriteBatch);


            spriteBatch.DrawString(fuentePressStart2P,
                "ENTER YOUR INITIALS",
                new Vector2(88, 215), Color.White);

            if (posicionTabla > -1)
            {
                spriteBatch.DrawString(fuentePressStart2P,
                tablaPuntuaciones[posicionTabla][0],
                new Vector2(125, 245), Color.White);
            }

            spriteBatch.DrawString(fuentePressStart2P,
                String.Format("{0:00000}", puntos),
                new Vector2(205, 245), Color.White);

            spriteBatch.DrawString(fuentePressStart2P,
                string.Join("", iniciales),
                new Vector2(305, 245), Color.White);

            spriteBatch.DrawString(fuentePressStart2P,
                "PRESS ENTER TO SUBMIT",
                new Vector2(72, 275), Color.White);
        }
    }
}
