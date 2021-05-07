using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;

namespace SaveEarth
{
    class PantallaDeBienvenida
    {
        // Tiempo de cambio entre opciones de menu y ranking (5000 cada uno)
        private const int TIEMPO_CAMBIO_PANTALLA = 10000;

        private DateTime ultimoCambioPantalla;

        // Fondo, fuentes de letra y música
        private Fondo fondo;
        private SpriteFont fuenteVerminBig;
        private SpriteFont fuenteVerminSmall;
        private SpriteFont fuentePressStart2P;
        private Song musicaMenu;

        // Gestor de pantalla
        private GestorDePantallas gestor;
        
        // Para controlar el alpha en efecto parpadeo en el menú
        private bool subeAlpha = true;
        private float alpha = 0;

        // Salida a juego o a créditos
        public bool Terminado { get; set; }
        public bool Creditos { get; set; }

        // Tabla de puntuaciones que cargará desde el archivo
        public string[][] tablaPuntuaciones =
        { 
            new string[] {"1st", "00000", "---"},
            new string[] {"2nd", "00000", "---"},
            new string[] {"3rd", "00000", "---"},
            new string[] {"4th", "00000", "---"},
            new string[] {"5th", "00000", "---"},
            new string[] {"6th", "00000", "---"},
            new string[] {"7th", "00000", "---"},
            new string[] {"8th", "00000", "---"},
            new string[] {"9th", "00000", "---"},
            new string[] {"10th", "00000", "---"},
        };
        private string file = "scores.dat";
        private string error = "";

        // Constructor, carga gestor y comienza ciclo de menú a puntuaciones
        public PantallaDeBienvenida(GestorDePantallas gestor)
        {
            this.gestor = gestor;
            Terminado = false;
            Creditos = false;
            ultimoCambioPantalla = DateTime.Now;
        }

        // Reproduce la música
        public void ReproducirMusica(ContentManager Content)
        {
            musicaMenu = Content.Load<Song>("menu");
            MediaPlayer.Play(musicaMenu);
        }

        // Carga el archivo de ranking
        public void LeerFicheroScores()
        {
            if (File.Exists(file))
            {
                try
                {
                    StreamReader entrada = File.OpenText(file);
                    string linea;
                    string[][] ficheroPuntuaciones = new string[10][];
                    int numLinea = 0;

                    do
                    {
                        linea = entrada.ReadLine();
                        if (linea != null)
                        {
                            if (linea.Trim() != "")
                            {
                                ficheroPuntuaciones[numLinea] =
                                    linea.Split(' ');
                                numLinea++;
                            }
                        }
                    }
                    while (linea != null);

                    entrada.Close();
                    tablaPuntuaciones = ficheroPuntuaciones;
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
            }
            else
                error = "Score file not found";
        }

        // Escribe el archivo de ranking
        public void EscribirFicheroScores()
        {
            try
            {
                StreamWriter salida = File.CreateText(file);

                for (int i = 0; i < tablaPuntuaciones.Length; i++)
                {
                    salida.WriteLine(tablaPuntuaciones[i][0] + " " +
                        tablaPuntuaciones[i][1] + " " +
                        tablaPuntuaciones[i][2]);
                }

                salida.Close();
            }
            catch (Exception e)
            {
                error = e.Message;
            }
        }

        // Carga Contenidos, lee ranking y reproduce música
        public void CargarContenidos(ContentManager Content)
        {
            LeerFicheroScores();
            ReproducirMusica(Content);
            fondo = new Fondo(Content);
            fuentePressStart2P = Content.Load<SpriteFont>("PressStart2P");
            fuenteVerminBig = Content.Load<SpriteFont>("VerminBig");
            fuenteVerminSmall = Content.Load<SpriteFont>("VerminSmall");
        }

        // Comprueba teclas pulsadas del menú, mueve fondo y el alpha
        public void Actualizar(GameTime gameTime)
        {
            var estadoGamePad = GamePad.GetState(PlayerIndex.One);

            if (Keyboard.GetState().IsKeyDown(Keys.D1) ||
                    estadoGamePad.Buttons.Start == ButtonState.Pressed)
            {
                Terminado = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                gestor.Terminar();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                Creditos = true;
            }


            fondo.Mover(gameTime);

            if (alpha <= 0)
                subeAlpha = true;
            if (alpha >= 1)
                subeAlpha = false;

            if (subeAlpha)
                alpha += 0.03f;
            else
                alpha -= 0.03f;
        }

        // Dibuja fondo, y el menú o ranking dependiendo del momento
        public void Dibujar(SpriteBatch spriteBatch)
        {
            fondo.Dibujar(spriteBatch);

            spriteBatch.DrawString(fuentePressStart2P,
                error,
                new Vector2(0, 0), Color.White);

            spriteBatch.DrawString(fuenteVerminBig, "SAVE",
                new Vector2(168, 50), Color.Yellow);
            spriteBatch.DrawString(fuenteVerminBig, "THE EARTH",
                new Vector2(85, 90), Color.Yellow);
            spriteBatch.DrawString(fuenteVerminSmall, 
                "FROM THOSE ALIEN BASTARDS",
                new Vector2(137, 145), Color.White);

            TimeSpan tiempotranscurrido = DateTime.Now - ultimoCambioPantalla;

            if (tiempotranscurrido.TotalMilliseconds > 
                TIEMPO_CAMBIO_PANTALLA / 2)
            {
                spriteBatch.DrawString(fuentePressStart2P,
                "TOP 10 RANKING SCORES",
                new Vector2(70, 235), Color.White);

                int fila, columna = 0;
                for (int i = 0; i < tablaPuntuaciones.Length; i++)
                {
                    fila = 275 + (25 * i);
                    for (int j = 0; j < tablaPuntuaciones[i].Length; j++)
                    {
                        switch (j)
                        {
                            case 0: columna = 125; break;
                            case 1: columna = 205; break;
                            case 2: columna = 305; break;
                        }
                        spriteBatch.DrawString(fuentePressStart2P,
                            tablaPuntuaciones[i][j],
                            new Vector2(columna, fila), Color.White);
                    }
                }
            }

            if (tiempotranscurrido.TotalMilliseconds <=
                TIEMPO_CAMBIO_PANTALLA / 2)
            {
                spriteBatch.DrawString(fuentePressStart2P,
                "Press 1 or Start to Play",
                new Vector2(49, 270), Color.White * alpha);

                spriteBatch.DrawString(fuentePressStart2P,
                    "Press E to Exit",
                    new Vector2(120, 310), Color.White);

                spriteBatch.DrawString(fuentePressStart2P,
                    "Press C to Credits",
                    new Vector2(95, 350), Color.White);

                spriteBatch.DrawString(fuentePressStart2P,
                    "Press S for Scanlines",
                    new Vector2(72, 390), Color.White);
            }

            if (tiempotranscurrido.TotalMilliseconds >
                TIEMPO_CAMBIO_PANTALLA)
            {
                ultimoCambioPantalla = DateTime.Now;
            }
        }
    }
}