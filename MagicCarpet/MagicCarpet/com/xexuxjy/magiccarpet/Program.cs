using System;

namespace com.xexuxjy.magiccarpet
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Globals.Game = new MagicCarpet())
            {
                Globals.Game.Run();
            }
        }
    }
#endif
}

