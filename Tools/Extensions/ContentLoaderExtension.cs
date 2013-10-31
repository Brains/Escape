using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Tools.Extensions
{
    public static class ContentLoaderExtension
    {
        public static Dictionary<string, T> LoadContentFolder<T> (this ContentManager contentManager, String contentFolder)
        {
            // Load directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo (contentManager.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException ();

            // Init the resulting list
            Dictionary<String, T> result = new Dictionary<String, T> ();

            // Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles ("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension (file.Name);

                result[key] = contentManager.Load<T> (contentFolder + "/" + key);
            }

            // Return the result
            return result;
        }
    }
}