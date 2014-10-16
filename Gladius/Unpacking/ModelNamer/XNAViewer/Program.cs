#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace XNAViewer
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
            //var game = new Game1();
            using (var game = new XNAPointViewer())
            {
                game.Run();
            }
        }
    }
#endif
}
