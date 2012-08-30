using System;

namespace MeanPong
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MeanPong game = new MeanPong())
            {
                game.Run();
            }
        }
    }
#endif
}

