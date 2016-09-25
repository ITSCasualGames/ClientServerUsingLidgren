using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NSLoader
{
    static class Loader
    {


        static public Dictionary<String, T> ContentLoad<T>(ContentManager Content, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory +  contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            Dictionary<String, T> result = new Dictionary<String, T>();
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[key] = Content.Load<T>(contentFolder + key);
            }
            return result;
        }

    }
}
