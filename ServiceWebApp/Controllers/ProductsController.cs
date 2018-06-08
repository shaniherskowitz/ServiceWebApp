using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ServiceWebApp.Models;

namespace ServiceWebApp.Controllers
{
    /// <summary>
    /// Creates the messageTypeEnum
    /// </summary>
    public enum MessageTypeEnum : int
    {
        INFO,
        WARNING,
        FAIL
    }

    /// <summary>
    /// Creates the commadnInfo
    /// </summary>
    public class CommandInfo
    {
        public string ID { get; set; }
        public string Details { get; set; }

        public CommandInfo(string iD, string details)
        {
            ID = iD;
            Details = details;
        }
    }
    /// <summary>
    /// Creates the ImageInfo
    /// </summary>
    public class ImageInfo
    {
        public string Path{ get; set; }
        public string image { get; set; }
        public string Name { get; set; }


        /// <summary>
        /// Gets the image info
        /// </summary>
        /// <param string="pth"></param>
        /// <param string="img"></param>
        /// <param string="name"></param>
        public ImageInfo(string pth, string img, string name)
        {
            Path = pth;
            image = img;
            Name = name;
        }
    }

    /// <summary>
    /// Creates the product controller
    /// </summary>
    [RoutePrefix("Products")]
    public class ProductsController : Controller
    {
        Model m = new Model();
        static string Output = "Output Directory: ";
        static string Source = "Source Name: ";
        static string LogName = "Log Name: ";
        static string ThumbName = "Thumbnail Name: ";
        static IList<string> ListPaths = new List<string>();
        private object lockObj = new object();
        public IList<CommandInfo> ListCommands = new List<CommandInfo>();
        public IList<ImageInfo> images = new List<ImageInfo>();

        /// <summary>
        /// For the webimage view
        /// </summary>
        /// <return web image view></return>
        // GET: Products
        public ActionResult Index()
        {
            if (m.isRunning())
            {
                ViewData["running"] = "Yes";

            }
            else ViewData["running"] = "No";

            IList<string> img = m.GetImage();
            ViewData["num"] = img.Count.ToString();
            return View();
        }

        /// <summary>
        /// For the config view
        /// </summary>
        /// <param bool="load"></param>
        /// <return config view></return>
        public ActionResult Config(bool load = true)
        {
            
                SetConfig(load);
                ViewData["outputDir"] = Output;
                ViewData["source"] = Source;
                ViewData["logName"] = LogName;
                ViewData["ThumbName"] = ThumbName;
                ViewData["ListPaths"] = ListPaths;
            
           
            return View();
        }


        /// <summary>
        /// Shows the image
        /// </summary>
        /// <param string="pth"></param>
        /// <param string="img"></param>
        /// <param string="name"></param>
        /// <return image view></return>
        public ActionResult ShowImage(string img, string path, string name)
        {
            ViewData["path"] = path;
            ViewData["date"] = img;
            ViewData["name"] = name;

            return View();
        }

        /// <summary>
        /// Shows delete handler view
        /// </summary>
        /// <param string="path"></param>
        /// <return remove handler view></return>
        public ActionResult RemoveHandlerOther(string path)
        {
            ViewData["path"] = path;
            return View();
        }

        /// <summary>
        /// Goes back to the config view
        /// </summary>
        /// <return  config view></return>
        public ActionResult GoBack()
        {
            return RedirectToAction("Config", new { load = false });
        }

        /// <summary>
        /// Delete the image view
        /// </summary>
        /// <param string="pth"></param>
        /// <param string="img"></param>
        /// <param string="name"></param>
        /// <return deleet image view></return>
        public ActionResult DeleteImg(string img, string path, string name)
        {
            ViewData["path"] = path;
            ViewData["date"] = img;
            ViewData["name"] = name;
            return View();
        }

        /// <summary>
        /// Shows the log view
        /// </summary>
        /// <return log view></return>
        public ActionResult Logs()
        {
            SetLogs();
            ViewData["ListLogs"] = ListCommands;
            return View();
        }

        /// <summary>
        /// Shows the image view
        /// </summary>
        /// <param string="path"></param>
        /// <return image view></return>
        public ActionResult Images(string path)
        {
            if (path != null)
            {
                this.RemoveImage(path);
            }
            setImage();
            ViewData["ImageList"] = images;

            return View();
        }

        /// <summary>
        /// Removes the image
        /// </summary>
        /// <param string="path"></param>
        public void RemoveImage(string path)
        {

            String paths = Server.MapPath(path);

            if (System.IO.File.Exists(paths)) { System.IO.File.Delete(paths); }

        }

        /// <summary>
        /// Sets the configuration
        /// </summary>
        /// <param bool="load"></param>
        public void SetConfig(bool load)
        {
            IList<string> eachPath = m.GetConfig();
            if (load && eachPath.Count == 5)
            {
                Output = Output + eachPath[4];
                Source = Source + eachPath[3];
                LogName = LogName + eachPath[2];
                ThumbName = ThumbName + eachPath[1];
                string lPaths = eachPath[0];

                IList<string> each = lPaths.Split(';').Reverse().ToList<string>();
                each.ToList().ForEach(ListPaths.Add);
            }
        }

        /// <summary>
        /// Sets the images
        /// </summary>
        public void setImage()
        {
            IList<string> img = m.GetImage();
            foreach (string pic in img)
            {
                string file2 = pic.Replace(@"C:\Users\IEUser\source\repos\ServiceWebApp\ServiceWebApp\", @"~\");
                DateTime dt = GetDateTakenFromImage(pic);
                string date = dt.Month.ToString() + "/" + dt.Year.ToString();
                string name = Path.GetFileNameWithoutExtension(pic);
                images.Add(new ImageInfo(file2, date, name));

            }
           
        }
        private static Regex r = new Regex(":");

        /// <summary>
        /// Date and time of the images
        /// </summary>
        /// <param string="path"></param>
        /// <return date and time></return>
        public static DateTime GetDateTakenFromImage(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    
                    return DateTime.Parse(dateTaken);
                }
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }
        /// <summary>
        /// Sets the logs, gets them from the model - service
        /// </summary>
        public void SetLogs()
        {
            string info = m.GetLogs();

            IList<string> eachPath = info.Split('*').Reverse().ToList<string>();

            for (int i = 0; i < eachPath.Count; i++)
            {
                IList<string> each = eachPath[i].Split(',').Reverse().ToList<string>();
                if (each.Count == 2)
                {
                    lock (lockObj)
                    {
                        Int32.TryParse(each[1], out int x);
                        if (x == (int)MessageTypeEnum.INFO)
                            ListCommands.Add(new CommandInfo(MessageTypeEnum.INFO.ToString(), each[0]));
                        if (x == (int)MessageTypeEnum.FAIL)
                            ListCommands.Add(new CommandInfo(MessageTypeEnum.FAIL.ToString(), each[0]));
                        if (x == (int)MessageTypeEnum.WARNING)
                            ListCommands.Add(new CommandInfo(MessageTypeEnum.WARNING.ToString(), each[0]));
                    }
                } /// <summary>
        /// Removes the handlers
        /// </summary>
        /// <param string="path"></param>
        /// <return remove handler view></return>
            }
        }

       
        public ActionResult RemoveHandler(string path)
        {
            
            bool res = m.RemoveHandler(path);
            if (res)
            {
                ListPaths.Remove(path);
            }
            return RedirectToAction("Config", new { load = false });
        }
      
    }
}
