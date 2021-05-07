// ORTS FERNÁNDEZ, RAMÓN DAVID
using System;

namespace SaveEarth
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new GestorDePantallas())
                game.Run();
        }
    }
#endif
}
