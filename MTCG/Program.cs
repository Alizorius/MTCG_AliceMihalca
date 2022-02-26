using System;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new HttpServer();
            x.Start();
            Console.ReadLine();
            x.Stop();
        }
    }
}
