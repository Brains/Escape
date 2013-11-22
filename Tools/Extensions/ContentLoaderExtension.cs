using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content;
using Microsoft.Xna.Framework.Content;

namespace Tools.Extensions
{
    public static class ContentLoaderExtension
    {
        public static Dictionary<string, T> LoadContentFolder<T> (this ContentManager contentManager, String contentFolder)
        {
            // Init the resulting list
            Dictionary<String, T> result = new Dictionary<String, T> ();
            LoadManual (contentManager, contentFolder, result);

            return result;

//            // Load directory info, abort if none
//            DirectoryInfo dir = new DirectoryInfo (contentManager.RootDirectory + "/" + contentFolder);
//            if (!dir.Exists)
//                throw new DirectoryNotFoundException ();
//
//            // Load all files that matches the file filter
//            FileInfo[] files = dir.GetFiles ("*.*");
//            foreach (FileInfo file in files)
//            {
//                string key = Path.GetFileNameWithoutExtension (file.Name);
//
//                result[key] = contentManager.Load<T> (contentFolder + "/" + key);
//            }
//
//            // Return the result
//            return result;
        }

        //------------------------------------------------------------------
        private static void LoadManual <T> (ContentManager contentManager, string contentFolder, Dictionary <string, T> result)
        {
            string key = "";

            key = "Acceleration";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Blinker";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Brake";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Car (Heavy)";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Car (Light)";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Car (Medium)";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Explosion";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Player";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Police";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
            key = "Road";
            result[key] = contentManager.Load <T> (contentFolder + "/" + key);
        }
    }
}