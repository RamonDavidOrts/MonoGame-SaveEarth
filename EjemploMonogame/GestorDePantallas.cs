// ORTS FERNÁNDEZ, RAMÓN DAVID

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace SaveEarth
{
    
    public class GestorDePantallas : Game
    {
        // Constantes con las medidas de la pantalla
        public const int ANCHO_PANTALLA = 480;
        public const int ALTO_PANTALLA = 640;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Las pantallas del juego
        private PantallaDeBienvenida bienvenida;
        private PantallaDeJuego juego;
        private PantallaDePuntuacion puntuacion;
        private PantallaDeCreditos creditos;

        // La puntuación más alta del ranking 
        private string highScore;

        // Imagen para simular scanlines
        private Sprite scanlines;
        private bool mostrarScanlines;

        // Para saber si una tecla ha sido pulsada
        private bool pulsada;
        
        // Para cambiar entre las distintas pantallas del juego
        public enum MODO { BIENVENIDA, JUEGO, PUNTUACION, CREDITOS};
        public MODO ModoActual { get; set; }

        // Propiedad estática con el contenido para hacerlo más accesible
        public static ContentManager Contenido { get; set; }


        // Constructor carga el content, tamaño de pantalla 
        // e instancia las pantallas
        public GestorDePantallas()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = ANCHO_PANTALLA;
            graphics.PreferredBackBufferHeight = ALTO_PANTALLA;
            graphics.ApplyChanges();
            bienvenida = new PantallaDeBienvenida(this);
            juego = new PantallaDeJuego();
            puntuacion = new PantallaDePuntuacion();
            creditos = new PantallaDeCreditos();
            mostrarScanlines = true;

            Contenido = Content;
        }
        
        // Carga el contenido de las pantallas, 
        // el ranking más alto desde la tabla e inicia la música
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bienvenida.CargarContenidos(Content);
            highScore = bienvenida.tablaPuntuaciones[0][1];
            juego.CargarContenidos(Content, highScore);
            MediaPlayer.IsRepeating = true;

            scanlines = new Sprite(0, 0, "scanlines", Content);
        }

        // Comprueba en que pantalla está el juego 
        protected override void Update(GameTime gameTime)
        {
            // Comprobación de scanlines para todas las pantallas del juego
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                pulsada = true;

            if (pulsada && Keyboard.GetState().IsKeyUp(Keys.S))
            {
                mostrarScanlines = !mostrarScanlines;
                pulsada = false;
            }

            // Actualiza la pantalla del juego en la que estemos
            switch (ModoActual)
            {
                case MODO.JUEGO: 
                    juego.Actualizar(gameTime);
                    break;
                case MODO.BIENVENIDA: 
                    bienvenida.Actualizar(gameTime);
                    break;
                case MODO.PUNTUACION:
                    puntuacion.Actualizar(gameTime);
                    break;
                case MODO.CREDITOS:
                    creditos.Actualizar(gameTime);
                    break;
            }

            // Pasa de bienvenida a créditos y viceversa
            if (bienvenida.Creditos)
            {
                ModoActual = MODO.CREDITOS;
                bienvenida.Creditos = false;
                creditos.CargarContenidos(Content);
            }

            if (creditos.Terminado)
            {
                ModoActual = MODO.BIENVENIDA;
                creditos.Terminado = false;
            }

            // Pasa de bienvenida a juego
            if (bienvenida.Terminado)
            {
                ModoActual = MODO.JUEGO;
                juego.ReproducirMusica(Content);
                bienvenida.Terminado = false;
            }

            // Pasa de juego a bienvenida o a puntuación si los 
            // puntos superan el ranking más bajo
            if (juego.Terminado)
            {
                int minimoPuntuar = 
                    Convert.ToInt32(bienvenida.tablaPuntuaciones[9][1]);

                if (juego.Puntos > minimoPuntuar)
                {
                    puntuacion.CargarContenidos(Content, juego.Puntos, 
                        bienvenida.tablaPuntuaciones);
                    ModoActual = MODO.PUNTUACION;
                }
                else
                    ModoActual = MODO.BIENVENIDA;

                bienvenida.ReproducirMusica(Content);
                juego.Terminado = false;
                juego.CargarContenidos(Content, highScore);
            }

            // Pasa de puntuación a bienvenida y guarda el ranking
            if (puntuacion.Terminado)
            {
                ModoActual = MODO.BIENVENIDA;
                puntuacion.Terminado = false;
                bienvenida.EscribirFicheroScores();
            }

            base.Update(gameTime);
        }

        // Dibuja la pantalla según en la que estemos
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 35));

            spriteBatch.Begin();
            switch (ModoActual)
            {
                case MODO.JUEGO:
                    juego.Dibujar(spriteBatch);
                    break;
                case MODO.BIENVENIDA: 
                    bienvenida.Dibujar(spriteBatch);
                    break;
                case MODO.PUNTUACION:
                    puntuacion.Dibujar(spriteBatch);
                    break;
                case MODO.CREDITOS:
                    creditos.Dibujar(spriteBatch);
                    break;
            }

            if (mostrarScanlines)
                scanlines.Dibujar(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Termina el juego
        public void Terminar()
        {
            Exit();
        }
    }
}