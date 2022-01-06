using System;

namespace MTCG
{
    class Program
    {
        static void Main(string[] args)
        {
            //Monster jack = new Monster("Sir Jack", 2, ElementType.Normal, MonsterType.Knight);
            //Monster mary = new Monster("Lady Mary", 4, ElementType.Water, MonsterType.Dragon);
            //Spell shazam = new Spell("Shazam!", 50, ElementType.Normal);
            //Spell bam = new Spell("BAM!", 35, ElementType.Fire);

            //Deck myDeck = new Deck(jack, mary, shazam, bam);

            var x = new HttpServer();
            x.Start();
            Console.ReadLine();
            x.Stop();
        }
    }
}
