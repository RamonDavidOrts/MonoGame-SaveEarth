using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace SaveEarth
{
    public class PantallaDeJuego
    {
        // Constantes de tiempo entre disparos, entre oleadas de enemigos 
        // y el tamaño de la pantalla
        private const int TIEMPO_ENTRE_DISPAROS = 300;
        private const int TIEMPO_ENTRE_OLEADAS = 4000;
        private const int ANCHO_PANTALLA = GestorDePantallas.ANCHO_PANTALLA;
        private const int ALTO_PANTALLA = GestorDePantallas.ALTO_PANTALLA;

        // Nave, disparo de nave, lista de disparos, control tiempo 
        // entre disparos e iconos para representar las vidas
        private Nave nave;
        private Disparo disparo;
        private List<Disparo> disparos;
        private DateTime instanteUltimoDisparo;
        private Texture2D iconoNave;

        // Enemigos, disparos enemigos, lista con oleada de enemigos, 
        // control de tiempo entre oleadas, control tiempo entre disparos
        private Enemigo enemigo;
        private Enemigo enemigo2;
        private List<Enemigo> enemigos;  
        private DateTime instanteUltimaOleada;
        private int oleada;
        private DisparoEnemigo disparoEnemigo;
        private DisparoEnemigo disparoEnemigo2;
        private List<DisparoEnemigo> disparosEnemigos;
        private int tiempoSiguienteDisparo;
        private int tiempoMinimoDisparo;
        private int tiempoMaximoDisparo;

        // Fondo y elementos de fondo
        private Fondo fondo;
        private ElementoFondo nebulosa1;
        private ElementoFondo nebulosa2;
        private ElementoFondo planeta;

        // Música , fuente y efectos de sonido
        public Song musicaJuego;
        private SpriteFont fuentePressStart2P;
        private SoundEffect disparoNave;
        private SoundEffect sonidoDisparoEnemigo;
        private SoundEffect explosionNave;
        private SoundEffect explosionEnemigo;

        // Puntuación más alta, puntos, vidas
        private string highScore;
        public int Puntos { get; set; }
        private int vidas;

        // Vuelta a bienvenida
        public bool Terminado { get; set; }

        // Constructor
        public PantallaDeJuego()
        {
            tiempoMinimoDisparo = 500;
            tiempoMaximoDisparo = 1000;

            instanteUltimoDisparo = DateTime.Now;

            Terminado = false;
        }

        // Carga oleada de enemigos
        protected void CargarEnemigos()
        {
            // Número de enemigos aleatorio entre 2 y 6
            int numEnemigos = new Random().Next(2, 7);
            int aleatorioEnemigo = 1;
            oleada++;

            // Al pasar de la tercera oleada 1/3 de posibilidades que
            // aparezcan los enemigos de doble disparo
            if (oleada > 3)
                aleatorioEnemigo = new Random().Next(1, 4);
            
            // Dependiendo de la oleada los enemigos tendrán 
            // tipos de movimiento diferentes
            for (int i = 0; i < numEnemigos; i++)
            {
                Enemigo e = new Enemigo(0, 0, enemigo);
                if (aleatorioEnemigo == 3)
                {
                    e = new Enemigo(0, 0, enemigo2);
                    e.disparoDoble = true;
                }

                if (oleada % 2 != 0)
                {
                    int puntoPantalla = ANCHO_PANTALLA / (numEnemigos + 1);
                    e.Y = 0 - enemigo.Altura - 10;
                    e.X = puntoPantalla * (i + 1);
                }
                else
                {
                    if (oleada % 4 == 0)
                    {
                        e.Y = -30 - (enemigo.Altura + 10) * i;
                        e.X = 240 + (enemigo.Anchura + 10) * i;
                        e.VelocX = -240;
                    }
                    else
                    { 
                        e.Y = 0 - (enemigo.Altura + 10) * i;
                        e.X = 240 - (enemigo.Anchura + 10) * i;
                        e.VelocX = 240;
                    }
                    e.tieneMovimientoLateral = true;
                    
                }
                
                enemigos.Add(e);
            }

            instanteUltimaOleada = DateTime.Now;
        }

        // Carga la nave después de una colisión
        protected void CargarNave()
        {
            nave.X = 203;
            nave.Y = 600;
            nave.Visible = true;
            nave.Chocable = true;
            nave.CambiarDireccion(0);
        }

        // Reproduce música del juego
        public void ReproducirMusica(ContentManager Content)
        {
            musicaJuego = Content.Load<Song>("nivel1");
            MediaPlayer.Play(musicaJuego);
        }

        // Carga Contenidos y resetea valores de inicio
        public void CargarContenidos(ContentManager Content, string highScore)
        {
            this.highScore = highScore;
            Puntos = 0;
            vidas = 3;
            fuentePressStart2P = Content.Load<SpriteFont>("PressStart2P");
            
            fondo = new Fondo(Content);
            nebulosa1 = new ElementoFondo("nebulosas", Content);
            nebulosa2 = new ElementoFondo("nebulosas", Content);
            planeta = new ElementoFondo("planeta", Content);

            iconoNave = Content.Load<Texture2D>("nave");
            nave = new Nave(Content);
            disparoNave = Content.Load<SoundEffect>("disparoNave");
            explosionNave = Content.Load<SoundEffect>("explosion_nave");
            disparo = new Disparo(Content);
            disparos = new List<Disparo>();
            disparo.Visible = false;

            enemigo = new Enemigo(Content);
            enemigo2 = new Enemigo(Content, true);
            disparoEnemigo = new DisparoEnemigo(Content);
            disparoEnemigo2 = new DisparoEnemigo(Content);
            disparoEnemigo2.CambiaImagen(Content, "fireEn2");
            sonidoDisparoEnemigo = Content.Load<SoundEffect>("disparoEnemigo");
            explosionEnemigo = Content.Load<SoundEffect>("explosion_enemigo");
            disparosEnemigos = new List<DisparoEnemigo>();
            tiempoSiguienteDisparo = new Random().Next(1000, 2000);
            enemigos = new List<Enemigo>();
            oleada = 0;
        }

        // Mueve elementos, comprueba colisiones y las teclas pulsadas
        public void Actualizar(GameTime gameTime)
        {
            MoverElementos(gameTime);
            ComprobarColisiones();
            ComprobarEntrada(gameTime);
        }

        // Mueve elementos de fondo, animación de nave, enemigos y disparos
        protected void MoverElementos(GameTime gameTime)
        {
            fondo.Mover(gameTime);
            planeta.Mover(gameTime);
            nebulosa1.Mover(gameTime);
            nebulosa2.Mover(gameTime);

            nave.Mover(gameTime);


            // Mover enemigos
            for (int i = 0; i < enemigos.Count; i++)
            {  
                if (enemigos[i].Chocable)
                {
                    enemigos[i].MoverAbajo(gameTime);
                    if (enemigos[i].tieneMovimientoLateral)
                        enemigos[i].MoverLateral(gameTime);
                }

                enemigos[i].Mover(gameTime);

                if ((enemigos[i].X > ANCHO_PANTALLA - 10 - enemigos[i].Anchura)
                    || (enemigos[i].X < 10))
                    enemigos[i].InvertirSentido();

                if ( ! nave.Visible)
                {
                    enemigos[i].VelocY *= 3;
                    enemigos[i].VelocX *= 3;
                }

                if (enemigos[i].Y >= ALTO_PANTALLA || !enemigos[i].Visible)
                {
                    enemigos.RemoveAt(i);
                    i--;
                }
            }

            // Si ha pasado el tiempo entre oleadas carga otra
            TimeSpan tiempoAnteriorOleada = DateTime.Now - 
                instanteUltimaOleada;
            if ( tiempoAnteriorOleada.TotalMilliseconds >= 
                TIEMPO_ENTRE_OLEADAS && nave.Visible )
                CargarEnemigos();

            // Si la nave ha explotado y no hay enemigos carga la nave
            if (!nave.Visible && enemigos.Count == 0)
                CargarNave();

            // Si no hay enemigos carga oleada
            if (enemigos.Count == 0)
                CargarEnemigos();


            // Mover disparos de la nave
            for (int i = 0; i < disparos.Count; i++)
            {
                disparos[i].Mover(gameTime);
                if (disparos[i].Y < 0)
                {
                    disparos.RemoveAt(i);
                    i--;
                }
            }

            // Mover disparos Enemigos
            for (int i = 0; i < disparosEnemigos.Count; i++)
            {
                disparosEnemigos[i].Mover(gameTime);

                if (!nave.Visible)
                    disparosEnemigos[i].VelocY *= 3;

                if (disparosEnemigos[i].Y > ALTO_PANTALLA)
                {
                    disparosEnemigos.RemoveAt(i);
                    i--;
                }
            }

            // Si ha pasado el tiempo entre disparos carga otro 
            // a una nave activa y dentro del area visible
            tiempoSiguienteDisparo -=
                    gameTime.ElapsedGameTime.Milliseconds;
            if (tiempoSiguienteDisparo <= 0 && enemigos.Count > 0)
            {
                int numeroEnemigo =
                        new Random().Next(enemigos.Count);

                if ( enemigos[numeroEnemigo].Visible
                    && enemigos[numeroEnemigo].Chocable
                    && enemigos[numeroEnemigo].Y > 0 )
                {
                    if (enemigos[numeroEnemigo].disparoDoble == true)
                    {
                        DisparoEnemigo de1 = new DisparoEnemigo(
                        (int)enemigos[numeroEnemigo].X + 7,
                        (int)enemigos[numeroEnemigo].Y + 10,
                        disparoEnemigo2);

                        DisparoEnemigo de2 = new DisparoEnemigo(
                        (int)enemigos[numeroEnemigo].X + 15,
                        (int)enemigos[numeroEnemigo].Y + 10,
                        disparoEnemigo2);

                        disparosEnemigos.Add(de1);
                        disparosEnemigos.Add(de2);
                    }
                    else
                    {
                        DisparoEnemigo de = new DisparoEnemigo(
                            (int)enemigos[numeroEnemigo].X + 6,
                            (int)enemigos[numeroEnemigo].Y + 10,
                            disparoEnemigo);

                        disparosEnemigos.Add(de);
                        sonidoDisparoEnemigo.CreateInstance().Play();
                    }

                    tiempoSiguienteDisparo =
                                new Random().Next(tiempoMinimoDisparo,
                                    tiempoMaximoDisparo);
                }
            }
        }


        // Comprueba teclas pulsadas
        protected void ComprobarEntrada(GameTime gameTime)
        {
            var estadoTeclado = Keyboard.GetState();
            var estadoGamePad = GamePad.GetState(PlayerIndex.One);

            // Salir del juego
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Terminado = true;

            // Acceso rápido a puntuación
            if (estadoTeclado.IsKeyDown(Keys.LeftControl) &&
                estadoTeclado.IsKeyDown(Keys.P))
            {
                Puntos = 1800;
                Terminado = true;
            }

            // Mover nave y disparar
            if (nave.Chocable)
            {
                if (!estadoTeclado.IsKeyDown(Keys.Left) &&
                    estadoGamePad.DPad.Left == 0 &&
                    estadoGamePad.DPad.Right == 0 &&
                    estadoGamePad.ThumbSticks.Left.X == 0 &&
                    !estadoTeclado.IsKeyDown(Keys.Left))
                {
                    nave.NoMover();
                }
                if (estadoTeclado.IsKeyDown(Keys.Left) ||
                    estadoGamePad.DPad.Left > 0 ||
                    estadoGamePad.ThumbSticks.Left.X < 0)
                {
                    nave.MoverIzquierda(gameTime);
                }
                if (estadoTeclado.IsKeyDown(Keys.Right) ||
                    estadoGamePad.DPad.Right > 0 ||
                    estadoGamePad.ThumbSticks.Left.X > 0)
                {
                    nave.MoverDerecha(gameTime);
                }
                if (estadoTeclado.IsKeyDown(Keys.Up) ||
                    estadoGamePad.DPad.Up > 0 ||
                    estadoGamePad.ThumbSticks.Left.Y > 0)
                {
                    nave.MoverArriba(gameTime);
                }
                if (estadoTeclado.IsKeyDown(Keys.Down) ||
                    estadoGamePad.DPad.Down > 0 ||
                    estadoGamePad.ThumbSticks.Left.Y < 0)
                {
                    nave.MoverAbajo(gameTime);
                }
                if (estadoTeclado.IsKeyDown(Keys.Space) ||
                    estadoGamePad.Buttons.A > 0)
                {
                    Disparar();
                }
            }
        }

        // Si ha pasado el tiempo entre disparos de la nave 
        // carga un doble disparo
        private void Disparar()
        {
            TimeSpan tiempoTranscurrido = DateTime.Now - instanteUltimoDisparo;
            if (tiempoTranscurrido.TotalMilliseconds >= TIEMPO_ENTRE_DISPAROS)
            {
                instanteUltimoDisparo = DateTime.Now;
                if (disparos.Count < 10)
                {
                    Disparo d1 = new Disparo(
                        (int)nave.X + 4,
                        (int)nave.Y - 8,
                        disparo);
                    Disparo d2 = new Disparo(
                        (int)nave.X + 20,
                        (int)nave.Y - 8,
                        disparo);
                    disparos.Add(d1);
                    disparos.Add(d2);
                    disparoNave.CreateInstance().Play();
                }
            }
        }

        // Reproduce la animación y sonido de explosión
        // de la nave y resta una vida
        protected void ExplotaNave()
        {
            explosionNave.CreateInstance().Play();
            nave.Explotar();
            vidas--;
            if (vidas <= 0)
                Terminado = true;
        }

        // Comprueba las colisiones del juego
        protected void ComprobarColisiones()
        {
            // Disparo enemigo con nave
            for (int i = 0; i < disparosEnemigos.Count; i++)
            {
                if (disparosEnemigos[i].ColisionaCon(nave))
                {
                    ExplotaNave();
                    disparosEnemigos.RemoveAt(i);
                }
            }
            

            // Disparo con enemigo
            for (int i = 0; i < enemigos.Count; i++)
            {
                for (int j = 0; j < disparos.Count; j++)
                {
                    if (disparos[j].ColisionaCon(enemigos[i]) 
                        && enemigos[i].Y > 0)
                    {
                        explosionEnemigo.CreateInstance().Play();
                        enemigos[i].Explotar();
                        disparos.RemoveAt(j);
                        Puntos += 10;
                    }
                }

                // Nave con enemigos
                if (nave.ColisionaCon(enemigos[i]))
                {
                    ExplotaNave();
                    enemigos[i].Explotar();
                }
            }
        }

        // Dibuja los elementos en la pantalla
        public void Dibujar(SpriteBatch spriteBatch)
        {
            fondo.Dibujar(spriteBatch);
            nebulosa1.Dibujar(spriteBatch);
            nebulosa2.Dibujar(spriteBatch);
            planeta.Dibujar(spriteBatch);

            spriteBatch.DrawString(fuentePressStart2P, "1UP",
                new Vector2(18, 2), Color.White);
            spriteBatch.DrawString(fuentePressStart2P, 
                String.Format("{0:00000}", Puntos),
                new Vector2(2, 18), Color.White);
            spriteBatch.DrawString(fuentePressStart2P, "HIGH SCORE",
                new Vector2(161, 2), Color.Yellow);
            spriteBatch.DrawString(fuentePressStart2P, 
                String.Format("{0:00000}", highScore),
                new Vector2(200, 18), Color.White);

            for (int i = 0; i < vidas; i++)
            {
                spriteBatch.Draw(iconoNave,
                            new Rectangle(10 + 20 * i, 620,
                                15, 15),
                            Color.White);
            }

            nave.Dibujar(spriteBatch);

            foreach (Enemigo enemigo in enemigos)
            {
                enemigo.Dibujar(spriteBatch);
            }

            for (int i = 0; i < disparos.Count; i++)
            {
                disparos[i].Dibujar(spriteBatch);
            }

            for (int i = 0; i < disparosEnemigos.Count; i++)
            {
                disparosEnemigos[i].Dibujar(spriteBatch);
            }

        }
    }
}