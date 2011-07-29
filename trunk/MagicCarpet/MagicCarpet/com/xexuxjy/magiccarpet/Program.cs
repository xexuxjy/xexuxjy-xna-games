using System;

namespace MagicCarpet
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MagicCarpet game = new MagicCarpet())
            {
                game.Run();
            }
        }
    }
#endif
}

