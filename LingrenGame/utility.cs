using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities
{
    

    static public class Utility
    {
        static private Dictionary<string, Texture2D> _playerTextures = new Dictionary<string, Texture2D>();
        static Random rand = new Random();

        public static Dictionary<string, Texture2D> PlayerTextures
        {
            get
            {
                return _playerTextures;
            }

            set
            {
                _playerTextures = value;
            }
        }

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
