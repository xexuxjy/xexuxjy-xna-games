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
            try
            {
                using (Globals.Game = new MagicCarpet())
                {
                    Globals.Game.Run();
                }
            }
            catch (System.Exception ex)
            {
                String st = ex.StackTrace;
                int ibreak = 0;

            }
        }
    }
#endif
}

