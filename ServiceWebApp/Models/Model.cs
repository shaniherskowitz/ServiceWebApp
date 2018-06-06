using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ServiceWebApp.connection;

namespace ServiceWebApp.Models
{
    public class Model
    {
        Connect c = Connect.Instance;

        public Model()
        {




        }

        public IList<string> GetConfig()
        {
            string set = c.WriteConnection("1");
            return set.Split('*').Reverse().ToList<string>();

        }

        public string GetLogs()
        {
            string info = c.WriteConnection("2");
            return info;
        }

        public bool RemoveHandler(string path)
        {
            string result = c.WriteConnection("5, " + path);
            if (result.Equals("2" + path)) return true;
            return false;
        }

        public IList<string> GetImage()
        {
            char sep_char = Path.DirectorySeparatorChar;
            List<string> MyImgList = new List<string>();
            string YourDir = "C:" + sep_char + "Users" + sep_char + "IEUser" + sep_char + "Desktop" + sep_char + "OutputDir ";
            DirectoryInfo di = new DirectoryInfo(YourDir);
            var images = Directory.GetFiles(YourDir, "*.thumb", SearchOption.AllDirectories);
            foreach (string file in images)
            {
                MyImgList.Add(file);
            }
            return MyImgList;
        }
    }
}