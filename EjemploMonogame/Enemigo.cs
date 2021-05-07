
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SaveEarth
{
    class Enemigo : Sprite
    {
        // Indica si la nave enemiga tiene disparo doble
        public bool disparoDoble = false;

        // Constructor con Content, carga la secuencia de animación
        public Enemigo(ContentManager Content)
            : base(0, 0, new string[] { "enemigo1", "enemigo1b" }, Content)
        {
            SetVelocidad(120, 120);
            CargarSecuencia((byte)Direcciones.DESAPARECIENDO,
                new string[] { "explosion1", "explosion2", "explosion3",
                    "explosion4" }, Content);
            CambiarDireccion(0);
        }

        // Constructor con Content para nave grande con disparo doble
        public Enemigo(ContentManager Content, bool disparoDoble)
            : base(0, 0, new string[] { "enemigo2", "enemigo2b" }, Content)
        {
            SetVelocidad(120, 120);
            CargarSecuencia((byte)Direcciones.DESAPARECIENDO,
                new string[] { "explosion1-2", "explosion2", "explosion3",
                    "explosion4" }, Content);
            CambiarDireccion(0);
            this.disparoDoble = disparoDoble;
        }

        // Constructor apoyado en instancia previa
        public Enemigo(int x, int y, Enemigo enemigo)
            : base(x, y, enemigo, true)
        {
            SetVelocidad(120, 120);
            CargarSecuencia((byte)Direcciones.DESAPARECIENDO, enemigo);
            CambiarDireccion(0);
        }

        // Invierte el sentido en X 
        public void InvertirSentido()
        {
            VelocX = -VelocX;
        }

        // Mueve lateralmente la nave
        public void MoverLateral(GameTime gameTime)
        {
            X += VelocX * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Mueva abajo la nave
        public void MoverAbajo(GameTime gameTime)
        {
            Y += VelocY * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
