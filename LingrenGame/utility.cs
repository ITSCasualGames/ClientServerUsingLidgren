using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    static public class Utility
    {
        static Random rand = new Random();

        public static int NextRandom()
        {
            return rand.Next();
        }

        public static int NextRandom(int min, int max)
        {
            return rand.Next(min,max);
        }
    }
}
