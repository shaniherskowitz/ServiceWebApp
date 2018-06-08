using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using ServiceWebApp.connection;

namespace ServiceWebApp.Models
{
    public class Model
    {
        Connect c = Connect.Instance;

        /// <summary>
        /// Creates the model
        /// </summary>
        public Model()
        {

        }

        /// <summary>
        /// Sets the configuration from the service
        /// </summary>
        /// <return List of strings - info of config></return>
        public IList<string> GetConfig()
        {
            string set = c.WriteConnection("1");
            return set.Split('*').Reverse().ToList<string>();

        }

        /// <summary>
        /// Gets the logs from the service
        /// </summary>
        /// <return list of log info></return>
        public string GetLogs()
        {
            string info = c.WriteConnection("2");
            return info;
        }

        /// <summary>
        /// Removes the hanlder from the service 
        /// </summary>
        /// <param string="path"></param>
        /// <return bool - false or true></return>
        public bool RemoveHandler(string path)
        {
            string result = c.WriteConnection("5, " + path);
            if (result.Equals("2 " + path)) return true;
            return false;
        }

        /// <summary>
        /// Sees if the service is running 
        /// </summary>
        /// <return bool false or true></return>
        public bool isRunning()
        {
            try
            {
                using (ServiceController sc = new ServiceController("ImageService"))
                {
                    return sc.Status == ServiceControllerStatus.Running;
                }
            }
            catch (ArgumentException) { return false; }
            catch (Win32Exception) { return false; }
        }

        /// <summary>
        /// Gets the images from the folder
        /// </summary>
        /// <return a list of the paths of the images></return>
        public IList<string> GetImage()
        {
            char sep_char = Path.DirectorySeparatorChar;
            IList<string> MyImgList = new List<string>();
            string YourDir = @"C:\Users\IEUser\source\repos\ServiceWebApp\ServiceWebApp\OutputDir\";
            DirectoryInfo di = new DirectoryInfo(YourDir);
            var images = Directory.GetFiles(YourDir, "*.jpg", SearchOption.AllDirectories);
            foreach (string file in images)
            {
                MyImgList.Add(file);
            }
            return MyImgList;
        }
    }
}