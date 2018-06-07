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

        public ImageInfo(string pth, string img)
        {
            Path = pth;
            image = img;
        }
    }

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

        public ActionResult Logs()
        {
            SetLogs();
            ViewData["ListLogs"] = ListCommands;
            return View();
        }

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

        public void RemoveImage(string path)
        {

            String paths = Server.MapPath(path);

            if (System.IO.File.Exists(paths)) { System.IO.File.Delete(paths); }

        }

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

        public void setImage()
        {
            IList<string> img = m.GetImage();
            foreach (string pic in img)
            {
                string file2 = pic.Replace(@"C:\Users\IEUser\source\repos\ServiceWebApp\ServiceWebApp\", @"~\");
                DateTime dt = GetDateTakenFromImage(pic);
                string date = dt.Month.ToString() + "/" + dt.Year.ToString();
                images.Add(new ImageInfo(file2, date));

            }
           
        }
        private static Regex r = new Regex(":");
        /// </summary>
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
                }
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
