using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SaveEarth
{
    class Sprite
    {
        // Posición X e Y
        public float X { get; set; }
        public float Y { get; set; }

        // Velocidad en X e Y
        public float VelocX { get; set; }
        public float VelocY { get; set; }

        // Visible y chocable para gestionar cuando explota
        public bool Visible { get; set; }
        public bool Chocable { get; set; }

        // Tamaño del sprite
        public int Anchura { get { return imagen.Width; } }
        public int Altura { get { return imagen.Height; } }

        // Imagen, secuencia de animación, tiempo entre fotogramas
        protected Texture2D imagen;
        private Texture2D[][] secuencia;
        private int fotogramaActual;
        private int tiempoFotograma;
        private int tiempoSiguienteFotograma;
        private bool haySecuencia;
        public bool tieneMovimientoLateral;

        // Direcciones de las secuencias de animación
        protected enum Direcciones
        {
            DERECHA, IZQUIERDA,
            ARRIBA, ABAJO, APARECIENDO, DESAPARECIENDO
        };
        private int direccionActual = (int)Direcciones.DERECHA;
        protected int cantidadDeDirecciones = 6;

        // Constructor con content, posición e imagen
        public Sprite(int x, int y, string nombreImagen, 
            ContentManager Content)
        {
            X = x;
            Y = y;
            imagen = Content.Load<Texture2D>(nombreImagen);
            Visible = true;
            Chocable = true;
            haySecuencia = false;
        }

        // Constructor apoyado en sprite previo con posición
        public Sprite(int x, int y, Sprite sprite)
        {
            X = x;
            Y = y;
            imagen = sprite.imagen;
            Visible = true;
            Chocable = true;
            haySecuencia = false;
        }

        // Constructor apoyado en sprite previo con posición y secuencia
        public Sprite(int x, int y, Sprite sprite, bool haySecuencia)
        {
            X = x;
            Y = y;
            Visible = true;
            Chocable = true;
            secuencia = new Texture2D[cantidadDeDirecciones][];
            CargarSecuencia(0, sprite);
            imagen = secuencia[0][0];
            fotogramaActual = 0;
            tiempoFotograma = 200;
            tiempoSiguienteFotograma = 200;
            this.haySecuencia = haySecuencia;
        }

        // Constructor con Content, posición y secuencia
        public Sprite(int x, int y, string[] imagenes,
            ContentManager Content)
        {
            X = x;
            Y = y;
            Visible = true;
            Chocable = true;
            secuencia = new Texture2D[cantidadDeDirecciones][];
            CargarSecuencia(0, imagenes, Content);
            imagen = secuencia[0][0];
            fotogramaActual = 0;
            tiempoFotograma = 200;
            tiempoSiguienteFotograma = 200;
            haySecuencia = true;
        }

        // Cargar secuencia con Content e imágenes
        public void CargarSecuencia(byte direcc, string[] imagenes, 
            ContentManager Content)
        {
            byte tamanyoSecuencia = (byte)imagenes.Length;
            secuencia[direcc] = new Texture2D[tamanyoSecuencia];
            for (int i = 0; i < imagenes.Length; i++)
            {
                secuencia[direcc][i] = Content.Load<Texture2D>(imagenes[i]);
            }
            haySecuencia = true;
            direccionActual = direcc;
        }

        // Cargar secuencia apoyada en sprite previo
        public void CargarSecuencia(byte direcc, Sprite sprite)
        {
            byte tamanyoSecuencia = (byte)sprite.secuencia[direcc].Length;
            secuencia[direcc] = new Texture2D[tamanyoSecuencia];
            secuencia[direcc] = sprite.secuencia[direcc];
            haySecuencia = true;
            direccionActual = direcc;
        }

        // Cambia la secuencia de animación al cambiar la dirección
        public void CambiarDireccion(byte nuevaDir)
        {
             if (haySecuencia)
            {
                if (direccionActual != nuevaDir)
                {
                    direccionActual = nuevaDir;
                    fotogramaActual = 0;
                    imagen = secuencia[direccionActual][0];
                }
            }
        }

        // Cambiar la velocidad
        public void SetVelocidad(float vx, float vy)
        {
            VelocX = vx;
            VelocY = vy;
        }
        
        // Dibuja el Sprite
        public void Dibujar(SpriteBatch spriteBatch)
        {
            if (Visible)
                spriteBatch.Draw(imagen,
                        new Rectangle( (int)X, (int)Y,
                            imagen.Width, imagen.Height),
                        Color.White);
        }

        // Comprueba colisión con otro Sprite
        public bool ColisionaCon(Sprite otro)
        {
            if (!Chocable) return false;
            if (!otro.Chocable) return false;

            Rectangle r1 = new Rectangle(
                (int)X, (int)Y,
                imagen.Width, imagen.Height);
            Rectangle r2 = new Rectangle(
                (int)otro.X, (int)otro.Y,
                otro.imagen.Width, otro.imagen.Height);

            return r1.Intersects(r2);
        }

        // Carga la explosión
        public void Explotar()
        {
            CambiarDireccion((byte)Direcciones.DESAPARECIENDO);
            Chocable = false;
        }

        // Mueve la animación de la secuencia actual
        public virtual void Mover(GameTime gameTime)
        {
            if (haySecuencia)
            {
                int cantidadFotogramas = secuencia[direccionActual].Length;
                tiempoSiguienteFotograma -= 
                    gameTime.ElapsedGameTime.Milliseconds;
                if (tiempoSiguienteFotograma <= 0)
                {
                    fotogramaActual++;
                    if (fotogramaActual >= cantidadFotogramas)
                    {
                        if (direccionActual == (byte)Direcciones.DESAPARECIENDO)
                            Visible = false;
                        
                        fotogramaActual = 0;
                    }
                    tiempoSiguienteFotograma = tiempoFotograma;
                    imagen = secuencia[direccionActual][fotogramaActual];
                }
            }
        }
    }
}
